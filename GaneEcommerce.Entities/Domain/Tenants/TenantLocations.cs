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
    public class TenantLocations : PersistableEntity<int>
    {
        public TenantLocations()
        {
            Devices = new HashSet<Terminals>();
            InventoryStocks = new HashSet<InventoryStock>();
            InventoryTransactions = new HashSet<InventoryTransaction>();
            StockTakes = new HashSet<StockTake>();
            ProductLocationStockLevels = new HashSet<ProductLocationStockLevel>();
        }

        [Key]
        [Display(Name = "Warehouse Id")]
        public int WarehouseId { get; set; }


        [MaxLength(200)]
        [Display(Name = "Location")]
        public string WarehouseName { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }
        [MaxLength(200)]
        [Display(Name = "Address Line 4")]
        public string AddressLine4 { get; set; }
        [MaxLength(200)]
        [Display(Name = "City")]
        public string City { get; set; }
        [MaxLength(200)]
        [Display(Name = "County / State")]
        public string CountyState { get; set; }
        [MaxLength(50)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Column(TypeName = "text")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public int? AddressId { get; set; }
        public int? ContactNumbersId { get; set; }
        public int MinimumDrivers { get; set; }
        public int MinimumKitchenStaff { get; set; }
        public int MinimumGeneralStaff { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenants { get; set; }
        public virtual Address Address { get; set; }
        public virtual ContactNumbers ContactNumbers { get; set; }
        public virtual ICollection<EmployeeShifts_Stores> EmployeeShifts_Stores { get; set; }
        public virtual ICollection<Terminals> Devices { get; set; }

        public virtual GlobalCountry GlobalCountry { get; set; }
        public virtual ICollection<Locations> Locations { get; set; }
        public virtual ICollection<OrderDetail> PODetails { get; set; }
        public virtual ICollection<InventoryStock> InventoryStocks { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<StockTake> StockTakes { get; set; }
        public virtual ICollection<ProductLocationStockLevel> ProductLocationStockLevels { get; set; }

        public bool? IsMobile { get; set; }
        [Display(Name = "Autotransfer Order")]
        public bool? AutoTransferOrders { get; set; }
        [Display(Name = "Monitor Stock Variance")]
        public bool? MonitorStockVariance { get; set; }

        public virtual int? MarketVehicleID { get; set; }
        [ForeignKey("MarketVehicleID")]
        public virtual MarketVehicle MarketVehicle { get; set; }

        [Display(Name = "Parent Warehouse")]
        public int? ParentWarehouseId { get; set; }
        [ForeignKey("ParentWarehouseId")]
        public virtual TenantLocations ParentWarehouse { get; set; }

        public int? SalesTerminalId { get; set; }
        [ForeignKey("SalesTerminalId")]
        public virtual Terminals SalesTerminal { get; set; }

        public int? SalesManUserId { get; set; }
        [ForeignKey("SalesManUserId")]
        public virtual AuthUser SalesManUser { get; set; }
        [Display(Name = "Pallet Tracking Scheme")]
        public PalletTrackingSchemeEnum PalletTrackingScheme { get; set; }
        [Display(Name = "Enable Process By Pallet")]
        public bool EnableGlobalProcessByPallet { get; set; }
        [Display(Name = "Auto Allow Process")]
        public bool AutoAllowProcess { get; set; }
        [Display(Name = "Allow Stocktake Add New")]
        public bool AllowStocktakeAddNew { get; set; }
        [Display(Name = "Allow Stocktake Edit")]
        public bool AllowStocktakeEdit { get; set; }
        [Display(Name = "Consolidate Order Processes")]
        public bool ConsolidateOrderProcesses { get; set; }
        [Display(Name = "Allow Ship To Account Address")]
        public bool AllowShipToAccountAddress { get; set; }
        [Display(Name = "Show Tax In BlindShipment")]
        public bool ShowTaxInBlindShipment { get; set; }
        [Display(Name = "Show Price In BlindShipment")]
        public bool ShowPriceInBlindShipment { get; set; }
        [Display(Name = "Show Qty In BlindShipment")]
        public bool ShowQtyInBlindShipment { get; set; }
        public bool ShowFullBalanceOnPayment { get; set; }
        public bool AllowSaleWithoutAccount { get; set; }
        public bool ShowCaseQtyInReports { get; set; }
        public DateTime? StartDateofHolidaysYear { get; set; }
        [Display(Name = "Show Department In BlindShipment")]
        public bool ShowDepartmentInBlindShipment { get; set; }
        [Display(Name = "Show Group In BlindShipment")]
        public bool ShowGroupInBlindShipment { get; set; }

        [Display(Name = "Show Price alerts in Sales Order")]
        public bool ShowPriceAlertInSalesOrder { get; set; }


    }
}