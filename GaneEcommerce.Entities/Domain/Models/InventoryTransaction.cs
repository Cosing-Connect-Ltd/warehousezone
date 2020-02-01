using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class InventoryTransaction
    {
        [Key]
        [Display(Name = "Transaction Id")]
        public int InventoryTransactionId { get; set; }
        [Display(Name = "Transaction Type Id")]
        public int InventoryTransactionTypeId { get; set; }
        [Display(Name = "Order")]
        public int? OrderID { get; set; }
        [Display(Name = "Reference")]
        public string InventoryTransactionRef { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        public decimal LastQty { get; set; }
        public int? WastageReasonId { get; set; }
        [ForeignKey("WastageReasonId")]
        public virtual WastageReason WastageReason { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public int? SerialID { get; set; }
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
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsCurrentLocation { get; set; }
        public bool? DontMonitorStock { get; set; }
        [Display(Name = "Terminal Id")]
        public int? TerminalId { get; set; }
        public virtual InventoryTransactionType InventoryTransactionType { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual TenantLocations TenantWarehouse { get; set; }
        public virtual Locations Location { get; set; }
        public virtual ProductSerialis ProductSerial { get; set; }
        public virtual Order Order { get; set; }
        public int? OrderProcessDetailId { get; set; }
        public int? OrderProcessId { get; set; }
        [ForeignKey("OrderProcessId")]
        public virtual OrderProcess OrderProcess { get; set; }
        [ForeignKey("OrderProcessDetailId")]
        public virtual OrderProcessDetail OrderProcessDetail { get; set; }
        public int? PalletTrackingId { get; set; }
        [ForeignKey("PalletTrackingId")]
        public virtual PalletTracking PalletTracking { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual AuthUser AuthUsers { get; set; }
    }

    [Table("WastageReason")]
    [Serializable]
    public class WastageReason
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
