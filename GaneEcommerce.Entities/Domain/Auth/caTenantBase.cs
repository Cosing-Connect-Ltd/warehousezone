using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class caTenantBase
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantNo { get; set; }
        public string TenantVatNo { get; set; }
        public string TenantAccountReference { get; set; }
        public string TenantWebsite { get; set; }
        public string TenantDayPhone { get; set; }
        public string TenantEveningPhone { get; set; }
        public string TenantMobilePhone { get; set; }
        public string TenantFax { get; set; }
        public string TenantEmail { get; set; }
        public string TenantAddress1 { get; set; }
        public string TenantAddress2 { get; set; }
        public string TenantAddress3 { get; set; }
        public string TenantCity { get; set; }
        public string TenantStateCounty { get; set; }
        public string TenantPostalCode { get; set; }
        public string TenantSubDmoain { get; set; }
        public System.DateTime? DateCreated { get; set; }
        public System.DateTime? DateUpdated { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? CountryID { get; set; }
        public bool PalletingEnabled { get; set; }
        public string ProductCodePrefix { get; set; }
        public Boolean AuthStatus { get; set; }
        public string TenantCulture { get; set; }
        public string TenantTimeZoneId { get; set; }
        public WarehouseThemeEnum Theme { get; set; }
        public ICollection<TenantLocations> TenantLocations { get; set; }
        public virtual ICollection<TenantModules> TenantModules { get; set; }
    }

    [Serializable]
    public class VechilesBase
    {
        public int vechileId { get; set; }
    }
}