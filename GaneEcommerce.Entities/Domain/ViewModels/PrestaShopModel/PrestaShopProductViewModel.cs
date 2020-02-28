using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int product_attribute_id { get; set; }
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
        public int id_product_attribute { get; set; }
        public int quantity { get; set; }
        public int depends_on_stock { get; set; }
        public int out_of_stock { get; set; }

        public int StockAvailableId { get; set; }
    }







}