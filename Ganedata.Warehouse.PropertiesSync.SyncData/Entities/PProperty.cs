using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Warehouse.PropertiesSync.SyncData.Entities
{
    public class PProperty
    {
        [Key]
        public int PPropertyId { get; set; }

        public string PropertyCode { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string AddressLine5 { get; set; }
        public string AddressPostcode { get; set; }
        public string PropertyStatus { get; set; }
        public bool IsVacant { get; set; }
        public DateTime? DateAvailable { get; set; }
        public DateTime? DateAdded { get; set; }
        public string PropertyBranch { get; set; }
        public double? TenancyMonths { get; set; }
        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }
        public DateTime? LetDate { get; set; }
        public int Order_OrderID { get; set; }
        public string CurrentLandlordCode { get; set; }
        public string CurrentTenantCode { get; set; }
        public virtual PLandlord PropertyLandlord { get; set; }
    }
}