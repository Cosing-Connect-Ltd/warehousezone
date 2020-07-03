using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class CheckoutViewModel
    {

        public CheckoutViewModel()
        {
            Addresses = new List<AccountAddresses>();
            CartItems = new List<OrderDetailSessionViewModel>();
            ShippingRules = new List<WebsiteShippingRules>();
        }

        public int? AccountId { get; set; }
        public int? accountAddressId { get; set; }
        public List<GlobalCountry> Countries { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShipmentRuleId { get; set; }
        public CheckoutStep? CurrentStep { get; set; }
        public int? DeliveryMethodId { get; set; }
        public int? CollectionPointId { get; set; }

        public string CurrencySymbol { get; set; }

        public CheckoutStep? ParentStep { get; set; }
        public List<AccountAddresses> Addresses { get; set; }
        public AccountAddresses AccountAddress { get; set; }

        public List<OrderDetailSessionViewModel> CartItems { get; set; }

        public List<WebsiteShippingRules> ShippingRules { get; set; }





    }
}