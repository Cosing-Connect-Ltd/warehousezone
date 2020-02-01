using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PTenant : PContactInfo
    {
        [Key]
        public int PTenantId { get; set; }
        [Display(Name ="Tenant Code")]
        [Required]
        public string TenantCode { get; set; }
        public string TenantYCode { get; set; }
        [Required]
        [Display(Name = "Tenant Full Name")]
        public string TenantFullName { get; set; }
        [Display(Name = "Tenant Salutation")]
        public string TenantSalutation { get; set; }
        [Display(Name = "Tenancy Status")]
        public string TenancyStatus { get; set; }
        [Display(Name = "Tenancy Category")]
        public int? TenancyCategory { get; set; }
        [Display(Name = "Tenancy Added")]
        public DateTime? TenancyAdded { get; set; }
        [Display(Name = "Tenant Salutation")]
        public DateTime? TenancyStarted { get; set; }
        [Display(Name = "Tenancy Renew Date")]
        public DateTime? TenancyRenewDate { get; set; }
        [Display(Name = "Tenancy Vacate Date")]
        public DateTime? TenancyVacateDate { get; set; }
        [Display(Name = "Tenancy Period Months")]
        public double? TenancyPeriodMonths { get; set; }

        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }
        [Display(Name = "Property Code")]
        public string CurrentPropertyCode { get; set; }

        [ForeignKey("CurrentPropertyId")]
        public virtual PProperty CurrentProperty { get; set; }

        [Display(Name = "Property")]
        public int? CurrentPropertyId { get; set; }

        public bool IsCurrentTenant { get; set; }
        public bool IsFutureTenant { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedUserId { get; set; }

        public bool IsHeadTenant { get; set; }

        public string CurrentAddress {
            get
            {
                if (CurrentProperty != null)
                {
                    return FullAddress();
                }
                return string.Empty;
            }
        }

        public string IdWithEmail {
            get { return PTenantId + (string.IsNullOrEmpty(Email) ? "" : "," + Email); }
        }
    }
}