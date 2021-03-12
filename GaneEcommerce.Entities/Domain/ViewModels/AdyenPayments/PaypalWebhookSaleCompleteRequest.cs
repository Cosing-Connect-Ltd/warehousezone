using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models.PaypalPayments
{

    public class PaypalWebhookSaleCompleteRequest
    {
        public string id { get; set; }
        public DateTime create_time { get; set; }
        public string resource_type { get; set; }
        public string event_type { get; set; }
        public string summary { get; set; }
        public Resource resource { get; set; }
        public string status { get; set; }
        public List<Transmission> transmissions { get; set; }
        public List<Link> links { get; set; }
        public string event_version { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Details
    {
        public string subtotal { get; set; }
    }

    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
        public Details details { get; set; }
    }

    public class TransactionFee
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Link
    {
        public string method { get; set; }
        public string rel { get; set; }
        public string href { get; set; }
        public string encType { get; set; }
    }

    public class Resource
    {
        public Amount amount { get; set; }
        public string payment_mode { get; set; }
        public DateTime create_time { get; set; }
        public TransactionFee transaction_fee { get; set; }
        public string billing_agreement_id { get; set; }
        public string parent_payment { get; set; }
        public DateTime update_time { get; set; }
        public string protection_eligibility_type { get; set; }
        public string protection_eligibility { get; set; }
        public List<Link> links { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public string invoice_number { get; set; }
    }

    public class Transmission
    {
        public string webhook_url { get; set; }
        public string transmission_id { get; set; }
        public string status { get; set; }
    }

}