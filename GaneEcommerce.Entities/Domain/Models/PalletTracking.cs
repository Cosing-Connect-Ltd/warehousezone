using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PalletTracking
    {
        public PalletTracking()
        {
            InventoryTransaction = new HashSet<InventoryTransaction>();
        }
        [Key]
        public int PalletTrackingId { get; set; }

        [Display(Name ="Product")]
        public int ProductId { get; set; }

        public int? OrderId { get; set; }

        public string OrderNumber { get; set; }
        public string PalletSerial { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        public decimal RemainingCases { get; set; }
        [Display(Name = "Total Cases")]
        public decimal TotalCases { get; set; }
        [Display(Name = "Batch No")]
        public string BatchNo { get; set; }
        public string Comments { get; set; }
        public PalletTrackingStatusEnum Status { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransaction { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenentWarehouse { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        

    }
}