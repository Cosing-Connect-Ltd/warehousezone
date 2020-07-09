using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderDetailSessionViewModel
    {
        [Display(Name = "Order Detail Id")]
        public int OrderDetailID { get; set; }
        [Display(Name = "Order Id")]
        [Required]
        public int OrderID { get; set; }
        [Display(Name = "Warehouse Id")]
        [Required]
        public int WarehouseId { get; set; }
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

        public decimal? CaseQuantity { get; set; }

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
        public OrderStatusEnum? OrderDetailStatusId { get; set; }

        [Display(Name = "Ordering Notes")]
        public string OrderingNotes { get; set; }
        public virtual ProductMasterViewModel ProductMaster { get; set; }
        public virtual GlobalTaxViewModel TaxName { get; set; }
        public virtual ProductAccountCodesViewModel AccountCode { get; set; }

        public string CurrencySign { get; set; }

        public string ProductPath { get; set; }

        public int? CurrencyId { get; set; }

        public bool? isNotfication { get; set; }

        public List<KitProductCartSession> KitProductCartItems { get; set; }

        public decimal ProductTotalAmount => Math.Round((Qty * Price), 2);

        public int? CartId { get; set; }

        public bool ShowLoginPopUp { get; set; }

        public bool ShowCartPopUp { get; set; }


}

    [Serializable]
    public class KitProductCartSession
    {
        public int SimpleProductId { get; set; }

        public int KitProductId { get; set; }

        public decimal Quantity { get; set; }

        public ProductMasterViewModel SimpleProductMaster { get; set; }




    }


}