using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class StockTakeSerialSnapshot
    {
        [Key]
        public int StockTakeSerialSnapshotId { get; set; }
        public int StockTakeSnapshotId { get; set; }
        [ForeignKey("StockTakeSnapshotId")]
        public virtual StockTakeSnapshot StockTakeSnapshot { get; set; }
        public int StockTakeId { get; set; }
        [ForeignKey("StockTakeId")]
        public virtual StockTake StockTake { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }
        public int ProductSerialId { get; set; }
        [ForeignKey("ProductSerialId")]
        public virtual ProductSerialis ProductSerial { get; set; }
        public InventoryTransactionTypeEnum CurrentStatus { get; set; }
        public bool IsInStock { get; set; }
    }

    public class StockTakePalletsSnapshot
    {
        [Key]
        public int StockTakePalletSnapshotId { get; set; }
        public int StockTakeSnapshotId { get; set; }
        [ForeignKey("StockTakeSnapshotId")]
        public virtual StockTakeSnapshot StockTakeSnapshot { get; set; }
        public int StockTakeId { get; set; }
        [ForeignKey("StockTakeId")]
        public virtual StockTake StockTake { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }
        public int PalletTrackingId { get; set; }
        [ForeignKey("PalletTrackingId")]
        public virtual PalletTracking PalletTracking { get; set; }
        public PalletTrackingStatusEnum Status { get; set; }
        public decimal RemainingCases { get; set; }
        public decimal TotalCases { get; set; }
    }
}