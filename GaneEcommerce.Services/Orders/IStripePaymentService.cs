using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web.Configuration;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Stripe;
using Stripe.Checkout;

namespace Ganedata.Core.Services
{

    public interface IStripePaymentService
    {
        StripePaymentAuthorisationResponse CreatePayment(StripePaymentRequestModel request);
        StripePaymentAuthorisationResponse ChargeOrder(StripePaymentChargeCapture model);
        object GetChargeInformation();
    }

    public class StripePaymentService : IStripePaymentService
    {
        private readonly IUserService _userService;
        private readonly IAccountServices _accountServices;
        private readonly IOrderService _orderService;
        private readonly IApplicationContext _currentDbContext;

        public StripePaymentService(IUserService userService, IAccountServices accountServices,
            IOrderService orderService, IApplicationContext currentDbContext)
        {
            _userService = userService;
            _accountServices = accountServices;
            _orderService = orderService;
            _currentDbContext = currentDbContext;
        }

        public static bool StripeIsLive => (WebConfigurationManager.AppSettings["StripeIsLive"]??"").ToLower()=="true";

        //public static string TestPublishableApiKey = "pk_test_51IOjRSHE8E2m6CoUGQlOPAhoGLzxEsGRJOmFW99n7PuHI595DBKYBFpHAn0faBFAIJPJM6pLrXgXu9Eps8DiLnvL00vlGsrFrc";

        public static string TestSecretKey = WebConfigurationManager.AppSettings["StripeTestSecretKey"] ??"sk_test_51IOjRSHE8E2m6CoUEQZUxzyMuLGH4xn5Ba5AqWTctxPbdriI2STK59TNc9hqmiHvJusivHqTVguNO2U7ivk5scz200muqM8gNX";

        //public static string LivePublishableApiKey = "pk_live_51IOjRSHE8E2m6CoUofToMDWnsS4XSW7KwPRSwpPEZXWlm5TUiuAF5XXEaoTyzoRZTatLCWHeqSO6nD1yjyzI1vEO00JjReH0Rx";

        public static string LiveSecretKey = WebConfigurationManager.AppSettings["StripeLiveSecretKey"] ?? "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";

        private string GetSecretKey()
        {
            return StripeIsLive ? LiveSecretKey : TestSecretKey;
        }

        public StripePaymentAuthorisationResponse CreatePayment(StripePaymentRequestModel request)
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var user = _userService.GetAuthUserById(request.UserId);

            try
            {
                if (user != null)
                {

                    var account = _accountServices.GetAccountsById(user.AccountId ?? 0);

                    if (account != null)
                    {
                        var warehouse =
                            _currentDbContext.TenantWarehouses.FirstOrDefault(m =>
                                m.WarehouseId == request.TenantLocationId);

                        if (warehouse == null)
                        {
                            return new StripePaymentAuthorisationResponse
                            {
                                FailureReasons = new List<string>()
                                {
                                    "Invalid warehouse location"
                                },
                                PaymentSuccessful = false
                            };
                        }

                        var chargeOptions = new ChargeCreateOptions
                        {
                            Amount = Convert.ToInt32((Math.Round(request.Amount, 2) * 100)),
                            Currency = "GBP",
                            Source = request.ChargeToken,
                            Capture = false
                        };
                        
                        var chargeService = new ChargeService(new StripeClient(GetSecretKey()));
                        var response = chargeService.Create(chargeOptions);
                        var stripeAutoAccept = warehouse?.LoyaltyAutoAcceptOrders ?? true;

                        if (!response.Paid || !response.Status.Equals("succeeded", StringComparison.CurrentCultureIgnoreCase))
                        {
                            return new StripePaymentAuthorisationResponse
                            {
                                FailureReasons = new List<string>()
                                {
                                    "Payment could not be completed"
                                },
                                PaymentSuccessful = false
                            };
                        }

                        var order = _currentDbContext.Order.Find(request.OrderId);
                        if (order == null)
                        {
                            return new StripePaymentAuthorisationResponse
                            {
                                FailureReasons = new List<string>()
                                {
                                    "Order cannot be found"
                                },
                                PaymentSuccessful = false
                            };
                        }

                        order.OrderStatusID = OrderStatusEnum.Complete;
                        order.StripeChargeInformation = new StripeChargeInformation()
                        {
                            OrderId = request.OrderId,
                            StripeAutoCharge = stripeAutoAccept,
                            StripeChargedAmount = request.Amount,
                            StripeChargeCreatedId = response.Id,
                            StripeChargeToken = request.ChargeToken,
                            StripeChargedCurrency = chargeOptions.Currency,
                            StripeChargedCreatedDate = DateTime.Now,
                            StripeChargedConfirmedDate = stripeAutoAccept? DateTime.Now: (DateTime?)null
                            
                        };

                        _currentDbContext.Entry(order).State = EntityState.Modified;
                        _currentDbContext.SaveChanges();

                        if (stripeAutoAccept)
                        {
                            var chargeInfo = ChargeOrder(new StripePaymentChargeCapture()
                            {
                                OrderId = request.OrderId,
                                UserId = request.UserId
                            });
                            if (!chargeInfo.PaymentSuccessful)
                            {
                                return chargeInfo;
                            }
                        }

                        return new StripePaymentAuthorisationResponse
                        {
                            AuthorisationCode = response.Id,
                            FailureReasons = new List<string>()
                            {
                                response.FailureMessage
                            },
                            PaymentSuccessful = true
                        };
                    }
                }
                return new StripePaymentAuthorisationResponse
                {
                    FailureReasons = new List<string>()
                    {
                        "User account cannot be found"
                    },
                    PaymentSuccessful = false
                };
            }
            catch (Exception ex)
            {
                return new StripePaymentAuthorisationResponse
                {
                    FailureReasons = new List<string>()
                    {
                        ex.Message,
                        ex.Source,
                        ex.StackTrace
                    },
                    PaymentSuccessful = false
                };
            }
        }

        public StripePaymentAuthorisationResponse ChargeOrder(StripePaymentChargeCapture model)
        {
            var order = _orderService.GetOrderById(model.OrderId);
           
            var chargeService = new ChargeService(new StripeClient(GetSecretKey()));
            var response = chargeService.Capture(order.StripeChargeInformation.StripeChargeCreatedId);

            if (response.Paid || response.Captured || response.Refunded)
            {
                return new StripePaymentAuthorisationResponse
                {
                    PaymentSuccessful = true,
                    AuthorisationCode = response.AuthorizationCode
                };
            }

            return new StripePaymentAuthorisationResponse
            {
                PaymentSuccessful = false,
                AuthorisationCode = response.AuthorizationCode,
                FailureReasons =
                {
                    response.FailureCode,
                    response.FailureMessage,
                }
            };
        }

        public object GetChargeInformation()
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var service = new ChargeService();
            var result = service.Get("ch_1IZHhyHE8E2m6CoUZWolCGOj");
            return result;
        }

    }

    #region Models

    public class StripePaymentRequestModel
    {
        public string SerialNumber { get; set; }
        public int UserId { get; set; }
        public string PersonalReferralCode { get; set; }
        public decimal Amount { get; set; }

        //By default, its false as the Tax is always included on Food orders
        public bool ExclusiveOfTax { get; set; }
        public string ChargeToken { get; set; }
        public int TenantLocationId { get; set; }
        public int OrderId { get; set; }
    }

    public class StripePaymentChargeCapture
    {
        public int OrderId { get; set; }
        public string SerialNumber { get; set; }
        public int UserId { get; set; }
        public string PersonalReferralCode { get; set; }
    }

    public class StripePaymentAuthorisationResponse
    {
        public bool PaymentSuccessful { get; set; }
        public List<string> FailureReasons { get; set; }
        public string AuthorisationCode { get; set; }
    }

    #endregion
}