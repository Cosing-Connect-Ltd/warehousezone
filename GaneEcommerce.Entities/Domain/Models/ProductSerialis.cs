using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductSerialis
    {
        public ProductSerialis()
        {
            TenantWarranty = new HashSet<TenantWarranty>();
        }
        [Key]
        [Display(Name = "Serial Id")]
        public int SerialID { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Serial Number")]
        public string  SerialNo { get; set; }
        [Display(Name = "RFID")]
        public string ElectronicTag { get; set; }
        [Display(Name = "Status")]
        public InventoryTransactionTypeEnum CurrentStatus { get; set; }
        [Display(Name = "Batch")]
        public string Batch { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Buy Price")]
        public decimal BuyPrice { get; set; }


        [Display(Name = "Warranty Type")]
        public int? WarrantyID { get; set; }
        public int SoldWarrantyDays { get; set; }
        public bool SoldWarrantyIsPercent { get; set; }
        public decimal SoldWarrantyPercentage { get; set; }
        public decimal SoldWarrantyFixedPrice { get; set; }

        [Display(Name = "Warranty Start Date")]
        public DateTime? SoldWarrantyStartDate { get; set; }
        [Display(Name = "Warranty End Date")]
        public DateTime? SoldWarrentyEndDate { get; set; }
        public string SoldWarrantyName { get; set; }
        public DeliveryMethods? DeliveryMethod { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
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
        public int? WarehouseId { get; set; }
        public int? LocationId { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual ICollection<TenantWarranty> TenantWarranty { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransaction { get; set; }
        public virtual TenantLocations TenentWarehouse { get; set; }
        public virtual Locations Location { get; set; }


    }
}