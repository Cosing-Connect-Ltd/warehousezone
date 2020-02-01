using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderDetail
    {
        [Key]
        [Display(Name = "Order Detail Id")]
        public int OrderDetailID { get; set; }
        [Display(Name = "Order Id")]
        [Required]
        public int OrderID { get; set; }
        [Display(Name = "Warehouse Id")]
        [Required]
        public int WarehouseId { get; set; }
        [Column(TypeName = "date")]
        // [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expected Date")]
        public DateTime? ExpectedDate { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Account Code")]
        public int? ProdAccCodeID { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Qty { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
        [Display(Name = "Warranty")]
        public int? WarrantyID { get; set; }
        [Display(Name = "Warranty Amount")]
        public decimal WarrantyAmount { get; set; }
        [Display(Name = "Tax")]
        public int? TaxID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int UpdatedBy { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Don't Monitor Stock")]
        public bool? DontMonitorStock { get; set; }
        public int SortOrder { get; set; }
        public int? OrderDetailStatusId { get; set; }
        [ForeignKey("OrderDetailStatusId")]
        public OrderStatus OrderDetailStatus { get; set; }
        public int? ProductGroupId { get; set; }
        [ForeignKey("ProductGroupId")]
        public virtual ProductGroups ProductGroups { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual TenantLocations TenantWarehouse { get; set; }
        public virtual GlobalTax TaxName { get; set; }
        public virtual TenantWarranty Warranty { get; set; }
        public virtual ProductAccountCodes AccountCode { get; set; }
        public decimal ProcessedQty
        {
            get
            {
                if (Order == null || Order.OrderProcess == null || !Order.OrderProcess.Any()) return 0;
                return this.Order.OrderProcess.Where(d => d.IsDeleted != true && d.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.Returns && d.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.Wastage
                && d.InventoryTransactionTypeId != (int)InventoryTransactionTypeEnum.WastedReturn)
                .SelectMany(m => m.OrderProcessDetail).Where(d => d.IsDeleted != true)
                    .Where(x => x.OrderDetailID == this.OrderDetailID).Sum(x => x.QtyProcessed);
            }
        }
        public decimal ReturnedQty
        {
            get
            {
                if (Order == null || Order.OrderProcess == null || !Order.OrderProcess.Any()) return 0;
                return this.Order.OrderProcess.Where(d => d.IsDeleted != true && (d.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns || d.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Wastage
                || d.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WastedReturn))
                .SelectMany(m => m.OrderProcessDetail).Where(d => d.IsDeleted != true)
                    .Where(x => x.OrderDetailID == this.OrderDetailID).Sum(x => x.QtyProcessed);
            }
        }

       
    }

}