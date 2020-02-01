using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.Collections.Generic;
    [Serializable]
    public class StockTake
    {
        public StockTake()
        {
            this.StockTakeDetails = new HashSet<StockTakeDetail>();
            this.StockTakeSnaphots = new HashSet<StockTakeSnapshot>();
        }
    
        [Key]
        [Display(Name = "Trans Id")]
        public int StockTakeId { get; set; }
        [Display(Name = "Reference")]
        public string StockTakeReference { get; set; }
        [Display(Name = "Description")]
        public string StockTakeDescription { get; set; }
        [Display(Name = "Start Date")]
        public System.DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        public virtual ICollection<StockTakeDetail> StockTakeDetails { get; set; }
        public virtual ICollection<StockTakeSnapshot> StockTakeSnaphots { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual TenantLocations TenantWarehouse { get; set; }
    }
}
