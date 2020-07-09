using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class CheckoutViewModel
    {

        public CheckoutViewModel()
        {
            Addresses = new List<AddressViewModel>();
            CartItems = new List<OrderDetailSessionViewModel>();
            ShippingRules = new List<WebsiteShippingRulesViewModel>();
            CurrentStep = (int)CheckoutStep.BillingAddress;
            ParentStep = (int)CheckoutStep.BillingAddress;
            SagePayPaymentResponse=new SagePayPaymentResponseViewModel();
        }

        public int? AccountId { get; set; }
        public int? AccountAddressId { get; set; }
        public List<CountryViewModel> Countries { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShipmentRuleId { get; set; }

        public int? PaymentMethodId { get; set; }
        public CheckoutStep? CurrentStep { get; set; }
        public int? DeliveryMethodId { get; set; }
        public int? CollectionPointId { get; set; }

        public string CurrencySymbol { get; set; }

        public string OrderNumber { get; set; }

        public decimal TotalOrderAmount => Math.Round((CartItems?.Sum(u => u.ProductTotalAmount) ?? 0) + (ShippingRule?.Price ?? 0), 2);

        public CheckoutStep? ParentStep { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
        public AddressViewModel AccountAddress { get; set; }

        public List<OrderDetailSessionViewModel> CartItems { get; set; }

        public List<WebsiteShippingRulesViewModel> ShippingRules { get; set; }
        public WebsiteShippingRulesViewModel ShippingRule { get; set; }
        public CollectionPointViewModel CollectionPoint { get; set; }

        public SagePayPaymentResponseViewModel SagePayPaymentResponse { get; set; }




    }
    [Serializable]
    public class CountryViewModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }
    }
    [Serializable]
    public class WebsiteShippingRulesViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        [Display(Name = "Courier")]
        public string Courier { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        [Display(Name = "Postal Area")]
        public string PostalArea { get; set; }
        [Display(Name = "Weight in Grams")]
        public int WeightinGrams { get; set; }
        public decimal Price { get; set; }
    }
    [Serializable]
    public class CollectionPointViewModel
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int CountryID { get; set; }
        public virtual CountryViewModel GlobalCountry { get; set; }

    }
}