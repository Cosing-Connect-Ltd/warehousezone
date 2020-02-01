using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Warehouse.PropertiesSync.SyncData.Entities
{
    public class PTenant : ContactInfo
    {
        [Key]
        public int PTenantId { get; set; }
        public string TenantCode { get; set; }
        public string TenantYCode { get; set; }
        public string TenantFullName { get; set; }
        public string TenantSalutation { get; set; }
        public string TenancyStatus { get; set; }
        public int? TenancyCategory { get; set; }
        public DateTime? TenancyAdded { get; set; }
        public DateTime? TenancyStarted { get; set; }
        public DateTime? TenancyRenewDate { get; set; }
        public DateTime? TenancyVacateDate { get; set; }
        public double? TenancyPeriodMonths { get; set; }
        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }


        public string CurrentPropertyCode { get; set; }
        public virtual PProperty CurrentProperty { get; set; }

        public bool IsCurrentTenant { get; set; }
        public bool IsFutureTenant { get; set; }
        public bool IsHeadTenant { get; set; }
    }
}