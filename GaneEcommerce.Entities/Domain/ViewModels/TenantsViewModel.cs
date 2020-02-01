using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class TenantsViewModel 
    {
        [Display(Name = "Client Id")]
        public int TenantId { get; set; }

        [StringLength(100, ErrorMessage = "Client Name maximum {1} characters exceeded")]
        [Display(Name = "Client Name"), Required(ErrorMessage = "Client Name is required.")]
        public string TenantName { get; set; }

        [Display(Name = "Client No")]
        public string TenantNo { get; set; }
        [StringLength(50, ErrorMessage = "Client VAT No maximum {1} characters exceeded")]
        [Display(Name = "Client VAT No")]
        public string TenantVatNo { get; set; }
        [StringLength(50, ErrorMessage = "Client Account Reference maximum {1} characters exceeded")]
        [Display(Name = "Client Account Reference")]
        public string TenantAccountReference { get; set; }
        [StringLength(250, ErrorMessage = "Client Website maximum {1} characters exceeded")]
        [Display(Name = "Client Website")]
        public string TenantWebsite { get; set; }
        [StringLength(50, ErrorMessage = "Day Phone maximum {1} characters exceeded")]
        [Display(Name = "Day Phone")]
        public string TenantDayPhone { get; set; }
        [StringLength(50, ErrorMessage = "Evening Phone maximum {1} characters exceeded")]
        [Display(Name = "Evening Phone")]
        public string TenantEveningPhone { get; set; }
        [StringLength(50, ErrorMessage = "Client Mobile No maximum {1} characters exceeded")]
        [Display(Name = "Client Mobile No")]
        public string TenantMobilePhone { get; set; }
        [StringLength(50, ErrorMessage = "Client Fax No maximum {1} characters exceeded")]
        [Display(Name = "Client Fax No")]
        public string TenantFax { get; set; }
        [StringLength(200, ErrorMessage = "Email maximum {1} characters exceeded")]
        [Display(Name = "Email"), EmailAddress(ErrorMessage = "Email is not valid.")]
        public string TenantEmail { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 1 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 1"), Required(ErrorMessage = "Address Line 1 is required.")]
        public string TenantAddress1 { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 2 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 2")]
        public string TenantAddress2 { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 3 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 3")]
        public string TenantAddress3 { get; set; }
        [StringLength(200, ErrorMessage = "Address Line 4 maximum {1} characters exceeded")]
        [Display(Name = "Address Line 4")]
        public string TenantAddress4 { get; set; }
        [StringLength(200, ErrorMessage = "City maximum {1} characters exceeded")]
        [Display(Name = "City")]
        public string TenantCity { get; set; }
        [StringLength(200, ErrorMessage = "County / State maximum {1} characters exceeded")]
        [Display(Name = "County / State")]
        public string TenantStateCounty { get; set; }
        [StringLength(50, ErrorMessage = "Postal Code maximum {1} characters exceeded")]
        [Display(Name = "Postal Code")]
        public string TenantPostalCode { get; set; }
        [Display(Name = "Base Currency"), Range(0, int.MaxValue, ErrorMessage = "Base Currency must be an integer.")]
        public int CurrencyID { get; set; }
        [StringLength(50, ErrorMessage = "Sub Domain in WMS maximum {1} characters exceeded")]
        [Display(Name = "Sub Domain in WMS")]
        public string TenantSubDmoain { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime? DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Country")]
        public int? CountryID { get; set; }

        public int Id { get; set; }
        [DisplayName("Tenant Name")]
        public string Name { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        public ICollection<TenantLocations> TenantLocations { get; set; }
    }
}