using System;
using System.Linq;
using System.Web.Configuration;
using System.Web.WebPages;
using Braintree;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models.PaypalPayments;
using Newtonsoft.Json;

namespace Ganedata.Core.Services
{
    public interface IPaypalPaymentServices
    {
        PaypalPaymentAuthorisationResponse SubmitPaypalAuthorisation(PaypalPaymentAuthorisationRequest request);
        bool ReceiveWebHook(PaypalPaymentWebhookRequest request);
    }

    public class PaypalPaymentServices : IPaypalPaymentServices
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IApplicationContext _context;

        public PaypalPaymentServices(IOrderService orderService, IUserService userService, IApplicationContext context)
        {
            _orderService = orderService;
            _userService = userService;
            _context = context;
        }
        //Braintree credentials initialised with Sandbox details, when going live, just updating AppSettings to Live keys will get updated automatically
        public static string BrainTreeMerchantId => WebConfigurationManager.AppSettings["BrainTreeMerchantId"] ?? "bmfnrkfbwpfqkrkm";

        public static string BrainTreePublicKey => WebConfigurationManager.AppSettings["BrainTreePublicKey"] ?? "cmq9c2xcbynf23bv";

        public static string BrainTreePrivateKey => WebConfigurationManager.AppSettings["BrainTreePrivateKey"] ?? "52cfede6f61f5ab39a089e88a0c996ea";

        private BraintreeGateway GetGatewayAccount()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = BrainTreeMerchantId,
                PublicKey = BrainTreePublicKey,
                PrivateKey = BrainTreePrivateKey
            };
            return gateway;
        }

        public PaypalPaymentAuthorisationResponse SubmitPaypalAuthorisation(PaypalPaymentAuthorisationRequest model)
        {
            var payingUser = _userService.GetAuthUserById(model.UserId);

            var gateway = GetGatewayAccount();
            
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
            if (result.Target != null)
            {
                var response = new PaypalPaymentAuthorisationResponse()
                {
                    PaymentSuccessful = result.IsSuccess(),
                    TransactionStatus = result.Target.Status,
                    AcquirerReferenceNumber = result.Target.AcquirerReferenceNumber,
                    AuthorizedTransactionId = result.Target.AuthorizedTransactionId,
                    IsCardFailure = result.CreditCardVerification != null &&
                                    result.CreditCardVerification.Status != VerificationStatus.VERIFIED,
                    FailureReasons = result.Errors != null ? result.Errors.All().Select(m => m.Code + " " + m.Message).ToList()
                        : null
                };
                _orderService.UpdateOrderStatus(model.OrderID, OrderStatusEnum.Complete, model.UserId);

                return response;
            }

            return new PaypalPaymentAuthorisationResponse();
        }

        public bool ReceiveWebHook(PaypalPaymentWebhookRequest request)
        {
            var gateway = GetGatewayAccount();
            WebhookNotification webhookNotification = gateway.WebhookNotification.Parse(
                request.bt_signature, request.bt_payload
            );

            
            var settled = webhookNotification.Kind == WebhookKind.TRANSACTION_SETTLED;

            //var transaction = new PaymentPaypalTransaction()
            //{
            //    TransactionStatus = TransactionStatus.SETTLEMENT_PENDING,
            //    FailureReasons = (webhookNotification.Errors != null
            //        ? string.Join(",", webhookNotification.Errors.All().Select(m => m.Code + " " + m.Message))
            //        : ""),
            //    DateCreated = DateTime.Now
            //};


            var transaction = new PaymentPaypalTransaction()
            {
                PaypalCustomerId = webhookNotification.Transaction.CustomerDetails.Id,
                TransactionStatus = webhookNotification.Transaction.Status,
                AcquirerReferenceNumber = webhookNotification.Transaction.AcquirerReferenceNumber,
                AuthorizedTransactionId = webhookNotification.Transaction.AuthorizedTransactionId,
                FailureReasons = (webhookNotification.Errors != null
                    ? string.Join(",", webhookNotification.Errors.All().Select(m => m.Code + " " + m.Message))
                    : ""),
                DateCreated = DateTime.Now
            };

            if (settled && webhookNotification.Transaction!=null)
            {
                transaction.DateSettled = DateTime.Now;
                transaction.PaymentSuccessful = true;
            }

            _context.PaymentPaypalTransactions.Add(transaction);
            _context.SaveChanges();
            return webhookNotification.Kind == WebhookKind.TRANSACTION_SETTLED;
        }
         
    }
}