using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class CheckoutViewModel
    {

        public CheckoutViewModel()
        {
            Addresses = new List<AddressViewModel>();
            CartItems = new List<WebsiteCartItemViewModel>();
            ShippingRules = new List<WebsiteShippingRulesViewModel>();
            StepsHistory = new List<CheckoutStep>();
            SagePayPaymentResponse=new SagePayPaymentResponseViewModel();
        }

        public void SetInitialStep(DeliveryMethod deliveryMethod)
        {
            CurrentStep = deliveryMethod == DeliveryMethod.ToPickupPoint ? CheckoutStep.CollectionPoint : CheckoutStep.ShippingAddress;
        }
        public int? AccountId { get; set; }
        public int? AccountAddressId { get; set; }
        public bool? CreateAccount { get; set; }
        public List<CountryViewModel> Countries { get; set; } = new List<CountryViewModel>();
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public bool? IsAddressSameForBilling { get; set; }
        public int? ShipmentRuleId { get; set; }
        public int? PaymentMethodId { get; set; }
        public bool? noTrackStep { get; set; }
        public CheckoutStep? CurrentStep { get; set; }
        public List<CheckoutStep> StepsHistory { get; set; }
        public int? DeliveryMethodId { get; set; }
        public int? CollectionPointId { get; set; }
        public string OrderNumber { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }

        public string PaypalClientId { get; set; }

        public decimal TotalOrderAmount => Math.Round((CartItems?.Sum(u => u.ProductTotalAmount) ?? 0) + (ShippingRule?.Price ?? 0), 2);
        public List<AddressViewModel> Addresses { get; set; }
        public AddressViewModel AccountAddress { get; set; }
        public List<WebsiteCartItemViewModel> CartItems { get; set; }
        public decimal CartTotalAmount => Math.Round(CartItems.Sum(c => c.ProductTotalAmount), 2) + (ShippingRule?.Price ?? 0);
        public List<WebsiteShippingRulesViewModel> ShippingRules { get; set; }
        public WebsiteShippingRulesViewModel ShippingRule { get; set; }
        public CollectionPointViewModel CollectionPoint { get; set; }
        public SagePayPaymentResponseViewModel SagePayPaymentResponse { get; set; }
    }

    public class ReviewOrderViewModel
    {
        public WebsiteCartItemsViewModel Cart { get; set; } = new WebsiteCartItemsViewModel();        
        public decimal CartTotalAmount => Math.Round(Cart.WebsiteCartItems.Sum(c => c.ProductTotalAmount), 2) + (ShippingRule?.Price ?? 0);
        public WebsiteShippingRulesViewModel ShippingRule { get; set; }
        public List<ProductMaster> RelatedProducts { get; set; } = new List<ProductMaster>();
        public bool IsStandardDelivery { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
    }

    public class AddressFormViewModel : AddressViewModel
    {
        public int SelectedAddressID { get; set; }
        public List<CountryViewModel> Countries { get; set; } = new List<CountryViewModel>();
        public List<AddressViewModel> SavedAddresses { get; set; }
        public string DeliveryInstructions { get; set; }
    }
}