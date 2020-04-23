using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Tenant
    {
        public Tenant()
        {
            TenantLocations = new HashSet<TenantLocations>();
            AuthUsers = new HashSet<AuthUser>();
            TenantEmailConfig = new HashSet<TenantEmailConfig>();
            TenantModules = new HashSet<TenantModules>();
            TenantWebsites = new HashSet<TenantWebsites>();

        }

        [Key]
        [Display(Name = "Client Id")]
        public int TenantId { get; set; }
        [MaxLength(100)]
        [Display(Name = "Client Name")]
        public string TenantName { get; set; }
        [Display(Name = "Client No")]
        public string TenantNo { get; set; }
        [MaxLength(50)]
        [Display(Name = "Client VAT No")]
        public string TenantVatNo { get; set; }
        [MaxLength(50)]
        [Display(Name = "Client Account Reference")]
        public string TenantAccountReference { get; set; }
        [MaxLength(250)]
        [Display(Name = "Client Website")]
        public string TenantWebsite { get; set; }
        [MaxLength(50)]
        [Display(Name = "Day Phone")]
        public string TenantDayPhone { get; set; }
        [MaxLength(50)]
        [Display(Name = "Evening Phone")]
        public string TenantEveningPhone { get; set; }
        [MaxLength(50)]
        [Display(Name = "Client Mobile No")]
        public string TenantMobilePhone { get; set; }
        [MaxLength(50)]
        [Display(Name = "Client Fax No")]
        public string TenantFax { get; set; }
        [MaxLength(200)]
        [Display(Name = "Email")]
        public string TenantEmail { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 1")]
        public string TenantAddress1 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 2")]
        public string TenantAddress2 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 3")]
        public string TenantAddress3 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 4")]
        public string TenantAddress4 { get; set; }
        [MaxLength(200)]
        [Display(Name = "City")]
        public string TenantCity { get; set; }
        [MaxLength(200)]
        [Display(Name = "county / state")]
        public string TenantStateCounty { get; set; }
        [MaxLength(50)]
        [Display(Name = "Postal Code")]
        public string TenantPostalCode { get; set; }


        [Display(Name = "Base Currency")]
        public int CurrencyID { get; set; }
        [MaxLength(50)]
        [Display(Name = "Sub Domain")]
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
        public string AccountNumber { get; set; }
        public string ProductCodePrefix { get; set; }
        [StringLength(10)]
        public string TenantCulture { get; set; }
        public string TenantTimeZoneId { get; set; }

        [StringLength(50)]
        public string TenantRegNo { get; set; }
        public WarehouseThemeEnum Theme { get; set; }
        public virtual ICollection<TenantLocations> TenantLocations { get; set; }
        public virtual ICollection<AuthUser> AuthUsers { get; set; }
        public virtual ICollection<TenantEmailConfig> TenantEmailConfig { get; set; }
        public virtual ICollection<TenantModules> TenantModules { get; set; }
        [ForeignKey("CountryID")]
        public virtual GlobalCountry Country { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual GlobalCurrency Currency { get; set; }
        public virtual ICollection<TenantWebsites> TenantWebsites { get; set; }
    }
}