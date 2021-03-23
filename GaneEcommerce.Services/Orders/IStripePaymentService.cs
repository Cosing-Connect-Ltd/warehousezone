using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Stripe;
using Stripe.Checkout;

namespace Ganedata.Core.Services
{
    public interface IStripePaymentService
    {
        object CreatePayment(StripePaymentRequestModel request);
        object CreatePaymentPost();

    }

    public class StripePaymentRequestModel
    {
        public string SerialNumber { get; set; }
        public int UserId { get; set; }
        public string PersonalReferralCode { get; set; }
        public decimal Amount { get; set; }
        //By default, its false as the Tax is always included on Food orders
        public bool ExclusiveOfTax { get; set; }
        public string ChargeToken { get; set; }
    }

    public class StripePaymentService : IStripePaymentService
    {
        private readonly IUserService _userService;
        private readonly IAccountServices _accountServices;

        public StripePaymentService(IUserService userService, IAccountServices accountServices)
        {
            _userService = userService;
            _accountServices = accountServices;
        }
        public static string TestPublishableApiKey = "pk_test_51IOjRSHE8E2m6CoUGQlOPAhoGLzxEsGRJOmFW99n7PuHI595DBKYBFpHAn0faBFAIJPJM6pLrXgXu9Eps8DiLnvL00vlGsrFrc";

        public static string TestSecretKey = "sk_test_51IOjRSHE8E2m6CoUEQZUxzyMuLGH4xn5Ba5AqWTctxPbdriI2STK59TNc9hqmiHvJusivHqTVguNO2U7ivk5scz200muqM8gNX";

        public static string LivePublishableApiKey = "pk_live_51IOjRSHE8E2m6CoUofToMDWnsS4XSW7KwPRSwpPEZXWlm5TUiuAF5XXEaoTyzoRZTatLCWHeqSO6nD1yjyzI1vEO00JjReH0Rx";

        public static string LiveSecretKey = "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";

        public object CreatePayment(StripePaymentRequestModel request)
        {
            StripeConfiguration.ApiKey = "sk_test_51IOjRSHE8E2m6CoUEQZUxzyMuLGH4xn5Ba5AqWTctxPbdriI2STK59TNc9hqmiHvJusivHqTVguNO2U7ivk5scz200muqM8gNX";

            var user = _userService.GetAuthUserById(request.UserId);

            var account = _accountServices.GetAccountsById(user.AccountId ?? 0);

            if (account != null)
            {
                var accountAddress = account.AccountAddresses.FirstOrDefault(m => m.IsDefaultBillingAddress == true);
                if (accountAddress == null)
                {
                    accountAddress = account.AccountAddresses.FirstOrDefault();
                }

                var cs = new CustomerService(new StripeClient(TestSecretKey));
                var customer = cs.Create(new CustomerCreateOptions()
                {
                    Name = user.DisplayName,
                    Address = accountAddress==null? null: new AddressOptions() { City = accountAddress?.Town, Line1  = accountAddress.AddressLine1, },

                });

                var chargeOptions = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32((Math.Round(request.Amount, 2) * 100)),
                    Currency = "GBP",
                    Customer = customer.Id,
                    Source = request.ChargeToken
                };

                var chargeService = new ChargeService(new StripeClient(TestSecretKey));
                var response = chargeService.Create(chargeOptions);

                return new { clientSecret = response.Id };
            }
            else
            {
                return new { ErrorCode = "Customer could not be found"  };
            }
            
        }
 

        public object CreatePaymentPost()
        {
            StripeConfiguration.ApiKey = "";

            var domain = "http://localhost:4242";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 200,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Stubborn Attachments",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return new { id = session.Id };
        }
    }
}