using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{

    public class TenantLocationsSyncCollection
    {
        public TenantLocationsSyncCollection()
        {
            TenantLocationSync = new List<TenantLocationsSync>();
        }
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<TenantLocationsSync> TenantLocationSync { get; set; }
    }
    public class TenantLocationsSync
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string CountyState { get; set; }
        public string PostalCode { get; set; }
        public int CountryID { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public int? AddressId { get; set; }
        public int? ContactNumbersId { get; set; }
        public int MinimumDrivers { get; set; }
        public int MinimumKitchenStaff { get; set; }
        public int MinimumGeneralStaff { get; set; }
        public bool? IsMobile { get; set; }
        public bool? AutoTransferOrders { get; set; }
        public bool? MonitorStockVariance { get; set; }
        public virtual int? MarketVehicleID { get; set; }
        public int? ParentWarehouseId { get; set; }
        public int? SalesTerminalId { get; set; }
        public int? SalesManUserId { get; set; }
        public PalletTrackingSchemeEnum PalletTrackingScheme { get; set; }
        public bool EnableGlobalProcessByPallet { get; set; }
        public bool AutoAllowProcess { get; set; }
        public bool AllowStocktakeAddNew { get; set; }
        public bool AllowStocktakeEdit { get; set; }
        public bool ConsolidateOrderProcesses { get; set; }
        public bool AllowShipToAccountAddress { get; set; }
        public bool ShowTaxInBlindShipment { get; set; }
        public bool ShowPriceInBlindShipment { get; set; }
        public bool ShowQtyInBlindShipment { get; set; }
        public bool ShowFullBalanceOnPayment { get; set; }
        public bool AllowSaleWithoutAccount { get; set; }
        public bool ShowCaseQtyInReports { get; set; }
        public DateTime? StartDateofHolidaysYear { get; set; }
        public bool ShowDepartmentInBlindShipment { get; set; }
        public bool ShowGroupInBlindShipment { get; set; }
        public bool ShowPriceAlertInSalesOrder { get; set; }
        public bool PickByContainer { get; set; }
        public bool MandatoryPickByContainer { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal? DeliveryMinimumOrderValue { get; set; }
        public decimal? DeliveryMinimumOrderValueForFree { get; set; }
        public decimal CollectionCharges { get; set; }
        public decimal EatInCharges { get; set; }
        public decimal DeliveryRadiusMiles { get; set; }
        public int? DeliveryPriceGroupId { get; set; }
        public int? CollectionPriceGroupId { get; set; }
        public int? EatInPriceGroupId { get; set; }
        public int? DefaultDeliveryTimeMinutes { get; set; }
        public int? DefaultCollectionTimeMinutes { get; set; }
        public int? DefaultEatInTimeMinutes { get; set; }
        public int TenantId { get; set; }
        public bool? IsDeleted { get; set; }
        public string TelephoneNumber1 { get; set; }
        public string TelephoneNumber2 { get; set; }

        public bool? LoyaltyDeliveryOrdersEnabled { get; set; }
        public bool? LoyaltyCollectionOrdersEnabled { get; set; }
        public bool? LoyaltyEatInOrdersEnabled { get; set; }

        public WarehouseOpeningTimeViewModel OpeningHours { get; set; }
    }

    public class WarehouseOpeningTimeViewModel
    {
        public int WarehouseId { get; set; }
        public WarehouseOpeningHoursViewModel SundayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel MondayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel TuesdayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel WednesdayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel ThursdayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel FridayOpeningHoursViewModel { get; set; }
        public WarehouseOpeningHoursViewModel SaturdayOpeningHoursViewModel { get; set; }
    }

    public class WarehouseOpeningHoursViewModel
    {
        public int OpeningTimeHour { get; set; }
        public int OpeningTimeMins { get; set; }
        public int ClosingTimeHour { get; set; }
        public int ClosingTimeMins { get; set; }
        public bool IsClosed { get; set; }
    }

}