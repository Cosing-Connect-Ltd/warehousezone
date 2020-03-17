using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain.ViewModels
{

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
        public int Text { get; set; }
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
        public List<AddressPrestashop> Address { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class PrestashopAddress
    {
        [XmlElement(ElementName = "addresses")]
        public Addresses Addresses { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }




}