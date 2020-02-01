using System;
using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Models
{
    public class MarketVehiclesSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<MarketVehiclesSync> Vehicles { get; set; }
    }

    public class MarketVehiclesSync : MarketVehicleViewModel
    {

    }

    public class TerminalMetadataSync
    {
        public string Serial { get; set; }
        public string TerminalName { get; set; }
        public int TerminalId { get; set; }
        public int ParentWarehouseId { get; set; }
        public string ParentWarehouseName { get; set; }
        public int? MobileWarehouseId { get; set; }
        public string MobileWarehouseName { get; set; }
        public int? MarketVehicleId { get; set; }
        public string VehicleRegistration { get; set; }
        public int? SalesManUserId { get; set; }
        public string SalesManResourceName { get; set; }
        public int TenantId { get; set; }
        public PalletTrackingSchemeEnum PalletTrackingScheme { get; set; }
        public bool GlobalProcessByPallet { get; set; }
        public string TenantName { get; set; }
        public string TenantReceiptPrintHeaderLine1 { get; set; }
        public string TenantReceiptPrintHeaderLine2 { get; set; }
        public string TenantReceiptPrintHeaderLine3 { get; set; }
        public string TenantReceiptPrintHeaderLine4 { get; set; }
        public string TenantReceiptPrintHeaderLine5 { get; set; }
        public byte[] TenantLogo { get; set; }
        public bool PrintLogoForReceipts { get; set; }
        public short SessionTimeoutHours { get; set; }
        public bool AllowStocktakeAddNew { get; set; }
        public bool AllowStocktakeEdit { get; set; }
        public bool VehicleChecksAtStart { get; set; }
        public bool PostGeoLocation { get; set; }
        public bool ShowFullBalanceOnPayment { get; set; }
        public bool AllowSaleWithoutAccount { get; set; }
        public bool AllowExportDatabase { get; set; }
        public bool ShowCasePrices { get; set; }
        public bool ScanVehicleLicensePlate { get; set; }
    }


    public class VehiclesInspectionChecklistSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<VehicleInspectionChecklistSync> Vehicles { get; set; }
    }

    public class VehicleInspectionChecklistSync : InspectionCheckListViewModel
    {

    }

    public class VehicleInspectionReportSync : VehicleInspectionViewModel
    {
        public int CurrentUserId { get; set; }
        public Guid TransactionLogId { get; set; }
    }
}