using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain
{


    [XmlRoot(ElementName = "id_zone")]
    public class Id_zone
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "language")]
    public class Language
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "name")]
    public class Name
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "country")]
    public class Country
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "id_zone")]
        public Id_zone Id_zone { get; set; }
        [XmlElement(ElementName = "id_currency")]
        public string Id_currency { get; set; }
        [XmlElement(ElementName = "call_prefix")]
        public string Call_prefix { get; set; }
        [XmlElement(ElementName = "iso_code")]
        public string Iso_code { get; set; }
        [XmlElement(ElementName = "active")]
        public string Active { get; set; }
        [XmlElement(ElementName = "contains_states")]
        public string Contains_states { get; set; }
        [XmlElement(ElementName = "need_identification_number")]
        public string Need_identification_number { get; set; }
        [XmlElement(ElementName = "need_zip_code")]
        public string Need_zip_code { get; set; }
        [XmlElement(ElementName = "zip_code_format")]
        public string Zip_code_format { get; set; }
        [XmlElement(ElementName = "display_tax_label")]
        public string Display_tax_label { get; set; }
        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }
    }

    [XmlRoot(ElementName = "countries")]
    public class Countries
    {
        [XmlElement(ElementName = "country")]
        public List<Country> Country { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class PrestaShopCountry
    {
        [XmlElement(ElementName = "countries")]
        public Countries Countries { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }

}

