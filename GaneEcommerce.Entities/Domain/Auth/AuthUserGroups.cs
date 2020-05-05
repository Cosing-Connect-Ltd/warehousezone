using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class AuthUserGroups : PersistableEntity<int>
    {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Profile Value")]
        //comma delimated email addresses
        public string GroupAdministrators { get; set; }
        [Display(Name = "Allow Enquiries")]
        public bool AllowEnquiries { get; set; }
        [Display(Name = "Allow Baskets")]
        public bool AllowBaskets { get; set; }
        [Display(Name = "Allow Checkout")]
        public bool AllowCheckout { get; set; }
        [Display(Name = "Default For Web sRegistration")]
        public bool DefaultForWebRegistration { get; set; }
        [Display(Name = "Super User Access")]
        public bool SuperUserAccess { get; set; }
        [Display(Name = "Max Products Per Order")]
        public int MaxProductsPerOrder { get; set; }
        [Display(Name = "Shipping Margin")]
        public decimal ShippingMargin { get; set; }
        [Display(Name = "Product Markup")]
        public decimal ProductMarkup { get; set; }
        [Display(Name = "Allow Price Breaks")]
        public bool AllowPriceBreaks { get; set; }
        [Display(Name = "Billing Company Name")]
        public string BillingCompanyName { get; set; }
        [Display(Name = "Billing Address Line1")]
        public string BillingAddressLine1 { get; set; }
        [Display(Name = "Billing Address Line2")]
        public string BillingAddressLine2 { get; set; }
        
        public string City { get; set; }
        public string Region { get; set; }
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        // sage or SAP account code
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
        [Display(Name = "Allow Sage Pay")]
        public bool AllowSagePay { get; set; }
        [Display(Name = "Allow PayPal")]
        public bool AllowPayPal { get; set; }
        [Display(Name = "Allow Invoice")]
        public bool AllowInvoice { get; set; }
        [Display(Name = "Allow Proforma Invoice")]
        public bool AllowProformaInvoice { get; set; }
        [Display(Name = "Allow FoC")]
        public bool AllowFoC { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("CountryId")]
        public virtual GlobalCountry GlobalCountry { get; set; }
        public virtual ICollection<AuthUser> AuthUser { get; set; }
    }
}
