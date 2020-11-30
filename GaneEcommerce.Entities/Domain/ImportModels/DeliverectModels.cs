using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ganedata.Core.Entities.Domain.ImportModels
{
    public class DeliverectApiToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }

    public class DeliverectProducts
    {
        [JsonProperty("_items")]
        public List<DeliverectProduct> Products { get; set; }
        [JsonProperty("_meta")]
        public DeliverectMetaData Meta { get; set; }
    }

    public class DeliverectMetaData
    {
        [JsonProperty("page")]
        public int Page { get; set; }
        [JsonProperty("max_results")]
        public int MaxResults { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class DeliverectProduct
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }
        [JsonProperty("productType")]
        public int Type { get; set; }
        [JsonProperty("deliveryTax")]
        public decimal DeliveryTax { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("plu")]
        public string PLU { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("_etag")]
        public string Etag { get; set; }
        [JsonProperty("productTags")]
        public string[] ProductTags { get; set; }
        [JsonProperty("isInternal")]
        public bool IsInternal { get; set; }
    }

    public class DeliverectChannelLinks
    {
        [JsonProperty("_items")]
        public List<DeliverectChannelLink> ChannelLinks { get; set; }
        [JsonProperty("_meta")]
        public DeliverectMetaData Meta { get; set; }
    }

    public class DeliverectChannelRegisterWebhookRequest
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("channelLocationId")]
        public string ChannelLocationId { get; set; }
        [JsonProperty("channelLinkId")]
        public string ChannelLinkId { get; set; }
    }

    public class DeliverectChannelRegisterWebhookResponse
    {
        [JsonProperty("statusUpdateURL")]
        public string StatusUpdateURL { get; set; }
        [JsonProperty("menuUpdateURL")]
        public string MenuUpdateURL { get; set; }
        [JsonProperty("disabledProductsURL")]
        public string DisabledProductsURL { get; set; }
        [JsonProperty("snoozeUnsnoozeURL")]
        public string SnoozeUnsnoozeURL { get; set; }
        [JsonProperty("busyModeURL")]
        public string BusyModeURL { get; set; }
    }

    public class DeliverectMenuUpdatedWebhookRequest
    {
        [JsonProperty("channelLinkId")]
        public string ChannelLinkId { get; set; }
    }

    public class DeliverectProductSnoozeChangedWebhookRequest
    {
        [JsonProperty("channelLinkId")]
        public string ChannelLinkId { get; set; }
    }

    public class DeliverectOrderStatusUpdatedWebhookRequest
    {
        [JsonProperty("channelOrderId")]
        public string ChannelOrderId { get; set; }
        [JsonProperty("orderId")]
        public string DeliverectOrderId { get; set; }
        [JsonProperty("location")]
        public string DeliverectLocationId { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }
    }

    public class DeliverectChannelLink
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("_deleted")]
        public bool Deleted { get; set; }
        [JsonProperty("taxExcl")]
        public bool TaxExcl { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("posSystemId")]
        public string PosSystemId { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("menuUrl")]
        public string MenuUrl { get; set; }
        [JsonProperty("bufferOrders")]
        public bool bufferOrders { get; set; }
        [JsonProperty("deliveryPLU")]
        public string DeliveryPLU { get; set; }
        [JsonProperty("serviceChargePLU")]
        public string ServiceChargePLU { get; set; }
        [JsonProperty("discountPLU")]
        public string DiscountPLU { get; set; }
        [JsonProperty("anonymizeCustomer")]
        public bool AnonymizeCustomer { get; set; }
        [JsonProperty("application")]
        public string Application { get; set; }
        [JsonProperty("sendDeliveryFee")]
        public bool SendDeliveryFee { get; set; }
        [JsonProperty("sendServiceCharge")]
        public bool SendServiceCharge { get; set; }
        [JsonProperty("sendDiscount")]
        public bool SendDiscount { get; set; }
        [JsonProperty("posOrdersAreAlwaysPaid")]
        public bool PosOrdersAreAlwaysPaid { get; set; }
        [JsonProperty("sendDeliveryFeeCondition")]
        public int SendDeliveryFeeCondition { get; set; }
        [JsonProperty("deliverySystem")]
        public DeliverectChannelDeliverySystem DeliverySystem { get; set; }
        [JsonProperty("channelSettings")]
        public DeliverectChannelSettings ChannelSettings { get; set; }
    }

    public class DeliverectChannelDeliverySystem
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("accessProfileCode")]
        public string AccessProfileCode { get; set; }
    }

    public class DeliverectChannelSettings
    {
        [JsonProperty("channelLocationId")]
        public int? ChannelLocationId { get; set; }
        [JsonProperty("activateURL")]
        public string ActivateURL { get; set; }
        [JsonProperty("menuUpdateURL")]
        public string MenuUpdateURL { get; set; }
        [JsonProperty("snoozeUnsnoozeURL")]
        public string SnoozeUnsnoozeURL { get; set; }
        [JsonProperty("busyModeURL")]
        public string BusyModeURL { get; set; }
        [JsonProperty("statusUpdateURL")]
        public string StatusUpdateURL { get; set; }
        [JsonProperty("updateMenuPrepTimeURL")]
        public string UpdateMenuPrepTimeURL { get; set; }
        [JsonProperty("application")]
        public string Application { get; set; }
        [JsonProperty("alwaysSendAllSnoozedProducts")]
        public bool AlwaysSendAllSnoozedProducts { get; set; }
        [JsonProperty("alwaysSendCalories")]
        public bool AlwaysSendCalories { get; set; }
        [JsonProperty("sendDeliveryFee")]
        public bool SendDeliveryFee { get; set; }
        [JsonProperty("sendServiceCharge")]
        public bool SendServiceCharge { get; set; }

        [JsonProperty("sendDiscount")]
        public bool SendDiscount { get; set; }
        [JsonProperty("sendRejectStatus")]
        public bool SendRejectStatus { get; set; }
        [JsonProperty("posOrdersAreAlwaysPaid")]
        public bool PosOrdersAreAlwaysPaid { get; set; }
        [JsonProperty("sendDeliveryFeeCondition")]
        public int SendDeliveryFeeCondition { get; set; }
        [JsonProperty("allowUpdateMenuPrepTime")]
        public bool AllowUpdateMenuPrepTime { get; set; }
        [JsonProperty("sendTableInfoToNotes")]
        public bool SendTableInfoToNotes { get; set; }
    }

    public class DeliverectOrder
    {
        [JsonProperty("channelOrderId")]
        public string ChannelOrderId { get; set; }
        [JsonProperty("channelOrderDisplayId")]
        public string ChannelOrderDisplayId { get; set; }
        [JsonProperty("channelLinkId")]
        public string ChannelLinkId { get; set; }
        [JsonProperty("by")]
        public string CreatedBy { get; set; }
        [JsonProperty("orderType")]
        public int OrderType { get; set; }
        [JsonProperty("table")]
        public string Table { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("pickupTime")]
        public string PickupTime { get; set; }
        [JsonProperty("estimatedPickupTime")]
        public string EstimatedPickupTime { get; set; }
        [JsonProperty("deliveryTime")]
        public string DeliveryTime { get; set; }
        [JsonProperty("deliveryIsAsap")]
        public bool DeliveryIsAsap { get; set; }
        [JsonProperty("courier")]
        public string Courier { get; set; }
        [JsonProperty("orderIsAlreadyPaid")]
        public bool OrderIsAlreadyPaid { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("decimalDigits")]
        public int decimalDigits { get; set; }
        [JsonProperty("numberOfCustomers")]
        public int NumberOfCustomers { get; set; }
        [JsonProperty("deliveryCost")]
        public decimal DeliveryCost { get; set; }
        [JsonProperty("serviceCharge")]
        public decimal ServiceCharge { get; set; }
        [JsonProperty("discountTotal")]
        public decimal DiscountTotal { get; set; }
        [JsonProperty("customer")]
        public DeliverectOrderCustomer Customer { get; set; }
        [JsonProperty("deliveryAddress")]
        public DeliverectOrderDeliveryAddress DeliveryAddress { get; set; }
        [JsonProperty("payment")]
        public DeliverectOrderPayment Payment { get; set; }
        [JsonProperty("items")]
        public List<DeliverectOrderItem> Items { get; set; }
    }

    public class DeliverectOrderCustomer
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class DeliverectOrderPayment
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public class DeliverectOrderItem
    {
        [JsonProperty("plu")]
        public string PLU { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("productType")]
        public int ProductType { get; set; }
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }
        [JsonProperty("remark")]
        public string Remark { get; set; }
        [JsonProperty("subItems")]
        public List<DeliverectOrderItem> SubItems { get; set; }
    }

    public class DeliverectOrderDeliveryAddress
    {
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("streetNumber")]
        public string StreetNumber { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("extraAddressInfo")]
        public string ExtraAddressInfo { get; set; }
    }
}