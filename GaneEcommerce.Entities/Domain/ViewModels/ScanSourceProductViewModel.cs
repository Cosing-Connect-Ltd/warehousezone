using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class ScanSourceProductViewModel
    {

    }

    public class ScanSourceSearchproductModel
    {
        public string ScanSourceItemNumber { get; set; }
        public string ManufacturerItemNumber { get; set; }
        public string Manufacturer { get; set; }
        public string ItemStatus { get; set; }
        public string PlantMaterialStatusValidfrom { get; set; }
        public string ItemImage { get; set; }
        public Nullable<bool> ReboxItem { get; set; }
        public Nullable<bool> BStockItem { get; set; }
        public string CatalogName { get; set; }
        public string BusinessUnit { get; set; }
        public string CategoryPath { get; set; }
        public string ProductFamilyImage { get; set; }
        public string ProductFamilyDescription { get; set; }
        public string ProductFamilyHeadline { get; set; }
        public string Description { get; set; }
        public string ProductFamily { get; set; }
    }

    public class ProductDetails
    {
        public string ScanSourceItemNumber { get; set; }
        public string ManufacturerItemNumber { get; set; }
        public string Manufacturer { get; set; }
        public string ItemStatus { get; set; }
        public string PlantMaterialStatusValidfrom { get; set; }
        public string ItemImage { get; set; }
        public string ReboxItem { get; set; }
        public string BStockItem { get; set; }
        public string CatalogName { get; set; }
        public string BusinessUnit { get; set; }
        public string CategoryPath { get; set; }
        public string ProductFamilyImage { get; set; }
        public string ProductFamilyDescription { get; set; }
        public string ProductFamilyHeadline { get; set; }
        public string Description { get; set; }
        public string ProductFamily { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public string GeneralItemCategoryGroup { get; set; }
        public string GrossWeight { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialType { get; set; }
        public string BatteryIndicator { get; set; }
        public string RoHSComplianceIndicator { get; set; }
        public string ManufacturerDivision { get; set; }
        public string CommodityImportCodeNumber { get; set; }
        public string CountryofOrigin { get; set; }
        public string UNSPSC { get; set; }
        public string DeliveringPlant { get; set; }
        public string MaterialFreightGroup { get; set; }
        public string MinimumOrderQuantity { get; set; }
        public string SalespersonInterventionRequired { get; set; }
        public string SellviaEDI { get; set; }
        public string SellviaWeb { get; set; }
        public string SerialNumberProfile { get; set; }
        public string PackagedLength { get; set; }
        public decimal? PackagedWidth { get; set; }
        public decimal? PackagedHeight { get; set; }
        public List<ProductMedia> ProductMedia { get; set; }
    }

    public class ProductMedia
    {
        public string MediaType { get; set; }
        public string URL { get; set; }
    }

    public class ScanSourceProductPricePost
    {
        public string CustomerNumber { get; set; }

        //public string Warehouse { get; set; }
        //public string BussinessUnit { get; set; }
        //public string DealID1 { get; set; }

        public ICollection<PricingRequestLine> Lines { get; set; }

    }
    public class PricingRequestLine
    {

        public string itemNumber { get; set; }

        //public string PartNumberType { get; set; }

        //public int Quantity { get; set; }


    }

    public class ScanSourceProductPrice
    {
        public string ItemNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitPriceCurrencyCode { get; set; }
        public decimal? ExtendedTotal { get; set; }
        public string ExtendedTotalCurrency { get; set; }
        public decimal MSRP { get; set; }
        public string MSRPCurrencyCode { get; set; }
        public bool? DealerAuthorized { get; set; }
        public bool? SPA { get; set; }
        public string SPADescription { get; set; }
        public bool? SPARestriction { get; set; }
        public string SPARestrictionDescription { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityOnOrder { get; set; }
        public bool? PricingError { get; set; }
        public string Pricing { get; set; }
        public string InventoryDisplay { get; set; }
        public string DealInfos { get; set; }



    }
}