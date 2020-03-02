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
        public bool IsRawMaterial { get; set; }
        public bool IsSelectable { get; set; }



        public string UOM { get; set; }
        public bool Kit { get; set; }
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
        public string DepartmentName { get; set; }
        public string Location { get; set; }

        public List<ProductRecipeItemViewModel> AllAvailableSubItems { get; set; }
        public List<ProductRecipeItemViewModel> AllSelectedSubItems { get; set; }
        public bool EnableWarranty { get; set; }
        public bool EnableTax { get; set; }
        public bool? DontMonitorStock { get; set; }
        [Display(Name = "Process by Case")]
        public bool ProcessByCase { get; set; }
        [Display(Name = "Process by Pallet")]
        public bool ProcessByPallet { get; set; }
        public bool TopProduct { get; set; }
        public bool BestSellerProduct { get; set; }
        public bool SpecialProduct { get; set; }
        public bool OnSaleProduct { get; set; }
        public string HoverImage { get; set; }
        public string DefaultImage { get; set; }

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
}