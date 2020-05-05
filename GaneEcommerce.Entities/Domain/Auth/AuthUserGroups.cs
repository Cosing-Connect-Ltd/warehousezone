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
        public bool AllowEnquiries { get; set; }
        public bool AllowBaskets { get; set; }
        public bool AllowCheckout { get; set; }
        public bool DefaultForWebRegistration { get; set; }
        public bool SuperUserAccess { get; set; }
        public int MaxProductsPerOrder { get; set; }
        public decimal ShippingMargin { get; set; }
        public decimal ProductMarkup { get; set; }
        public bool AllowPriceBreaks { get; set; }
        public string BillingCompanyName { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressLine2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostCode { get; set; }
        public int? CountryId { get; set; }
        // sage or SAP account code
        public string AccountNumber { get; set; }
        public bool AllowSagePay { get; set; }
        public bool AllowPayPal { get; set; }
        public bool AllowInvoice { get; set; }
        public bool AllowProformaInvoice { get; set; }
        public bool AllowFoC { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("CountryId")]
        public virtual GlobalCountry GlobalCountry { get; set; }
        public virtual ICollection<AuthUser> AuthUser { get; set; }
    }
}
