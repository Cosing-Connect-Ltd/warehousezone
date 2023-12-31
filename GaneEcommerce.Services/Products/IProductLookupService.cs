using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Models.AdyenPayments;

namespace Ganedata.Core.Services
{
    public interface IProductLookupService
    {

        IEnumerable<ProductLotOptionsCodes> GetAllValidProductLotOptionsCodes();
        IEnumerable<ProductLotProcessTypeCodes> GetAllValidProductLotProcessTypeCodes();
        IEnumerable<Locations> GetAllValidProductLocations(int tenantId, int warehouseId, int filterForProductId = 0);
        IEnumerable<ProductAttributes> GetAllValidProductAttributes();
        IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValues();
        IEnumerable<ProductAttributeValuesMap> GetAllValidProductAttributeValuesMapsByProductId(int productId);
        IEnumerable<ProductAttributeValuesMap> GetAllValidProductAttributeValuesMap();
        IEnumerable<ProductSCCCodes> GetAllProductSccCodesByProductId(int productId, int tenantId);
        IEnumerable<ProductLocations> GetAllProductLocationsByProductId(int productId, int warehouseId);
        IEnumerable<ProductAttributeMap> GetAllValidProductAttributeMaps(int TenantId);
        Locations GetLocationById(int locationId);
        ProductGroups GetProductGroupById(int productGroupId);
        IQueryable<ProductMaster> GetAllValidProductGroupById(int? productGroupId, int? departmentId = null);
        IQueryable<ProductMaster> GetAllValidProductGroupAndDeptByName(int siteId, string category = "", string manufacturerName = "", string ProductName = "");
        IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(IQueryable<ProductMaster> productMasters);
        Dictionary<int, string> GetAllValidSubCategoriesByDepartmentAndGroup(List<int> productIds);
        IEnumerable<WebsiteNavigation> GetWebsiteNavigationCategoriesList(int? parentCategoryId, int siteId);
        List<Tuple<string, string>> AllPriceListAgainstGroupAndDept(IQueryable<ProductMaster> productMasters);

        ProductGroups GetProductGroupByName(string groupName);
        PalletType GetPalletTypeByName(string palletType);
        ProductSCCCodes GetProductSccCodesById(int productSccCodesId);
        ProductAttributeValues GetProductAttributeValueById(int productAttributeValueId);
        ProductAttributeValuesMap GetProductAttributeValuesMapById(int id);
        ProductAttributeValuesMap GetProductAttributeValueMap(int productId, int attributeValueId);
        PalletType CreatePalletType(PalletType model, int userId, int tenantId);
        PalletType UpdatePalletType(PalletType model, int userId);
        PalletType GetPalletTypeById(int palletTypeId);
        void DeletePalletType(int palletTypeId, int userId);
        ProductAttributes SaveProductAttribute(string attributeName, int sortOrder, bool isColorTyped, int? attributeId=null, bool isPriced = false);
        ProductAttributeValues SaveProductAttributeValue(int attributeId, string attributeValue, int sortOrder, string color, int userId = 0, int? attributeValueId = null);
        bool SaveProductAttributeValueMap(int attributeValueId, int userId, int tenantId, int productId, int? productAttributeValueMapId);
        void DeleteProductAttributeValuesMap(int productId, int attributeValueId, int userId, int tenantId);


        Locations SaveProductLocation(Locations model, int warehouseId, int tenantId, int userId, int productId = 0);
        void DeleteProductLocation(int productId, int locationId, int warehouseId, int tenantId, int userId);

        ProductSCCCodes SaveSccCode(ProductSCCCodes model, int productId, int userId, int tenantId);
        void DeleteSccCode(int productSccCodesId, int userId);
        ProductGroups CreateProductGroup(ProductGroups model, int userId, int tenantId);
        ProductGroups UpdateProductGroup(ProductGroups model, int userId);
        void DeleteProductGroup(int productGroupId, int userId);

        ProductCategory GetProductCategoryById(int productCategoryId);
        ProductCategory GetProductCategoryByName(string categoryName);
        ProductCategory CreateProductCategory(ProductCategory model, int userId, int tenantId);
        ProductCategory UpdateProductCategory(ProductCategory model, int userId, int tenantId);
        void DeleteProductCategory(int id, int userId);
        List<WastageReason> GetAllWastageReasons();
        IEnumerable<ProductKitType> GetProductKitTypes(int TenantId);
        ProductKitType GetProductKitTypeById(int productKitTypeId);
        ProductKitType CreateProductKitType(ProductKitType model, int userId, int tenantId);
        ProductKitType UpdateProductKitType(ProductKitType model, int userId, int tenantId);
        void DeleteProductKitType(int productKitTypeId, int userId);
        ProductKitType GetProductKitTypeByName(string productKitTypeName);
        IQueryable<ProductMaster> ApplyFixedFilters(IQueryable<ProductMaster> productMaster, string filterString,int siteId, int? accountId);
        IQueryable<ProductMaster> ApplyAttributeFilters(IQueryable<ProductMaster> productMaster, string filterString, int siteId);

        List<ProductKitType> GetProductKitTypes(List<int?> kitIds);

        Dictionary<string, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> product);

        //ProductTags
        IEnumerable<ProductTag> GetAllValidProductTag(int TenantId);
        ProductTag CreateOrUpdateProductTag(ProductTag productTag, int UserId, int TenantId);
        ProductTag RemoveProductTag(int Id, int UserId);
        ProductTag GetProductTagById(int Id);

        // CreateUpdateProductKitMap

        bool CreateOrUpdateKitMap(ProductMasterViewModel productMaster, int ProductId, ProductKitTypeEnum productKitType, int UserId, int TenantId);

        bool RemoveProductAttriubteValueMap(int id, int userId);

        bool RemoveProductAttriubte(int id);

        Dictionary<string, List<string>> ReadFiltersString(string filterString);
        ProductAttributes GetProductAttributeById(int productAttributeId);
        ProductSpecialAttributePrice SaveProductAttributeValuesMap(ProductSpecialAttributePrice model, int userId, int tenantId);
        bool DeleteProductAttributeValuesMap(int id, int userId);



        IQueryable<MarketVehicle> GetAllTrucks(int TenantId);

        MarketVehicle CreateOrUpdateTruck(MarketVehicle truck, int UserId, int TenantId);

        MarketVehicle RemoveTruck(int Id, int UserId);

    }
}