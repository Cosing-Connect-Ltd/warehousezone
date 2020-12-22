using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ganedata.Core.Models.AdyenPayments
{
    public class AdyenPaylinkHookAdditionalData
    {
        [JsonProperty("paymentLinkId")]
        public string PaymentLinkId { get; set; }
    }
     
    public class AdyenPaylinkHookNotificationRequest
    {
        [JsonProperty("additionalData")]
        public AdyenPaylinkHookAdditionalData AdditionalData { get; set; }

        [JsonProperty("amount")]
        public AdyenAmount Amount { get; set; }

        [JsonProperty("eventCode")]
        public string EventCode { get; set; }

        [JsonProperty("merchantAccountCode")]
        public string MerchantAccountCode { get; set; }

        [JsonProperty("merchantReference")]
        public string MerchantReference { get; set; }

        [JsonProperty("pspReference")]
        public string PspReference { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonIgnore]
        public string RawJson { get; set; }
    }

    public class AdyenPaylinkHookNotificationItem
    {
        [JsonProperty("NotificationRequestItem")]
        public AdyenPaylinkHookNotificationRequest NotificationRequestItem { get; set; }
    }

    public class Root
    {
        [JsonProperty("notificationItems")]
        public List<AdyenPaylinkHookNotificationItem> NotificationItems { get; set; }
    }



}