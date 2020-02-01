using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class StockTakeDetailsSerial
    {
        public StockTakeDetailsSerial()
        {
            DateScanned = DateTime.UtcNow;
        }

        [Key]
        [Display(Name = "Stocktake SerialId")]
        public int StockTakeDetailsSerialId { get; set; }
        public int ProductSerialId { get; set; }
        [ForeignKey("ProductSerialId")]
        public virtual ProductSerialis ProductSerial { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }
        public string SerialNumber { get; set; }
        public int StockTakeDetailId { get; set; }
        [ForeignKey("StockTakeDetailId")]
        public virtual StockTakeDetail StockTakeDetail { get; set; }
        [Display(Name = "Date Scanned")]
        public System.DateTime DateScanned { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }

    }

    public class StockTakeDetailsPallets
    {
        public StockTakeDetailsPallets()
        {
            DateScanned = DateTime.UtcNow;
        }

        [Key]
        [Display(Name = "Stocktake Pallet Id")]
        public int StockTakeDetailPalletId { get; set; }
        public int ProductPalletId { get; set; }
        [ForeignKey("ProductPalletId")]
        public virtual PalletTracking PalletTracking { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }
        public string PalletSerial { get; set; }
        public int StockTakeDetailId { get; set; }
        [ForeignKey("StockTakeDetailId")]
        public virtual StockTakeDetail StockTakeDetail { get; set; }
        [Display(Name = "Date Scanned")]
        public System.DateTime DateScanned { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }

    }


}