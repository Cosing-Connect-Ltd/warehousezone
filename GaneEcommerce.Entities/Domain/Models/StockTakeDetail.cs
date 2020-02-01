using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class StockTakeDetail
    {
        [Key]
        [Display(Name = "Detail Id")]
        public int StockTakeDetailId { get; set; }
        [Display(Name = "Stocktake")]
        public int StockTakeId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }

        [StringLength(50)]
        [Display(Name = "SKU")]
        public string ReceivedSku { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Date Scanned")]
        public System.DateTime DateScanned { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }

        public bool? IsApplied { get; set; }
        public DateTime? DateApplied { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        public virtual StockTake StockTake { get; set; }
        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Locations Location { get; set; }

    }
}
