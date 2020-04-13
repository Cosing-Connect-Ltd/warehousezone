using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{

    public class StockMovemeneCollectionViewModel
    {
        public int Count { get; set; }
        public string SerialNo { get; set; }
        public List<StockMovementViewModel> StockMovements { get; set; }
    }

    [Serializable]
    public class StockMovementViewModel
    {
        [Required]
        [Display(Name = "From Location")]
        public int? FromLocation { get; set; }
        [Required]
        [Display(Name = "To Location")]
        public int? ToLocation { get; set; }
        public List<int> SerialIds { get; set; }
        public List<int> PalletSerialIds { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public decimal Qty { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int TenentId { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class InventoryStockViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string ProductGroup { get; set; }
        public string DepartmentName { get; set; }

        public string SkuCode { get; set; }

        public string Barcode { get; set; }

        public decimal InStock { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
        public decimal OnOrder { get; set; }
        public bool? PalletProduct { get; set; }
        public bool? SerialProduct { get; set; }

        public bool? StockIssue { get; set; }

    }
}