using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ReceivePOVM
    {
        [Key]
        public int OrderID { get; set; }
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }
        [Display(Name = "Delivery Reference")]
        public string DeliveryNumber { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        [Required]
        [Display(Name = "Delivery Method")]
        public DeliveryMethods DeliveryMethod { get; set; }
        [Display(Name = "Delivery Service")]
        public int? TenantDeliveryServiceId { get; set; }
        public virtual Account Account { get; set; }

        public int AccountID { get; set; }
        public int OrderStatusID { get; set; }
        [Display(Name = "Shipment Address Line1")]
        public string ShipmentAddressLine1 { get; set; }
        [Display(Name = "Shipment Address Line2")]
        public string ShipmentAddressLine2 { get; set; }
        [Display(Name = "Shipment Address Line3")]
        public string ShipmentAddressLine3 { get; set; }
        [Display(Name = "Shipment Address Line4")]
        public string ShipmentAddressLine4 { get; set; }
        [Display(Name = "Shipment Address Postcode")]
        public string ShipmentAddressPostcode { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public string WorksResourceName { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class AccountShipmentInfo
    {
        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressLine3 { get; set; }
        public string ShipmentAddressLine4 { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public int? OrderProcessId { get; set; }
        public string FSC { get; set; }
        public string PEFC { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string  DeliveryNo { get; set; }
        public int? UserId { get; set; }


    }

    [Serializable]
    public class BSDto
    {
        public int Id { get; set; }
        public string Serial { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public bool IsSerial { get; set; }
        public bool IsNewProduct { get; set; }
        public string ProductName { get; set; }
        public int? TaxId { get; set; }

        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }

        public string GroupProduct { get; set; }

        public string ProductDesc { get; set; }
        public int? ProductGroupId { get; set; }
        public int? ProductDepartmentId { get; set; }
        public string FscPercent { get; set; }



    }
}