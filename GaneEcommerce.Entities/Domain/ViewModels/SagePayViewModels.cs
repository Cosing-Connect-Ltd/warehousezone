using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class SagepayVendor
    {
        public string vendorName { get; set; }
    }

    public class SagepayTokenResponse
    {
        public string merchantSessionKey { get; set; }
        public DateTime expiry { get; set; }
    }
    [Serializable]
    public class Card
    {
        public string merchantSessionKey { get; set; }
        public string cardIdentifier { get; set; }
        public string save { get; set; }

    }
    [Serializable]
    public class PaymentMethod
    {
        public Card card { get; set; }

    }

    public class BillingAddress
    {
        public string address1 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }

    }

    public class SagePayPaymentViewModel
    {
        public string transactionType { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string vendorTxCode { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string apply3DSecure { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public BillingAddress billingAddress { get; set; }
        public string entryMethod { get; set; }

    }

    [Serializable]
    public class AvsCvcCheck
    {
        public string status { get; set; }
        public string address { get; set; }
        public string postalCode { get; set; }
        public string securityCode { get; set; }

    }
    [Serializable]
    public class Amount
    {
        public int totalAmount { get; set; }
        public int saleAmount { get; set; }
        public int surchargeAmount { get; set; }

    }
    [Serializable]
    public class SagePayPaymentResponseViewModel
    {
        public string transactionId { get; set; }
        public string transactionType { get; set; }
        public string status { get; set; }
        public string statusCode { get; set; }
        public string statusDetail { get; set; }
        public int retrievalReference { get; set; }
        public string bankResponseCode { get; set; }
        public string bankAuthorisationCode { get; set; }
        public AvsCvcCheck avsCvcCheck { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public Amount amount { get; set; }
        public string currency { get; set; }

    }
}