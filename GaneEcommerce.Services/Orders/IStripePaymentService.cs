using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web.Configuration;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace Ganedata.Core.Services
{

    public interface IStripePaymentService
    {
        StripePaymentAuthorisationResponse CreatePayment(StripePaymentRequestModel request);
        StripePaymentAuthorisationResponse ChargeOrder(StripePaymentChargeCapture model);
        StripePaymentRefundResponse ProcessRefundByOrderId(int orderId, string customerToken);
        object GetChargeInformation();
        StripePaymentRefundResponse ProcessWebhook(StripeWebhookRequest model);
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


        public StripePaymentRefundResponse ProcessRefundByOrderId(int orderId, string customerToken)
        {
            var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.PersonalReferralCode.Equals(customerToken) && m.IsActive);

            if (user == null)
            {
                return new StripePaymentRefundResponse  { Success= false, ErrorMessages = new List<string>(){ "User cannot be found" } };
            }
             
            var order = _currentDbContext.Order.FirstOrDefault(m => m.OrderID == orderId);
            if (order == null || order.StripeChargeInformation==null)
            {
                return new StripePaymentRefundResponse { Success = false, ErrorMessages = new List<string>() { "Order or payment cannot be found" }};
            }

            try
            {
                StripeConfiguration.ApiKey = GetSecretKey();

                var options = new RefundCreateOptions
                {
                    Charge = order.StripeChargeInformation.StripeChargeCreatedId
                };
                var service = new RefundService();
                var refundResponse = service.Create(options);

                if (refundResponse.Status.Equals("succeeded", StringComparison.CurrentCultureIgnoreCase))
                {
                    order.OrderStatusID = OrderStatusEnum.FullyRefunded;

                    if (order.StripeChargeInformation != null)
                    {
                        order.StripeChargeInformation.RefundId = refundResponse.Id;
                        order.StripeChargeInformation.RefundBalanceTransactionId = refundResponse.BalanceTransactionId;
                        order.StripeChargeInformation.RefundCreatedDate = refundResponse.Created;
                        order.StripeChargeInformation.RefundAmount = refundResponse.Amount;
                        order.StripeChargeInformation.RefundAmountCurrency = refundResponse.Currency;
                    }

                    _currentDbContext.Entry(order).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                     
                    return new StripePaymentRefundResponse { Success = true, OrderId = orderId, OrderNumber = order.OrderNumber, RefundAuthorisationCode = order.StripeChargeInformation.RefundId };
                }
                else
                {
                    return new StripePaymentRefundResponse { Success = false, OrderNumber = order.OrderNumber, ErrorMessages = new List<string> { refundResponse.FailureReason}  };
                }

            }
            catch (Exception ex)
            {
                return new StripePaymentRefundResponse { Success = false, ErrorMessages = new List<string> { ex.Message, ex.Source, ex.StackTrace}};
            }
        }

        public object GetChargeInformation()
        {
            StripeConfiguration.ApiKey = GetSecretKey();

            var service = new ChargeService();
            var result = service.Get("ch_1IZHhyHE8E2m6CoUZWolCGOj");
            return result;
        }

        public StripePaymentRefundResponse ProcessWebhook(StripeWebhookRequest model)
        {
            var json = JsonConvert.SerializeObject(model);
            ErrorSignal.FromCurrentContext().Raise(new Exception("Refund payment hook", new Exception(json)));
            return new StripePaymentRefundResponse() {Success = true};
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

    public class StripeRefundOrderPaymentRequest
    {
        public int OrderID { get; set; }
    }

    public class StripePaymentRefundResponse
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string RefundAuthorisationCode { get; set; }
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }

    }

     

    public class StripeWebhookRequestItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("api_version")]
        public object ApiVersion { get; set; }

        [JsonProperty("application")]
        public object Application { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("enabled_events")]
        public List<string> EnabledEvents { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class StripeWebhookRequest
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public List<StripeWebhookRequestItem> Data { get; set; }
    }


    #endregion
}