using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderProcess
    {
        public OrderProcess()
        {
            OrderProcessDetail = new HashSet<OrderProcessDetail>();
            OrderProofOfDelivery = new HashSet<OrderProofOfDelivery>();
            Pallets = new HashSet<Pallet>();
            PalletsDispatches = new HashSet<PalletsDispatch>();
        }

        [Key]
        [Display(Name = "Process Id")]
        public int OrderProcessID { get; set; }
        [Display(Name = "Delivery Number")]
        public string DeliveryNO { get; set; }
        [Display(Name = "Delivery Method")]
        public DeliveryMethods? DeliveryMethod { get; set; }
        [Required]
        [Display(Name = "Order Id")]
        public int? OrderID { get; set; }
        [Required]
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }

        public string InvoiceNo { get; set; }

        public InventoryTransactionTypeEnum? InventoryTransactionTypeId { get; set; }

        public OrderProcessStatusEnum? OrderProcessStatusId { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<OrderProcessDetail> OrderProcessDetail { get; set; }
        public virtual ICollection<OrderProofOfDelivery> OrderProofOfDelivery { get; set; }
        public string ShipmentAddressName { get; set; }
        [Display(Name = "Shipment Address Line1")]
        public string ShipmentAddressLine1 { get; set; }
        [Display(Name = "Shipment Address Line2")]
        public string ShipmentAddressLine2 { get; set; }
        [Display(Name = "Shipment Address Line3")]
        public string ShipmentAddressLine3 { get; set; }
        [Display(Name = "Shipment Address Town")]
        public string ShipmentAddressTown { get; set; }
        [Display(Name = "Shipment Address Postcode")]
        public string ShipmentAddressPostcode { get; set; }
        [Display(Name = "Shipment Country")]
        public int? ShipmentCountryId { get; set; }
        [ForeignKey("ShipmentCountryId")]
        public virtual GlobalCountry ShipmentCountry { get; set; }

        [Display(Name = "Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        public virtual ICollection<Pallet> Pallets { get; set; }
        public virtual ICollection<PalletsDispatch> PalletsDispatches { get; set; }
        //timber properties
        public string FSC { get; set; }
        public string PEFC { get; set; }
        public string PickContainerCode { get; set; }
    }

}