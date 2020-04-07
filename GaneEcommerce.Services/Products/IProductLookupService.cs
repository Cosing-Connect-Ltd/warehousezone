using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IProductLookupService
    {

        IEnumerable<ProductLotOptionsCodes> GetAllValidProductLotOptionsCodes();
        IEnumerable<ProductLotProcessTypeCodes> GetAllValidProductLotProcessTypeCodes();
        IEnumerable<Locations> GetAllValidProductLocations(int tenantId, int warehouseId, int filterForProductId = 0);
        IEnumerable<ProductAttributes> GetAllValidProductAttributes();
        IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValues();
        IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValuesByProductId(int productId);
        IEnumerable<ProductAttributeValuesMap> GetAllValidProductAttributeValuesMap();
        IEnumerable<ProductSCCCodes> GetAllProductSccCodesByProductId(int productId, int tenantId);
        IEnumerable<ProductLocations> GetAllProductLocationsByProductId(int productId, int warehouseId);

        Locations GetLocationById(int locationId);
        ProductGroups GetProductGroupById(int productGroupId);
        IQueryable<ProductMaster> GetAllValidProductGroupById(int? productGroupId, int? departmentId = null);
        IQueryable<ProductMaster> GetAllValidProductGroupAndDeptByName(string productGroup, string department = "", string SubCategory = "", string manufacturerName = "", string productName="");
        IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(IQueryable<ProductMaster> productMasters, string productGroup, string department = "", string SubCategory = "");

        IEnumerable<string> GetAllValidSubCategoriesByDepartmentAndGroup(IQueryable<ProductMaster> productMasters);
        List<Tuple<string, string>> AllPriceListAgainstGroupAndDept(IQueryable<ProductMaster> productMasters, string productGroup, string department = "", string SubCategory = "");

        ProductGroups GetProductGroupByName(string groupName);
        PalletType GetPalletTypeByName(string palletType);
        ProductSCCCodes GetProductSccCodesById(int productSccCodesId);
        ProductAttributeValues GetProductAttributeValueById(int productAttributeValueId);
        ProductAttributeValuesMap GetProductAttributeValueMap(int productId, int attributeValueId);
        PalletType CreatePalletType(PalletType model, int userId, int tenantId);
        PalletType UpdatePalletType(PalletType model, int userId);
        PalletType GetPalletTypeById(int palletTypeId);
        void DeletePalletType(int palletTypeId, int userId);
        ProductAttributes SaveProductAttribute(string attributeName);
        ProductAttributeValues SaveProductAttributeValue(int attributeId, string attributeValue, int userId = 0);
        ProductAttributeValues SaveProductAttributeValueMap(ProductAttributeValues attributeValue, int userId, int tenantId, int productId);
        void DeleteProductAttributeValue(int productId, int attributeValueId, int userId, int tenantId);


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
        IQueryable<ProductMaster> FilterProduct(IQueryable<ProductMaster> productMaster, string filterstring);

        IEnumerable<ProductKitType> GetProductKitTypes(List<int?> kitIds);

        Dictionary<string, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> product);


    }
}