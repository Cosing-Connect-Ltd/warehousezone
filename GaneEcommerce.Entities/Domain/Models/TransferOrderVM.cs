using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace WMS.VModels
{
    [Serializable]
    public class TransferOrderVM
    {
        public int OrderID { get; set; }
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }
        public int Type { get; set; }
        [Display(Name = "Warehouse")]
        public string Warehouse { get; set; }
        [Display(Name = "Delivery Number")]
        public string DeliveryNumber { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public InventoryTransactionTypeEnum InventoryTransactionTypeId { get; set; }
    }
}