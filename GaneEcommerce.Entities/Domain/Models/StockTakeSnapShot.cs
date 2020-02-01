using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{

    [Serializable]
    public class StockTakeSnapshot
    {

        public StockTakeSnapshot()
        {
            StockTakeSerialSnapshots = new HashSet<StockTakeSerialSnapshot>();
            StockTakePalletsSnapshot = new HashSet<StockTakePalletsSnapshot>();
        }

        [Key]
        [Display(Name = "Snapshot Id")]
        public int StockTakeSnapshotId { get; set; }
        [Display(Name = "Stocktake")]
        public int StockTakeId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [StringLength(50)]
        [Display(Name = "SKU")]
        public string ReceivedSku { get; set; }
        [Display(Name = "Action")]
        public bool ActionTaken { get; set; }
        [Display(Name = "Previous Quantity")]
        public decimal PreviousQuantity { get; set; }
        [Display(Name = "New Quantity")]
        public decimal NewQuantity { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual StockTake StockTake { get; set; }

        public virtual ICollection<StockTakeSerialSnapshot> StockTakeSerialSnapshots { get; set; }
        public virtual ICollection<StockTakePalletsSnapshot> StockTakePalletsSnapshot { get; set; }
    }
}
