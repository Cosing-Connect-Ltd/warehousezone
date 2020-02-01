using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class MarketCustomer : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Customer { get; set; }
        public int SortOrder { get; set; }
        public bool IsSkippable { get; set; }
        public DateTime? SkipFromDate { get; set; }
        public DateTime? SkipToDate { get; set; }
        public MarketCustomerVisitFrequency VisitFrequency { get; set; }
        public int MarketId { get; set; }
        [ForeignKey("MarketId")]
        public virtual Market Market { get; set; }
    }

    public class Market : PersistableEntity<int>
    {
        public Market()
        {
            MarketCustomers = new HashSet<MarketCustomer>();
            ProductMarketStockLevels = new HashSet<ProductMarketStockLevel>();
            MarketRouteMap = new HashSet<MarketRouteMap>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Town { get; set; }

        public int? ExternalId { get; set; }
        public virtual ICollection<MarketRouteMap> MarketRouteMap { get; set; }
        public virtual ICollection<MarketCustomer> MarketCustomers { get; set; }
        public virtual ICollection<ProductMarketStockLevel> ProductMarketStockLevels { get; set; }
    }

    public class MarketRoute : PersistableEntity<int>
    {
        public MarketRoute()
        {
            MarketRouteMap = new HashSet<MarketRouteMap>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Duration (Minutes)")]
        public int? RouteDurationMins { get; set; }
        public virtual ICollection<MarketRouteMap> MarketRouteMap { get; set; }
    }

    public class MarketRouteMap
    {
        [Key]
        public int Id { get; set; }
        public int MarketId { get; set; }
        public int MarketRouteId { get; set; }
        public int SortOrder { get; set; }
        [ForeignKey("MarketId")]
        public virtual Market Market { get; set; }
        [ForeignKey("MarketRouteId")]
        public virtual MarketRoute MarketRoute { get; set; }
    }


    public class MarketRouteSchedule
    {
        [Key]
        public int MarketRouteScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseIDs { get; set; }
        public int? TenentId { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelReason { get; set; }
        public int? VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual MarketVehicle MarketVehicle { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantLocations { get; set; }

        public virtual int RouteId { get; set; }
        [ForeignKey("RouteId")]
        public virtual MarketRoute MarketRoute { get; set; }
    }


    [Table("ProductMarketStockLevel")]
    public class ProductMarketStockLevel : PersistableEntity<int>
    {
        [Key]
        public int ProductMarketStockLevelID { get; set; }

        public int ProductMasterID { get; set; }

        [ForeignKey("ProductMasterID")]
        public virtual ProductMaster Product { get; set; }

        public int MarketId { get; set; }
        [ForeignKey("MarketId")]
        public virtual Market Market { get; set; }

        public decimal MinStockQuantity { get; set; }

    }

    [Serializable]
    public class MarketVehicle : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name = "Vehicle Registration")]
        public string VehicleIdentifier { get; set; }
        public virtual ICollection<TenantLocations> TenantLocations { get; set; }
    }

    public class MarketJobStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MarketJob : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public int? MarketRouteId { get; set; }
        [ForeignKey("MarketRouteId")]
        public virtual MarketRoute MarketRoute { get; set; }
        public int? MarketId { get; set; }
        [ForeignKey("MarketId")]
        public virtual Market Market { get; set; }
        public int? LatestJobStatusId { get; set; }
        public int? LatestJobAllocationId { get; set; }

    }

    [Table("MarketJobAllocation")]
    public class MarketJobAllocation : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public virtual Resources Resource { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string Reason { get; set; }
        public DateTime? ActionDate { get; set; }
        public string DeviceSerial { get; set; }
        public int MarketJobId { get; set; }
        [ForeignKey("MarketJobId")]
        public virtual MarketJob MarketJob { get; set; }
        public int MarketJobStatusId { get; set; }
        [ForeignKey("MarketJobStatusId")]
        public virtual MarketJobStatus MarketJobStatus { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}