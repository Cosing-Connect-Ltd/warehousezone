using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ganedata.Core.Models.AdyenPayments
{
    public class AdyenCreatePayLinkRequestModel
    {
        public int OrderID { get; set; }

        [JsonProperty("reference")]
        public string PaymentReference { get; set; }

        [JsonProperty("amount")]
        public AdyenAmount Amount { get; set; } = new AdyenAmount();
        [JsonProperty("shopperReference")]
        public string ShopperUniqueReference { get; set; }
        [JsonProperty("description")]
        public string OrderDescription { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; } = "GB";
        [JsonProperty("merchantAccount")]
        public string MerchantAccount { get; set; }
        [JsonProperty("shopperLocale")]
        public string ShopperLocale { get; set; } = "en-GB";
    }
    public class AdyenCreatePayLinkResponseModel : AdyenCreatePayLinkRequestModel
    {
        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class AdyenAmount
    {
        public AdyenAmount()
        {
            CurrencyCode = "GBP";
        }
        public decimal Value { get; set; }
        public string CurrencyCode { get; set; }
    }
}