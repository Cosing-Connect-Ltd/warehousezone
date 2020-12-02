using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductRecipeItemViewModel
    {
        public int ProductId { get; set; }
        public int ParentProductId { get; set; }
        public string Name { get; set; }
        public string SKUCode { get; set; }
        public string BarCode { get; set; }
        public decimal Quantity { get; set; }

        public int ProductKitId { get; set; }

        public string ProductKitType { get; set; }
        public ProductKitTypeEnum? ProductKitTypeEnum { get; set; }

    }

    [Serializable]
    public class ProductMasterViewModel
    {
        public ProductMasterViewModel()
        {
            AllSelectedSubItems = new List<ProductRecipeItemViewModel>();
            AllAvailableSubItems = new List<ProductRecipeItemViewModel>();

        }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string SKUCode { get; set; }
        public string Description { get; set; }
        public string BarCode { get; set; }
        public bool Serialisable { get; set; }
        public bool IsStockItem { get; set; }
        public bool IsSelectable { get; set; }
        public string UOM { get; set; }

        public ProductKitTypeEnum ProductType { get; set; }
        public string BarCode2 { get; set; }
        public int? ShelfLifeDays { get; set; }
        public decimal? ReorderQty { get; set; }
        public string ShipConditionCode { get; set; }
        public string CommodityCode { get; set; }
        public string CommodityClass { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Weight { get; set; }
        public double Depth { get; set; }
        public decimal PercentMargin { get; set; }
        public string LotOptionDescription { get; set; }
        public string TaxName { get; set; }
        public string GlobalWeightGrpDescription { get; set; }
        public bool LotOption { get; set; }
        public bool Discontinued { get; set; }
        public DateTime ProdStartDate { get; set; }
        public string ProductLotProcessTypeCodesDescription { get; set; }
        public decimal Available { get; set; }
        public decimal Allocated { get; set; }
        public decimal InStock { get; set; }
        public decimal OnOrder { get; set; }
        public string ProductGroupName { get; set; }
        public string ProductCategoryName { get; set; }
        public string DepartmentName { get; set; }
        public string Location { get; set; }
        public List<ProductRecipeItemViewModel> AllAvailableSubItems { get; set; }
        public List<ProductRecipeItemViewModel> AllSelectedSubItems { get; set; }
        public bool EnableWarranty { get; set; }
        public bool EnableTax { get; set; }
        public bool? DontMonitorStock { get; set; }
        public bool? IsPreOrderAccepted { get; set; }
        public int? MinDispatchDays { get; set; }
        public int? MaxDispatchDays { get; set; }
        [Display(Name = "Process by Case")]
        public bool ProcessByCase { get; set; }
        [Display(Name = "Process by Pallet")]
        public bool ProcessByPallet { get; set; }
        public decimal? BuyPrice { get; set; }
        [Display(Name = "Landing Cost")]
        public decimal? LandedCost { get; set; }
        [Display(Name = "Sell Price")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? SellPrice { get; set; }
        public string HoverImage { get; set; }
        public string DefaultImage { get; set; }

        public decimal? Qty { get; set; }

        public int? Id { get; set; }

        public bool? IsActive { get; set; }
        public List<string> AttributeValueNames { get; set; }

        public List<string> ProductTagMap { get; set; }
        public string TagIds { get; set; }
    }


    public class WarehouseProductLevelViewModel
    {
        public int ProductLocationStockLevelID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int TenantLocationID { get; set; }
        public decimal ReOrderQuantity { get; set; }
        public decimal MinStockQuantity { get; set; }
        public string SKUCode { get; set; }
    }

    [Serializable]
    public class LabelPrintViewModel
    {
        public int? OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductBarcode { get; set; }
        public string ProductSkuCode { get; set; }
        public string ProductName { get; set; }
        public string Comments { get; set; }
        public string BatchNumber { get; set; }
        public short LabelsCount { get; set; }
        public int? PalletsCount { get; set; }
        public int Quantity { get; set; }
        public int? Cases { get; set; }
        public DateTime LabelDate { get; set; }
        public string OrderNumber { get; set; }
        public string PalletSerial { get; set; }
        public bool? RequiresBatchNumber { get; set; }
        public bool? RequiresExpiryDate { get; set; }
    }
}