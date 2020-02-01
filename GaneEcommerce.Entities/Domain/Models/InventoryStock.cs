using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{


    [Serializable]
    public class InventoryStock
    {
        [Key]
        [Display(Name = "Stock Id")]
        public int InventoryStockId { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        [Display(Name = "In Stock")]
        public decimal InStock { get; set; }
        [Display(Name = "In Stock")]
        public decimal OnOrder { get; set; }
        [Display(Name = "On Order")]
        public decimal Allocated { get; set; }
        [Display(Name = "Available")]
        public decimal Available { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual TenantLocations TenantWarehous { get; set; }
    }
}
