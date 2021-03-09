using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    public enum PaypalTransactionStatusEnum
    {
        [Description("authorization_expired")] AUTHORIZATION_EXPIRED,
        [Description("authorized")] AUTHORIZED,
        [Description("authorizing")] AUTHORIZING,
        [Description("failed")] FAILED,
        [Description("gateway_rejected")] GATEWAY_REJECTED,
        [Description("processor_declined")] PROCESSOR_DECLINED,
        [Description("settled")] SETTLED,
        [Description("settling")] SETTLING,
        [Description("submitted_for_settlement")] SUBMITTED_FOR_SETTLEMENT,
        [Description("voided")] VOIDED,
        [Description("unrecognized")] UNRECOGNIZED,
        [Description("settlement_confirmed")] SETTLEMENT_CONFIRMED,
        [Description("settlement_declined")] SETTLEMENT_DECLINED,
        [Description("settlement_pending")] SETTLEMENT_PENDING,
    }
    public class PaymentPaypalTransaction
    {
        [Key]
        [Display(Name = "Transaction Id")]
        public int PaymentPaypalTransactionId { get; set; }
        public string PaypalCustomerId { get; set; }
        public Braintree.TransactionStatus TransactionStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateSettled { get; set; }

        public string SettlementAuthorisationCode { get; set; }
        public bool PaymentSuccessful { get; set; }
        public bool IsCardFailure { get; set; }
        public string FailureReasons { get; set; }
        public string AcquirerReferenceNumber { get; set; }
        public string AuthorizedTransactionId { get; set; }

    }
}