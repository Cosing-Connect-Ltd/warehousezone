using System.Linq;
using System.Web.Configuration;
using System.Web.WebPages;
using Braintree;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models.PaypalPayments;
using Newtonsoft.Json;

namespace Ganedata.Core.Services
{
    public interface IPaypalPaymentServices
    {
        PaypalPaymentAuthorisationResponse SubmitPaypalAuthorisation(PaypalPaymentAuthorisationRequest request);
    }

    public class PaypalPaymentServices : IPaypalPaymentServices
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public PaypalPaymentServices(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }
        //Braintree credentials initialised with Sandbox details, when going live, just updating AppSettings to Live keys will get updated automatically
        public static string BrainTreeMerchantId => WebConfigurationManager.AppSettings["BrainTreeMerchantId"] ?? "bmfnrkfbwpfqkrkm";

        public static string BrainTreePublicKey => WebConfigurationManager.AppSettings["BrainTreePublicKey"] ?? "cmq9c2xcbynf23bv";

        public static string BrainTreePrivateKey => WebConfigurationManager.AppSettings["BrainTreePrivateKey"] ?? "52cfede6f61f5ab39a089e88a0c996ea";

        public PaypalPaymentAuthorisationResponse SubmitPaypalAuthorisation(PaypalPaymentAuthorisationRequest model)
        {
            var payingUser = _userService.GetAuthUserById(model.UserId);

            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = BrainTreeMerchantId,
                PublicKey = BrainTreePublicKey,
                PrivateKey = BrainTreePrivateKey
            };

            var request = new TransactionRequest
            {
                Amount = model.PaymentAmount,
                PaymentMethodNonce = model.AuthorisationNonceCode,
                DeviceData = model.PaymentReference,
                CustomerId = string.IsNullOrWhiteSpace(payingUser.PaypalCustomerId)? null : payingUser.PaypalCustomerId,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    StoreInVault = true,
                    StoreInVaultOnSuccess = true
                },
                CurrencyIsoCode = "GBP"
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (string.IsNullOrWhiteSpace(payingUser.PaypalCustomerId) && result.Transaction?.CustomerDetails != null)
            {
                _userService.UpdateUserPaypalCustomerId(payingUser.UserId, result.Transaction.CustomerDetails.Id);
            }

            var jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);

            var response = new PaypalPaymentAuthorisationResponse()
            {
                PaymentSuccessful = result.IsSuccess(),
                TransactionStatus = result.Transaction.Status.ToString(),
                AcquirerReferenceNumber = result.Transaction.AcquirerReferenceNumber,
                AuthorizedTransactionId = result.Transaction.AuthorizedTransactionId,
                IsCardFailure = result.CreditCardVerification != null &&
                                result.CreditCardVerification.Status != VerificationStatus.VERIFIED,
                FailureReasons = result.Errors != null
                    ? result.Errors.All().Select(m => m.Code + " " + m.Message).ToList()
                    : null
            };

            _orderService.UpdateOrderStatus(model.OrderID, OrderStatusEnum.Complete, model.UserId);

            return response;
        }
    }
}