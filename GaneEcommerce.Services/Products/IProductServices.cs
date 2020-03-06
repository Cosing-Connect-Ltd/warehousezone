using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Models;
using Ganedata.Core.Entities.Enums;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public interface IProductServices
    {
        IQueryable<ProductMaster> GetAllValidProductMasters(int tenantId, DateTime? lastUpdated = null, bool includeIsDeleted = false);

        IQueryable<SelectListItem> GetAllValidProductMastersForSelectList(int tenantId, DateTime? lastUpdated = null, bool includeIsDeleted = false);

        ProductMaster GetProductMasterById(int productId);

        PalletTracking GetPalletbyPalletId(int palletTrackingId);
        IEnumerable<ProductKitMap> GetAllProductInKitsByProductId(int productId);
        IEnumerable<ProductMaster> GetAllProductInKitsByKitProductId(int productId);
        IEnumerable<int> GetAllProductsInALocationFromMaps(int locationId);
        IEnumerable<int> GetAllProductLocationsFromMaps(int productId);
        IQueryable<ProductMasterViewModel> GetAllProductMasterDetail(int tenantId, int warehouseId);
        IEnumerable<ProductAccountCodes> GetAllProductAccountCodesByProductId(int productId);
        InventoryStock GetInventoryStockByProductTenantLocation(int productId, int warehouseId);
        List<InventoryStock> GetAllInventoryStocksByProductId(int productId);
        InventoryStock GetInventoryStocksByProductAndTenantLocation(int productId, int tenantLocationId);
        IQueryable<InventoryStock> GetAllInventoryStocks(int tenantId, int warehouseId, DateTime? reqDate = null);
        IQueryable<InventoryStockViewModel> GetAllInventoryStocksList(int tenantId, int warehouseId, int filterByProductId = 0);
        IQueryable<InventoryTransaction> GetAllInventoryTransactions(int tenantId, int warehouseId);
        IEnumerable<InventoryTransaction> GetAllInventoryTransactionsByProductId(int productId, int warehouseId);
        IEnumerable<StockTakeSnapshot> GetAllStockTakeSnapshotsByProductId(int productId);
        InventoryTransaction GetInventoryTransactionById(int transactionId);
        IQueryable<ProductSerialis> GetAllProductSerial(int tenantId, int warehouseId, DateTime? lastUpdated = null);
        void DeleteInventoryTransactionById(int transactionId);
        IQueryable<InventoryTransaction> GetInventoryTransactionsByPalletTrackingId(int Id);
        decimal GetInventoryTransactionsByPalletTrackingId(int PalletTrackingId, int OrderProcessDetailId);
        IEnumerable<ProductSerialis> GetAllProductSerialsByTenantId(int warehouseId, DateTime? reqDate = null);
        IEnumerable<ProductSerialis> GetAllProductSerialsByProductId(int productId, int warehouseId);
        ProductSerialis GetProductSerialBySerialCode(string serialCode, int tenantId, bool inStockOnly = false);
        ProductSerialis SaveProductSerial(ProductSerialis serial, int userId);
        ProductMaster GetProductMasterByProductCode(string productCode, int tenantId);
        ProductMaster GetProductMasterByBarcode(string barcode, int tenantId);
        ProductMaster GetProductMasterByOuterBarcode(string outerBarcode, int tenantId);
        IEnumerable<ProductMaster> GetProductMasterDropdown(int productId);

        IEnumerable<ProductMaster> GetProductByCategory(int tenantId, int NumberofProducts, bool TopProduct = false, bool BestSellerProduct = false, bool SpecialProduct = false, bool OnSaleProduct = false);

        string GenerateNextProductCode(int tenantId);
        bool IsCodeAvailableForUse(string code, int tenantId, EnumProductCodeType codeType = EnumProductCodeType.All, int productId = 0);

        bool IsNameAvailableForUse(string Name, int tenantId, EnumProductCodeType codeType = EnumProductCodeType.All, int productId = 0);
        IQueryable<InventoryTransaction> GetInventoryTransactionsByProductSerialId(int Id);
        ProductMaster SaveProduct(ProductMaster productMaster, List<string> productAccountCodeIds,
            List<int> productAttributesIds,
            List<int> productLocationIds, List<int> productKitIds, int userId, int tenantId, List<int> SiteId);

        LocationGroup SaveLocationGroup(string locationGroupName, int userId, int tenantId);
        bool AddOrderId(int? orderId, int palletTrackingId, int? type);
        ProductKitMap UpdateProductKitMap(int productId, int kitProductId, int userId, int tenantId);

        void SaveProductSerials(List<string> serialList, int product, string delivery, int order, int location, int tenantId, int warehouseId, int userId);
        void DeleteProductsAndKits(int productId, int kitProductId, int tenantId, int userId);

        void SaveSelectedProductRecipeItems(int productId, List<RecipeProductItemRequest> recipeItems, int currentUserId);
        void SaveSelectedProductKitItems(int productId, List<RecipeProductItemRequest> kitItems, int currentUserId, int tenantId);
        void RemoveRecipeItemProduct(int productId, int recipeItemProductId, int currentUserId);
        void UpdateRecipeItemProduct(int productId, int recipeItemProductId, decimal quantity, int currentUserId);
        void RemoveKitItemProduct(int productId, int kitItemProductId, int currentUserId);
        void UpdateKitItemProduct(int productId, int kitItemProductId, decimal quantity, int currentUserId);

        bool MoveStockBetweenLocations(int transactionId, int? locationId, decimal moveQuantity, int tenantId, int warehouseId, int userId);

        List<OrderPriceViewModel> GetPriceHistoryForProduct(int productId, int tenantId);

        List<ProductAttributes> GetAllProductAttributes();
        ProductAttributeValuesMap GetProductAttributeMapById(int mapId);
        ProductAttributeValuesMap UpdateProductAttributeMap(int productId, int mapId);
        ProductAttributeValuesMap RemoveProductAttributeMap(int productId, int mapId);
        ProductMaster UpdateProductAttributeMapCollection(int productId, int?[] mapIds);
        List<ProductAttributeValues> GetAllProductAttributeValuesForAttribute(int attributeId);
        Dictionary<int, string> GetProductAttributesValues(int productId, int attributeId, string valueText);
        ProductAccountCodes CreateProductAccountCodes(ProductAccountCodes accountCode, int tenantId, int userId);
        bool RemoveProductAccountCodes(int accountCodeId, int productId, int tenantId, int userId);
        ProductLocations AddProductLocationMap(int productId, int locationId);
        List<ProductLocations> AddProductLocationMaps(int productId, int?[] locationIds);
        ProductLocations GetProductLocationMapById(int productLocationId);
        bool RemoveProductLocationMap(int productId, int locationId);
        Dictionary<int, string> GetProductLocationList(long productId, int warehouseId, string code);
        Dictionary<int, string> GetWarehouseLocationList(int warehouseId);
        ProductSCCCodes AddProductSccCodes(ProductSCCCodes code, int tenantId, int userId);
        bool RemoveProductSccCodes(int productId, int codeId, int tenantId, int userId);
        ProductSCCCodes GetProductSccCodesById(int productSccCodeId);
        InventoryStock GetInventoryStockById(int inventoryStockId);
        InventoryStock AddBlankInventoryStock(int productId, int tenantId, int warehouseId, int userId);
        bool IsValidBatchForProduct(int productId, string batchNumber);
        Task<OrderProcessSerialResponse> GetProductInfoBySerial(string productSerial, int tenantId);
        IQueryable<PalletTracking> GetAllPalletTrackings(int tenantId, int warehouseId, DateTime? lastUpdated = null, bool includeArchived = true);
        PalletTrackingSync SavePalletTrackings(PalletTrackingSync pallet, int tenantId);
        PalletTracking GetPalletByPalletSerial(string palletSerial, int tenantId);
        void SaveProductFile(string path, int ProductId, int tenantId, int userId);
        int EditProductFile(ProductFiles productFiles, int tenantId, int userId);
        void RemoveFile(int ProductId, int tenantId, int userId, string filePath);
        IEnumerable<ProductFiles> GetProductFiles(int ProductId, int tenantId, bool sort = false);
        ProductFiles GetProductFilesById(int Id);
        bool SyncDate(int palletTrackingId);

        bool UpdateDontMonitorStockFlagForLocationproducts(int currentTenantId);
        List<Tuple<string, string, decimal, bool>> AllocatedProductDetail(int productId, int WarehouseId, int detail);

        IEnumerable<InventoryTransaction> GetInventoryTransactionsReturns(int productId, int? orderId, string orderNumber, int inventoryTransactionType, string grouptoken);
        IQueryable<ProductMaster> GetAllValidProducts(int tenantId, string args, int OrderId, int departmentId = 0, int groupId = 0, int ProductId = 0);

        IEnumerable<PalletTracking> GetAllPalletByOrderProcessDetailId(int orderprocessdetailId, int tenantId);

        IEnumerable<ProductSerialis> GetAllProductSerialbyOrderProcessDetailId(int orderprocessdetailId, int tenantId, bool? type);

        ProductMaster SaveEditProduct(ProductMaster productMaster, int UserId, int TenantId);
        bool GetInventroyTransactionCountbyOrderProcessDetailId(int orderprocessdetailId, int tenantId);
        IEnumerable<ProductMaster> GetAllProductProcessByPallet(int tenantId);
        string CreatePalletTracking(PalletTracking palletTracking, int NoOfLabels);
    }
}
