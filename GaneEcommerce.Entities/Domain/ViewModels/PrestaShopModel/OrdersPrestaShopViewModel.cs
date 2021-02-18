using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain.ViewModels
{

    [XmlRoot(ElementName = "id_address_delivery")]
    public class Id_address_delivery
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "id_address_invoice")]
    public class Id_address_invoice
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "id_cart")]
    public class Id_cart
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "id_currency")]
    public class Id_currency
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "id_lang")]
    public class Id_lang
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }



    [XmlRoot(ElementName = "id_carrier")]
    public class Id_carrier
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "current_state")]
    public class Current_state
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "shipping_number")]
    public class Shipping_number
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
    }

    [XmlRoot(ElementName = "order_row")]
    public class Order_row
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "product_id")]
        public int Product_id { get; set; }
        [XmlElement(ElementName = "product_attribute_id")]
        public string Product_attribute_id { get; set; }
        [XmlElement(ElementName = "product_quantity")]
        public decimal Product_quantity { get; set; }
        [XmlElement(ElementName = "product_name")]
        public string Product_name { get; set; }
        [XmlElement(ElementName = "product_reference")]
        public string Product_reference { get; set; }
        [XmlElement(ElementName = "product_ean13")]
        public string Product_ean13 { get; set; }
        [XmlElement(ElementName = "product_isbn")]
        public string Product_isbn { get; set; }
        [XmlElement(ElementName = "product_upc")]
        public string Product_upc { get; set; }
        [XmlElement(ElementName = "product_price")]
        public string Product_price { get; set; }
        [XmlElement(ElementName = "unit_price_tax_incl")]
        public decimal Unit_price_tax_incl { get; set; }
        [XmlElement(ElementName = "unit_price_tax_excl")]
        public string Unit_price_tax_excl { get; set; }
    }

    [XmlRoot(ElementName = "order_rows")]
    public class Order_rows
    {
        [XmlElement(ElementName = "order_row")]
        public List<Order_row> Order_row { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "virtualEntity")]
        public string VirtualEntity { get; set; }
    }

    [XmlRoot(ElementName = "associations")]
    public class AssociationsOrders
    {
        [XmlElement(ElementName = "order_rows")]
        public Order_rows Order_rows { get; set; }
    }

    [XmlRoot(ElementName = "order")]
    public class PrestaShopOrder
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "id_address_delivery")]
        public Id_address_delivery Id_address_delivery { get; set; }
        [XmlElement(ElementName = "id_address_invoice")]
        public Id_address_invoice Id_address_invoice { get; set; }
        [XmlElement(ElementName = "id_cart")]
        public Id_cart Id_cart { get; set; }
        [XmlElement(ElementName = "id_currency")]
        public Id_currency Id_currency { get; set; }
        [XmlElement(ElementName = "id_lang")]
        public Id_lang Id_lang { get; set; }
        [XmlElement(ElementName = "id_customer")]
        public Id_customer Id_customer { get; set; }
        [XmlElement(ElementName = "id_carrier")]
        public Id_carrier Id_carrier { get; set; }
        [XmlElement(ElementName = "current_state")]
        public Current_state Current_state { get; set; }
        [XmlElement(ElementName = "module")]
        public string Module { get; set; }
        [XmlElement(ElementName = "invoice_number")]
        public string Invoice_number { get; set; }
        [XmlElement(ElementName = "invoice_date")]
        public string Invoice_date { get; set; }
        [XmlElement(ElementName = "delivery_number")]
        public string Delivery_number { get; set; }
        [XmlElement(ElementName = "delivery_date")]
        public string Delivery_date { get; set; }
        [XmlElement(ElementName = "valid")]
        public string Valid { get; set; }
        [XmlElement(ElementName = "urgent")]
        public int Urgent { get; set; }
        [XmlElement(ElementName = "next_day_delivery")]
        public int Next_day_delivery { get; set; }
        [XmlElement(ElementName = "date_add")]
        public string Date_add { get; set; }
        [XmlElement(ElementName = "date_upd")]
        public string Date_upd { get; set; }
        [XmlElement(ElementName = "shipping_number")]
        public Shipping_number Shipping_number { get; set; }
        [XmlElement(ElementName = "id_shop_group")]
        public string Id_shop_group { get; set; }
        [XmlElement(ElementName = "id_shop")]
        public string Id_shop { get; set; }
        [XmlElement(ElementName = "secure_key")]
        public string Secure_key { get; set; }
        [XmlElement(ElementName = "payment")]
        public string Payment { get; set; }
        [XmlElement(ElementName = "recyclable")]
        public string Recyclable { get; set; }
        [XmlElement(ElementName = "gift")]
        public string Gift { get; set; }
        [XmlElement(ElementName = "gift_message")]
        public string Gift_message { get; set; }
        [XmlElement(ElementName = "mobile_theme")]
        public string Mobile_theme { get; set; }
        [XmlElement(ElementName = "total_discounts")]
        public string Total_discounts { get; set; }
        [XmlElement(ElementName = "total_discounts_tax_incl")]
        public string Total_discounts_tax_incl { get; set; }
        [XmlElement(ElementName = "total_discounts_tax_excl")]
        public string Total_discounts_tax_excl { get; set; }
        [XmlElement(ElementName = "total_paid")]
        public string Total_paid { get; set; }
        [XmlElement(ElementName = "total_paid_tax_incl")]
        public string Total_paid_tax_incl { get; set; }
        [XmlElement(ElementName = "total_paid_tax_excl")]
        public string Total_paid_tax_excl { get; set; }
        [XmlElement(ElementName = "total_paid_real")]
        public string Total_paid_real { get; set; }
        [XmlElement(ElementName = "total_products")]
        public string Total_products { get; set; }
        [XmlElement(ElementName = "total_products_wt")]
        public string Total_products_wt { get; set; }
        [XmlElement(ElementName = "total_shipping")]
        public string Total_shipping { get; set; }
        [XmlElement(ElementName = "total_shipping_tax_incl")]
        public string Total_shipping_tax_incl { get; set; }
        [XmlElement(ElementName = "total_shipping_tax_excl")]
        public string Total_shipping_tax_excl { get; set; }
        [XmlElement(ElementName = "carrier_tax_rate")]
        public string Carrier_tax_rate { get; set; }
        [XmlElement(ElementName = "total_wrapping")]
        public string Total_wrapping { get; set; }
        [XmlElement(ElementName = "total_wrapping_tax_incl")]
        public string Total_wrapping_tax_incl { get; set; }
        [XmlElement(ElementName = "total_wrapping_tax_excl")]
        public string Total_wrapping_tax_excl { get; set; }
        [XmlElement(ElementName = "round_mode")]
        public string Round_mode { get; set; }
        [XmlElement(ElementName = "round_type")]
        public string Round_type { get; set; }
        [XmlElement(ElementName = "conversion_rate")]
        public string Conversion_rate { get; set; }
        [XmlElement(ElementName = "reference")]
        public string Reference { get; set; }
        [XmlElement(ElementName = "associations")]
        public AssociationsOrders Associations { get; set; }
    }

    [XmlRoot(ElementName = "orders")]
    public class ListOrders
    {
        [XmlElement(ElementName = "order")]
        public List<PrestaShopOrder> Order { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class PrestashopOrders
    {
        [XmlElement(ElementName = "orders")]
        public ListOrders Orders { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class PrestashopOrderSingle
    {
        [XmlElement(ElementName = "order")]
        public PrestaShopOrder Order { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }

    public enum PrestashopOrderStateEnum
    {
        Preparation=3,
        PaymentError=8,
        RemotePaymentAccepted=11,
        OrderAccepted=2,
        PickAndPack=4,
        Shipped=5,
        Canceled=6,
        Refunded=7,
        OnBackorderPaid=9,
        OnBackorderNotPaid=12,
        PendingPayment=14,
        OnHold=17,
        PartiallyPaid=18,
        AwaitingForBraintreePayment=19,
        AwaitingForBraintreeValidation=20,
        Updating=21,
        KlarnaPendingPayment=22,
        KlarnaPaymentStopped=23,
        KlarnaFraudReviewPending=24,
        KlarnaCaptureError=25,
        KlarnaPendingInvoice=26,
        KlarnaPendingPartpayment=27,
        KlarnaAcceptedInvoice=28,
        KlarnaAcceptedPartpayment=29,
        KlarnaPaymentAccepted=30,
        KlarnaPaymentRejected=31,
        AwaitingForSofortPayment=38,
        StripePartialRefund=39,
        WaitingForStripecapture=40,
        WaitingForSepaPayment=41,
        SepaDispute=42,
        AwaitingCashOnDeliveryValidation=13,
        WaitingForPayPalPayment=32,
        WaitingForCreditCardPayment=33,
        WaitingForLocalPaymentMethodPayment=34,
        AuthorizedToBeCapturedByMerchant=35,
        PartialRefund=36,
        WaitingCapture=37,
        AwaitingCheckPayment=1,
        AwaitingBankWirePayment=10
    }

}