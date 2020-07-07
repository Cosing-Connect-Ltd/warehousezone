using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
            CurrentStep = (int) CheckoutStep.BillingAddress;
            ParentStep= (int)CheckoutStep.BillingAddress;
        }

        public int? AccountId { get; set; }
        public int? AccountAddressId { get; set; }
        public List<CountryViewModel> Countries { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShipmentRuleId { get; set; }
        public CheckoutStep? CurrentStep { get; set; }
        public int? DeliveryMethodId { get; set; }
        public int? CollectionPointId { get; set; }

        public string CurrencySymbol { get; set; }

        public CheckoutStep? ParentStep { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
        public AddressViewModel AccountAddress { get; set; }

        public List<OrderDetailSessionViewModel> CartItems { get; set; }

        public List<WebsiteShippingRulesViewModel> ShippingRules { get; set; }





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
}