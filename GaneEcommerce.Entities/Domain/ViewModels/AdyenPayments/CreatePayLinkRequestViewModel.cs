﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ganedata.Core.Models.AdyenPayments
{
    public class AdyenCreatePayLinkRequestModel
    {
        [JsonIgnore]
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

        [JsonProperty("storePaymentMethod")] 
        public bool StorePaymentMethod { get; set; } = true;

        [JsonProperty("recurringProcessingModel")]
        public string RecurringProcessingModel { get; set; } = "CardOnFile";
        [JsonProperty("billingAddress")]
        public AdyenBillingAddress BillingAddress { get; set; }
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
        [JsonProperty("value")]
        public decimal Value { get; set; }
        [JsonProperty("currency")]
        public string CurrencyCode { get; set; }
    }

    public class AdyenBillingAddress
    {
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("houseNumberOrName")]
        public string houseNumberOrName { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
    }
}