using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class PrestaShopProductDetailViewModel
    {
        public int id { get; set; }
        public double? width { get; set; }
        public double? height { get; set; }
        public double? depth { get; set; }
        public double? weight { get; set; }
        public decimal? price { get; set; }
        public decimal? wholesale_price { get; set; }
        public string reference { get; set; }

        public string manufacturer_name { get; set; }

        public string name { get; set; }

        public string description { get; set; }
    }
    public class PresatShopProductName
    {
        public int Id { get; set; }
        public string value { get; set; }
    }
    public class PresatShopProductDesc
    {
        public int Id { get; set; }
        public string value { get; set; }
    }


    public class PrestaShopOrderViewModel
    {

        public int id { get; set; }
        public int id_address_delivery { get; set; }
        public int id_address_invoice { get; set; }
        public int id_cart { get; set; }
        public int id_currency { get; set; }
        public int id_lang { get; set; }
        public int id_customer { get; set; }
        public int id_carrier { get; set; }
        public int current_state { get; set; }
        public string module { get; set; }
        public int invoice_number { get; set; }
        public string shipping_number { get; set; }
        
        public int urgent { get; set; }
        public int next_day_delivery { get; set; }
        public OrderDetailAssosiation associations { get; set; }

    }
    public class PrestaShopProductViewModel
    {
        public int id { get; set; }

        public string reference { get; set; }


    }


    public class OrderDetailAssosiation
    {
        public List<PrestaShopOrderDetailViewModel> order_rows { get; set; }
    }

    public class PrestaShopOrderDetailViewModel
    {

        public int id { get; set; }
        public int product_id { get; set; }
        public int? product_attribute_id { get; set; }
        public int product_quantity { get; set; }
        public string product_name { get; set; }
        public string product_reference { get; set; }
        public string product_ean13 { get; set; }
        public string product_isbn { get; set; }
        public string product_upc { get; set; }
        public decimal product_price { get; set; }
        public int? id_customization { get; set; }
        public decimal unit_price_tax_incl { get; set; }
        public decimal unit_price_tax_excl { get; set; }



    }


    public class PrestaShopAccountViewModel
    {
        public int Id { get; set; }
        public string email { get; set; }

        public string website { get; set; }

        public string company { get; set; }

        public string lastname { get; set; }

        public string firstname { get; set; }


        public string secure_key { get; set; }



    }

   
    public class PrestaShopAccountAddressViewModel
    {
        public int id { get; set; }
        public string company { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string postcode { get; set; }

        public string phone_mobile { get; set; }


    }

    public class stock_available
    {
        public int id_product { get; set; }
        public int? id_product_attribute { get; set; }
        public int quantity { get; set; }
        public int depends_on_stock { get; set; }
        public int out_of_stock { get; set; }

        public int StockAvailableId { get; set; }
    }


    [XmlRoot(ElementName = "id_customer")]
    public class Id_customer
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "id_country")]
    public class Id_country
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "address")]
    public class AddressPrestashop
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "id_customer")]
        public Id_customer Id_customer { get; set; }
        [XmlElement(ElementName = "id_manufacturer")]
        public string Id_manufacturer { get; set; }
        [XmlElement(ElementName = "id_supplier")]
        public string Id_supplier { get; set; }
        [XmlElement(ElementName = "id_warehouse")]
        public string Id_warehouse { get; set; }
        [XmlElement(ElementName = "id_country")]
        public Id_country Id_country { get; set; }
        [XmlElement(ElementName = "id_state")]
        public string Id_state { get; set; }
        [XmlElement(ElementName = "alias")]
        public string Alias { get; set; }
        [XmlElement(ElementName = "company")]
        public string Company { get; set; }
        [XmlElement(ElementName = "lastname")]
        public string Lastname { get; set; }
        [XmlElement(ElementName = "firstname")]
        public string Firstname { get; set; }
        [XmlElement(ElementName = "vat_number")]
        public string Vat_number { get; set; }
        [XmlElement(ElementName = "address1")]
        public string Address1 { get; set; }
        [XmlElement(ElementName = "address2")]
        public string Address2 { get; set; }
        [XmlElement(ElementName = "postcode")]
        public string Postcode { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "other")]
        public string Other { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "phone_mobile")]
        public string Phone_mobile { get; set; }
        [XmlElement(ElementName = "dni")]
        public string Dni { get; set; }
        [XmlElement(ElementName = "deleted")]
        public string Deleted { get; set; }
        [XmlElement(ElementName = "date_add")]
        public string Date_add { get; set; }
        [XmlElement(ElementName = "date_upd")]
        public string Date_upd { get; set; }
    }

    [XmlRoot(ElementName = "addresses")]
    public class Addresses
    {
        [XmlElement(ElementName = "address")]
        public AddressPrestashop Address { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class Prestashop
    {
        [XmlElement(ElementName = "addresses")]
        public Addresses Addresses { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }




}