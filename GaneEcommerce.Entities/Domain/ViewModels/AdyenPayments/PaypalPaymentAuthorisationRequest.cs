
using System.Collections.Generic;

namespace Ganedata.Core.Models.PaypalPayments
{
   
    public class PaypalPaymentAuthorisationRequest
    {
        public int OrderID { get; set; }
        public string AuthorisationNonceCode { get; set; }
        public decimal PaymentAmount { get; set; }
        //By default OrderNumber
        public string PaymentReference { get; set; }

        //Optional values
        public bool? HasDiscountApplied { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? OrderAmount { get; set; }
        public int UserId { get; set; }
    }
    public class PaypalPaymentAuthorisationResponse
    {
        public bool PaymentSuccessful { get; set; }
        public bool IsCardFailure { get; set; }
        public List<string> FailureReasons { get; set; }
        public string AcquirerReferenceNumber { get; set; }
        public string AuthorizedTransactionId { get; set; }
        public string TransactionStatus { get; set; }
    }

}