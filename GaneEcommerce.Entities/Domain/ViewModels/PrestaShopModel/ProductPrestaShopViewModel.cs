using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    [XmlRoot(ElementName = "id_category_default")]
    public class Id_category_default
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "id_default_image")]
    public class Id_default_image
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "id_default_combination")]
    public class Id_default_combination
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
    }

    [XmlRoot(ElementName = "id_tax_rules_group")]
    public class Id_tax_rules_group
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "position_in_category")]
    public class Position_in_category
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "manufacturer_name")]
    public class Manufacturer_name
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
    }

    [XmlRoot(ElementName = "quantity")]
    public class Quantity
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "type")]
    public class Type
    {
        [XmlAttribute(AttributeName = "notFilterable")]
        public string NotFilterable { get; set; }
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

    [XmlRoot(ElementName = "delivery_in_stock")]
    public class Delivery_in_stock
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "delivery_out_stock")]
    public class Delivery_out_stock
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "meta_description")]
    public class Meta_description
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "meta_keywords")]
    public class Meta_keywords
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "meta_title")]
    public class Meta_title
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "link_rewrite")]
    public class Link_rewrite
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "name")]
    public class Name
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "description")]
    public class Description
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "description_short")]
    public class Description_short
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "available_now")]
    public class Available_now
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "available_later")]
    public class Available_later
    {
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
    }

    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "categories")]
    public class Categories
    {
        [XmlElement(ElementName = "category")]
        public List<Category> Category { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "images")]
    public class Images
    {
        [XmlElement(ElementName = "image")]
        public Image Image { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "combinations")]
    public class Combinations
    {
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "product_option_values")]
    public class Product_option_values
    {
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "id_feature_value")]
    public class Id_feature_value
    {
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "product_feature")]
    public class Product_feature
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "id_feature_value")]
        public Id_feature_value Id_feature_value { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "product_features")]
    public class Product_features
    {
        [XmlElement(ElementName = "product_feature")]
        public Product_feature Product_feature { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "tags")]
    public class Tags
    {
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
        [XmlElement(ElementName = "tag")]
        public List<Tag> Tag { get; set; }
    }

    [XmlRoot(ElementName = "stock_available")]
    public class Stock_available
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "id_product_attribute")]
        public int Id_product_attribute { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "stock_availables")]
    public class Stock_availables
    {
        [XmlElement(ElementName = "stock_available")]
        public Stock_available Stock_available { get; set; }
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "accessories")]
    public class Accessories
    {
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "product_bundle")]
    public class Product_bundle
    {
        [XmlAttribute(AttributeName = "nodeType")]
        public string NodeType { get; set; }
        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }
    }

    [XmlRoot(ElementName = "associations")]
    public class Associations
    {
        [XmlElement(ElementName = "categories")]
        public Categories Categories { get; set; }
        [XmlElement(ElementName = "images")]
        public Images Images { get; set; }
        [XmlElement(ElementName = "combinations")]
        public Combinations Combinations { get; set; }
        [XmlElement(ElementName = "product_option_values")]
        public Product_option_values Product_option_values { get; set; }
        [XmlElement(ElementName = "product_features")]
        public Product_features Product_features { get; set; }
        [XmlElement(ElementName = "tags")]
        public Tags Tags { get; set; }
        [XmlElement(ElementName = "stock_availables")]
        public Stock_availables Stock_availables { get; set; }
        [XmlElement(ElementName = "accessories")]
        public Accessories Accessories { get; set; }
        [XmlElement(ElementName = "product_bundle")]
        public Product_bundle Product_bundle { get; set; }
    }

    [XmlRoot(ElementName = "product")]
    public class Product
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "id_manufacturer")]
        public string Id_manufacturer { get; set; }
        [XmlElement(ElementName = "id_supplier")]
        public string Id_supplier { get; set; }
        [XmlElement(ElementName = "id_category_default")]
        public Id_category_default Id_category_default { get; set; }
        [XmlElement(ElementName = "new")]
        public string New { get; set; }
        [XmlElement(ElementName = "cache_default_attribute")]
        public string Cache_default_attribute { get; set; }
        [XmlElement(ElementName = "id_default_image")]
        public Id_default_image Id_default_image { get; set; }
        [XmlElement(ElementName = "id_default_combination")]
        public Id_default_combination Id_default_combination { get; set; }
        [XmlElement(ElementName = "id_tax_rules_group")]
        public Id_tax_rules_group Id_tax_rules_group { get; set; }
        [XmlElement(ElementName = "position_in_category")]
        public Position_in_category Position_in_category { get; set; }
        [XmlElement(ElementName = "manufacturer_name")]
        public Manufacturer_name Manufacturer_name { get; set; }
        [XmlElement(ElementName = "quantity")]
        public Quantity Quantity { get; set; }
        [XmlElement(ElementName = "type")]
        public Type Type { get; set; }
        [XmlElement(ElementName = "id_shop_default")]
        public string Id_shop_default { get; set; }
        [XmlElement(ElementName = "reference")]
        public string Reference { get; set; }
        [XmlElement(ElementName = "supplier_reference")]
        public string Supplier_reference { get; set; }
        [XmlElement(ElementName = "location")]
        public string Location { get; set; }
        [XmlElement(ElementName = "width")]
        public double? Width { get; set; }
        [XmlElement(ElementName = "height")]
        public double? Height { get; set; }
        [XmlElement(ElementName = "depth")]
        public double? Depth { get; set; }
        [XmlElement(ElementName = "weight")]
        public double? Weight { get; set; }
        [XmlElement(ElementName = "quantity_discount")]
        public string Quantity_discount { get; set; }
        [XmlElement(ElementName = "ean13")]
        public string Ean13 { get; set; }
        [XmlElement(ElementName = "isbn")]
        public string Isbn { get; set; }
        [XmlElement(ElementName = "upc")]
        public string Upc { get; set; }
        [XmlElement(ElementName = "cache_is_pack")]
        public string Cache_is_pack { get; set; }
        [XmlElement(ElementName = "cache_has_attachments")]
        public string Cache_has_attachments { get; set; }
        [XmlElement(ElementName = "is_virtual")]
        public string Is_virtual { get; set; }
        [XmlElement(ElementName = "state")]
        public string State { get; set; }
        [XmlElement(ElementName = "additional_delivery_times")]
        public string Additional_delivery_times { get; set; }
        [XmlElement(ElementName = "delivery_in_stock")]
        public Delivery_in_stock Delivery_in_stock { get; set; }
        [XmlElement(ElementName = "delivery_out_stock")]
        public Delivery_out_stock Delivery_out_stock { get; set; }
        [XmlElement(ElementName = "on_sale")]
        public string On_sale { get; set; }
        [XmlElement(ElementName = "online_only")]
        public string Online_only { get; set; }
        [XmlElement(ElementName = "ecotax")]
        public string Ecotax { get; set; }
        [XmlElement(ElementName = "minimal_quantity")]
        public string Minimal_quantity { get; set; }
        [XmlElement(ElementName = "low_stock_threshold")]
        public string Low_stock_threshold { get; set; }
        [XmlElement(ElementName = "low_stock_alert")]
        public string Low_stock_alert { get; set; }
        [XmlElement(ElementName = "price")]
        public decimal? Price { get; set; }
        [XmlElement(ElementName = "wholesale_price")]
        public string Wholesale_price { get; set; }
        [XmlElement(ElementName = "unity")]
        public string Unity { get; set; }
        [XmlElement(ElementName = "unit_price_ratio")]
        public string Unit_price_ratio { get; set; }
        [XmlElement(ElementName = "additional_shipping_cost")]
        public string Additional_shipping_cost { get; set; }
        [XmlElement(ElementName = "customizable")]
        public string Customizable { get; set; }
        [XmlElement(ElementName = "text_fields")]
        public string Text_fields { get; set; }
        [XmlElement(ElementName = "uploadable_files")]
        public string Uploadable_files { get; set; }
        [XmlElement(ElementName = "active")]
        public string Active { get; set; }
        [XmlElement(ElementName = "redirect_type")]
        public string Redirect_type { get; set; }
        [XmlElement(ElementName = "id_type_redirected")]
        public string Id_type_redirected { get; set; }
        [XmlElement(ElementName = "available_for_order")]
        public string Available_for_order { get; set; }
        [XmlElement(ElementName = "available_date")]
        public string Available_date { get; set; }
        [XmlElement(ElementName = "show_condition")]
        public string Show_condition { get; set; }
        [XmlElement(ElementName = "condition")]
        public string Condition { get; set; }
        [XmlElement(ElementName = "show_price")]
        public string Show_price { get; set; }
        [XmlElement(ElementName = "indexed")]
        public string Indexed { get; set; }
        [XmlElement(ElementName = "visibility")]
        public string Visibility { get; set; }
        [XmlElement(ElementName = "advanced_stock_management")]
        public string Advanced_stock_management { get; set; }
        [XmlElement(ElementName = "date_add")]
        public string Date_add { get; set; }
        [XmlElement(ElementName = "date_upd")]
        public string Date_upd { get; set; }
        [XmlElement(ElementName = "pack_stock_type")]
        public string Pack_stock_type { get; set; }
        [XmlElement(ElementName = "meta_description")]
        public Meta_description Meta_description { get; set; }
        [XmlElement(ElementName = "meta_keywords")]
        public Meta_keywords Meta_keywords { get; set; }
        [XmlElement(ElementName = "meta_title")]
        public Meta_title Meta_title { get; set; }
        [XmlElement(ElementName = "link_rewrite")]
        public Link_rewrite Link_rewrite { get; set; }
        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "description")]
        public Description Description { get; set; }
        [XmlElement(ElementName = "description_short")]
        public Description_short Description_short { get; set; }
        [XmlElement(ElementName = "available_now")]
        public Available_now Available_now { get; set; }
        [XmlElement(ElementName = "available_later")]
        public Available_later Available_later { get; set; }
        [XmlElement(ElementName = "associations")]
        public Associations Associations { get; set; }
    }

    [XmlRoot(ElementName = "tag")]
    public class Tag
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "products")]
    public class Products
    {
        [XmlElement(ElementName = "product")]
        public List<Product> Product { get; set; }
    }

    [XmlRoot(ElementName = "prestashop")]
    public class PrestashopProduct
    {
        [XmlElement(ElementName = "products")]
        public Products Products { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
    }

}