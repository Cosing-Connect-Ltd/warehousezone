using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ganedata.Core.Entities.Domain;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class PTenantViewModelForTenantFlag
    {
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
        public int? CurrentPropertyId { get; set; }
        public bool IsCurrentTenant { get; set; }
        public bool IsFutureTenant { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedUserId { get; set; }
        public bool IsHeadTenant { get; set; }

    }
}