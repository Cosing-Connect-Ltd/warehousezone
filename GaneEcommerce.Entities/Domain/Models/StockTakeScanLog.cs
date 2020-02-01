using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class StockTakeScanLog : PersistableEntity<int>
    {
        public StockTakeScanLog()
        {
            ResponseSuccess = true;
            DateCreated = DateTime.UtcNow;
        }

        [Key]
        public int StockTakeScanLogId { get; set; }

        public int RequestCurrentTenantId { get; set; }
        public int RequestWarehouseId { get; set; }
        public int RequestAuthUserId { get; set; }
        public int RequestStockTakeId { get; set; }
        public string RequestProductCode { get; set; }
        public string RequestProductSerial { get; set; }
        public string RequestPalletSerial { get; set; }
        public string ResponseProductName { get; set; }
        public string ResponseProductDescription { get; set; }
        public decimal ResponseInStock { get; set; }
        public decimal ResponseAllocated { get; set; }
        public decimal ResponseAvailable { get; set; }
        public bool ResponseSerialRequired { get; set; }

        public bool ResponseSuccess { get; set; }
        public string ResponseFailureMessage { get; set; }
        public bool ResponseHasWarning { get; set; }
        public string ResponseWarningMessage { get; set; }

    }

    [Serializable]
    public class VanSalesDailyCash : PersistableEntity<int>
    {
        [Key]
        public int VanSalesDailyCashId { get; set; }
        public DateTime SaleDate { get; set; }
        public int TerminalId { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        public int MobileLocationId { get; set; }
        [ForeignKey("MobileLocationId")]
        public virtual TenantLocations TenantLocations { get; set; }
        public int SalesManUserId { get; set; }
        [ForeignKey("SalesManUserId")]
        public virtual AuthUser SalesManUser { get; set; }
        public int FiveHundred { get; set; }
        public int TwoHundred { get; set; }
        public int OneHundred { get; set; }
        public int Fifty { get; set; }
        public int Twenty { get; set; }
        public int Ten { get; set; }
        public int Five { get; set; }
        public int Two { get; set; }
        public int One { get; set; }
        public int PointFifty { get; set; }
        public int PointTwentyFive { get; set; }
        public int PointTwenty { get; set; }
        public int PointTen { get; set; }
        public int PointFive { get; set; }
        public int PointTwo { get; set; }
        public int PointOne { get; set; }
        public decimal TotalCashSale { get; set; }
        public decimal TotalCardSale { get; set; }
        public decimal TotalChequeSale { get; set; }
        public decimal TotalSale { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalNetSale { get; set; }
        public decimal TotalNetTax { get; set; }
        public decimal TotalPaidCash { get; set; }
        public decimal TotalPaidCheques { get; set; }
        public decimal TotalPaidCards { get; set; }
        public decimal TotalCashSubmitted { get; set; }
        public decimal TotalChequeSubmitted { get; set; }
        public decimal TotalCardSubmitted { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int ChequesCount { get; set; }
        public int CashCount { get; set; }
        public int CardCount { get; set; }
        public string Notes { get; set; }
        public decimal ReturnOrderValue { get; set; }
        public int ReturnOrderCount { get; set; }
    }
}