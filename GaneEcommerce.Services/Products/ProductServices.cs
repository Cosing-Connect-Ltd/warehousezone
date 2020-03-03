using AutoMapper;
using AutoMapper.Mappers;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ICommonDbServices _commonDbServices;
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public ProductServices(IApplicationContext currentDbContext, ICommonDbServices commonDbServices, IMapper mapper)
        {
            _commonDbServices = commonDbServices;
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public IQueryable<ProductMaster> GetAllValidProductMasters(int tenantId, DateTime? lastUpdated = null, bool includeIsDeleted = false)
        {
            return _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && (includeIsDeleted || a.IsDeleted != true) && (!lastUpdated.HasValue || (a.DateUpdated ?? a.DateCreated) >= lastUpdated));
        }

        public IQueryable<SelectListItem> GetAllValidProductMastersForSelectList(int tenantId, DateTime? lastUpdated = null, bool includeIsDeleted = false)
        {
            return from p in _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && (includeIsDeleted || a.IsDeleted != true) && (!lastUpdated.HasValue || (a.DateUpdated ?? a.DateCreated) >= lastUpdated)).Take(100)
                   select new SelectListItem
                   {
                       Text = p.Name,
                       Value = p.ProductId.ToString()
                   };
        }

        public IQueryable<ProductMaster> GetAllValidProducts(int tenantId, string args, int OrderId, int departmentId = 0, int groupId = 0, int ProductId = 0)
        {

            if (ProductId > 0)
            {
                return _currentDbContext.ProductMaster.Where(u => u.ProductId == ProductId && u.TenantId == tenantId && u.IsDeleted != true);
            }

            var productIds = _currentDbContext.OrderDetail.Where(u => u.OrderID == OrderId && u.TenentId == tenantId && u.IsDeleted != true).Select(u => u.ProductId).ToList();
            return _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && (a.IsDeleted != true)
            && (productIds.Count == 0 || productIds.Contains(a.ProductId)) && (departmentId == 0 || a.DepartmentId == departmentId)
            && (groupId == 0 || a.ProductGroupId == groupId)
            && ((a.SKUCode.Equals(args, StringComparison.CurrentCultureIgnoreCase) || a.SKUCode.Contains(args)) || a.BarCode.Contains(args) || a.Name.Contains(args)));

        }
        public bool SyncDate(int palletTrackingId)
        {
            var palletProduct = _currentDbContext.PalletTracking.FirstOrDefault(u => u.PalletTrackingId == palletTrackingId);
            if (palletProduct != null)
            {
                palletProduct.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(palletProduct).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;

            }
            return true;
        }
        public bool AddOrderId(int? orderId, int palletTrackingId, int? type)
        {
            var palletProduct = _currentDbContext.PalletTracking.FirstOrDefault(u => u.PalletTrackingId == palletTrackingId);
            if (palletProduct != null)
            {
                palletProduct.DateUpdated = DateTime.UtcNow;
                if (type.HasValue)
                {
                    if (type == 1)
                    {
                        palletProduct.Status = PalletTrackingStatusEnum.Hold;
                    }
                    else
                    {
                        palletProduct.Status = PalletTrackingStatusEnum.Active;
                    }

                }
                else
                {

                    palletProduct.OrderId = orderId;
                }

                _currentDbContext.Entry(palletProduct).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;

            }
            return true;
        }
        public PalletTracking GetPalletbyPalletId(int palletTrackingId)
        {
            return _currentDbContext.PalletTracking.FirstOrDefault(u => u.PalletTrackingId == palletTrackingId);

        }
        public IQueryable<PalletTracking> GetAllPalletTrackings(int tenantId, int warehouseId, DateTime? lastUpdated = null, bool includeArchived = true)
        {
            return _currentDbContext.PalletTracking.AsNoTracking().Where(a => a.TenantId == tenantId && (includeArchived || a.Status != PalletTrackingStatusEnum.Archived) && a.WarehouseId == warehouseId
            && (!lastUpdated.HasValue || (a.DateUpdated ?? a.DateCreated) >= lastUpdated));
        }
        public IQueryable<ProductSerialis> GetAllProductSerial(int tenantId, int warehouseId, DateTime? lastUpdated = null)
        {
            return _currentDbContext.ProductSerialization.AsNoTracking().Where(a => a.TenentId == tenantId && a.WarehouseId == warehouseId && (!lastUpdated.HasValue || (a.DateUpdated ?? a.DateCreated) >= lastUpdated));
        }

        public ProductMaster GetProductMasterById(int productId)
        {
            var product = _currentDbContext.ProductMaster.FirstOrDefault(a => a.ProductId == productId && a.IsDeleted != true);
            if (product != null)
            {
                product.RecipeItemProducts = _currentDbContext.ProductReceipeMasters.Where(m => m.ProductMasterID == productId && m.IsDeleted != true).ToList();
                product.ProductKitMap = _currentDbContext.ProductKitMaps.Where(m => m.ProductId == productId && m.IsDeleted != true).ToList();
            }
            return product;
        }



        public IEnumerable<ProductMaster> GetProductMasterDropdown(int productId)
        {
            var product = _currentDbContext.ProductMaster.Where(a => a.ProductId == productId && a.IsDeleted != true);

            return product;
        }

        public IEnumerable<ProductKitMap> GetAllProductInKitsByProductId(int productId)
        {
            return _currentDbContext.ProductKitMaps.Where(a => a.ProductId == productId && a.IsDeleted != true)
                .Distinct().ToList();

            //if (disProdIds.Count > 0)
            //    return _currentDbContext.ProductKitMaps.Where(a => disProdIds.Contains(a.ProductId)).ToList();

            //return new List<ProductKitMap>();
        }

        public IEnumerable<ProductMaster> GetAllProductInKitsByKitProductId(int productId)
        {
            var disProdIds = _currentDbContext.ProductKitMaps.Where(a => a.ProductId == productId && a.IsDeleted != true)
                .Select(a => a.KitProductMaster.ProductId).Distinct().ToList();

            if (disProdIds.Count > 0)
                return _currentDbContext.ProductMaster.Where(a => disProdIds.Contains(a.ProductId)).ToList();

            return new List<ProductMaster>();
        }


        public void SaveSelectedProductRecipeItems(int productId, List<RecipeProductItemRequest> recipeItems, int currentUserId)
        {
            var productMaster = GetProductMasterById(productId);
            var recipeProducts = productMaster.RecipeItemProducts.Where(m => m.IsDeleted != true).Select(m => m.RecipeItemProduct).ToList();

            var removedItems = recipeProducts.Where(m => !recipeItems.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in removedItems)
            {
                var recipeLink = _currentDbContext.ProductReceipeMasters.First(m => m.ProductMasterID == productId && m.RecipeItemProductID == item.ProductId && m.IsDeleted != true);
                recipeLink.IsDeleted = true;
                recipeLink.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(recipeLink).State = EntityState.Modified;
            }

            var addedItems = recipeItems.Where(m => !recipeProducts.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in addedItems)
            {
                var recipeItem = new ProductReceipeMaster
                {
                    ProductMasterID = productId,
                    RecipeItemProductID = item.ProductId,
                    Quantity = item.Quantity
                };
                recipeItem.UpdateCreatedInfo(currentUserId);
                _currentDbContext.Entry(recipeItem).State = EntityState.Added;
            }

            var modifiedItems = productMaster.RecipeItemProducts.Where(m => recipeItems.Select(x => x.ProductId).Contains(m.RecipeItemProductID));
            foreach (var item in modifiedItems)
            {
                var recipeItem = recipeItems.First(m => m.ProductId == item.RecipeItemProductID);
                item.Quantity = recipeItem.Quantity;
                item.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }
        public void SaveSelectedProductKitItems(int productId, List<RecipeProductItemRequest> kitItems, int currentUserId, int tenantId)
        {
            var productMaster = GetProductMasterById(productId);
            var recipeProducts = productMaster.ProductKitMap.Where(m => m.IsDeleted != true).Select(m => m.KitProductMaster).ToList();

            var removedItems = recipeProducts.Where(m => m.TenantId == tenantId && !kitItems.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in removedItems)
            {
                var recipeLink = _currentDbContext.ProductKitMaps.First(m => m.ProductId == productId && m.KitProductId == item.ProductId && m.IsDeleted != true);
                recipeLink.IsDeleted = true;
                recipeLink.UpdatedBy = currentUserId;
                recipeLink.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(recipeLink).State = EntityState.Modified;
            }

            var addedItems = kitItems.Where(m => !recipeProducts.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in addedItems)
            {
                var recipeItem = new ProductKitMap
                {
                    ProductId = productId,
                    KitProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TenantId = tenantId,
                    CreatedBy = currentUserId,
                    DateCreated = DateTime.UtcNow
                };
                _currentDbContext.Entry(recipeItem).State = EntityState.Added;
            }

            var modifiedItems = productMaster.ProductKitMap.Where(m => m.TenantId == tenantId && kitItems.Select(x => x.ProductId).Contains(m.KitProductId));
            foreach (var item in modifiedItems)
            {
                var recipeItem = kitItems.First(m => m.ProductId == item.KitProductId);
                item.Quantity = recipeItem.Quantity;

                item.UpdatedBy = currentUserId;
                item.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }

        public void RemoveRecipeItemProduct(int productId, int recipeItemProductId, int currentUserId)
        {
            var productRecipeItem = _currentDbContext.ProductReceipeMasters.FirstOrDefault(m => m.ProductMasterID == productId && m.RecipeItemProductID == recipeItemProductId && m.IsDeleted != true);

            if (productRecipeItem != null)
            {
                productRecipeItem.IsDeleted = true;
                productRecipeItem.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(productRecipeItem).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }
        public void UpdateRecipeItemProduct(int productId, int recipeItemProductId, decimal quantity, int currentUserId)
        {
            var productRecipeItem = _currentDbContext.ProductReceipeMasters.FirstOrDefault(m => m.ProductMasterID == productId && m.RecipeItemProductID == recipeItemProductId && m.IsDeleted != true);

            if (productRecipeItem != null)
            {
                productRecipeItem.Quantity = quantity;
                productRecipeItem.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(productRecipeItem).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }
        public void RemoveKitItemProduct(int productId, int kitItemProductId, int currentUserId)
        {
            var productRecipeItem = _currentDbContext.ProductKitMaps.FirstOrDefault(m => m.ProductId == productId && m.KitProductId == kitItemProductId && m.IsDeleted != true);

            if (productRecipeItem != null)
            {
                productRecipeItem.IsDeleted = true;
                productRecipeItem.UpdatedBy = currentUserId;
                productRecipeItem.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(productRecipeItem).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }
        public void UpdateKitItemProduct(int productId, int kitItemProductId, decimal quantity, int currentUserId)
        {
            var kitProduct = _currentDbContext.ProductKitMaps.FirstOrDefault(m => m.ProductId == productId && m.KitProductId == kitItemProductId && m.IsDeleted != true);

            if (kitProduct != null)
            {
                kitProduct.Quantity = quantity;
                kitProduct.UpdatedBy = currentUserId;
                kitProduct.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(kitProduct).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }

        public bool MoveStockBetweenLocations(int transactionId, int? locationId, decimal moveQuantity, int tenantId, int warehouseId, int userId)
        {
            bool status = false;
            var cTransaction = _currentDbContext.InventoryTransactions.Find(transactionId);

            cTransaction.IsCurrentLocation = false;
            var cTransactions = _currentDbContext.InventoryTransactions
                .Where(a => a.LocationId == cTransaction.LocationId &&
                            a.ProductId == cTransaction.ProductId && a.InventoryTransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder
                            && a.IsCurrentLocation && a.ProductSerial == null
                            && a.WarehouseId == cTransaction.WarehouseId && a.TenentId == cTransaction.TenentId
                            && a.InventoryTransactionId != cTransaction.InventoryTransactionId).ToList();
            foreach (var item in cTransactions)
            {
                item.IsCurrentLocation = false;
            }

            if (cTransaction.ProductMaster.Serialisable)
            {

                var productSerial = new ProductSerialis
                {
                    ProductId = cTransaction.ProductId,
                    SerialNo = cTransaction.ProductSerial.SerialNo,
                    DateCreated = DateTime.UtcNow,
                    CurrentStatus = InventoryTransactionTypeEnum.PurchaseOrder,
                    CreatedBy = userId,
                    TenentId = tenantId,
                };

                _currentDbContext.ProductSerialization.Add(productSerial);
                _currentDbContext.SaveChanges();


                status = Inventory.StockTransaction(cTransaction.ProductId, (int)InventoryTransactionTypeEnum.PurchaseOrder, 1, cTransaction.OrderID, locationId, null, productSerial.SerialID);

            }
            else
            {
                var cQuantity = _commonDbServices.GetQuantityInLocation(cTransaction);

                status = Inventory.StockTransaction(cTransaction.ProductId, (int)InventoryTransactionTypeEnum.PurchaseOrder, moveQuantity, cTransaction.OrderID, locationId, null, null);


                if ((cQuantity - moveQuantity) > 0)
                {

                    status = Inventory.StockTransaction(cTransaction.ProductId, (int)InventoryTransactionTypeEnum.PurchaseOrder, (cQuantity - moveQuantity), cTransaction.OrderID, locationId, null, null);
                }

            }

            return status;
        }

        public List<OrderPriceViewModel> GetPriceHistoryForProduct(int productId, int tenantId)
        {
            return (from Order in _currentDbContext.Order
                    join s in _currentDbContext.Account on Order.AccountID equals s.AccountID
                    join c in _currentDbContext.GlobalCurrencies on s.CurrencyID equals c.CurrencyID
                    join pd in _currentDbContext.OrderDetail on Order.OrderID equals pd.OrderID
                    where (pd.ProductId == productId && pd.TenentId == tenantId && pd.IsDeleted != true)
                    orderby (s.AccountID)
                    select new OrderPriceViewModel() { ProductId = pd.ProductId, CompanyName = s.CompanyName, Price = pd.Price, CurrencyName = c.CurrencyName, OrderID = Order.OrderID, OrderNumber = Order.OrderNumber, Status = Order.OrderStatus.Status }).ToList();
        }

        public List<ProductAttributes> GetAllProductAttributes()
        {
            return _currentDbContext.ProductAttributes.ToList();
        }

        public ProductAttributeValuesMap GetProductAttributeMapById(int mapId)
        {
            return _currentDbContext.ProductAttributeValuesMap.Find(mapId);
        }

        public ProductAttributeValuesMap UpdateProductAttributeMap(int productId, int mapId)
        {
            var productmaster = _currentDbContext.ProductMaster.Find(productId);

            var map = _currentDbContext.ProductAttributeValuesMap.Find(mapId);

            if (map == null || productmaster == null) return null;

            productmaster.ProductAttributeValuesMap.Add(map);

            _currentDbContext.Entry(productmaster).State = EntityState.Modified;

            _currentDbContext.SaveChanges();

            return map;
        }

        public ProductAttributeValuesMap RemoveProductAttributeMap(int productId, int mapId)
        {
            var productmaster = _currentDbContext.ProductMaster.Find(productId);

            var map = _currentDbContext.ProductAttributeValuesMap.Find(mapId);

            if (map == null || productmaster == null) return null;

            productmaster.ProductAttributeValuesMap.Add(map);

            _currentDbContext.Entry(productmaster).State = EntityState.Deleted;

            _currentDbContext.SaveChanges();

            return map;
        }

        public ProductMaster UpdateProductAttributeMapCollection(int productId, int?[] mapIds)
        {
            var productmaster = _currentDbContext.ProductMaster.Find(productId);
            if (productmaster == null) return null;

            if (mapIds != null)
            {
                for (int i = 0; i < mapIds.Length; i++)
                {
                    var prodaval = GetProductAttributeMapById(mapIds[i].Value);
                    if (prodaval != null)
                    {
                        productmaster.ProductAttributeValuesMap.Add(prodaval);
                    }
                }
                _currentDbContext.SaveChanges();
            }
            return productmaster;
        }

        public List<ProductAttributeValues> GetAllProductAttributeValuesForAttribute(int attributeId)
        {
            return _currentDbContext.ProductAttributeValues.Where(r => r.AttributeId == attributeId).ToList();
        }

        public Dictionary<int, string> GetProductAttributesValues(int productId, int attributeId, string valueText)
        {
            var productmaster = _currentDbContext.ProductMaster.Find(productId);
            var pgoups = productmaster.ProductAttributeValuesMap
                .Where(s => s.ProductAttributeValues.AttributeId == attributeId)
                .Select(s => s.AttributeValueId);

            var pg = from r in _currentDbContext.ProductAttributeValues
                     where (r.AttributeId == attributeId && (r.Value.Contains(valueText) || String.IsNullOrEmpty(valueText)))
                     select new { r.AttributeValueId, r.Value };

            var res = (from p in pg
                       where !(pgoups)
                           .Contains(p.AttributeValueId)
                       select p).ToDictionary(m => m.AttributeValueId, m => m.Value);

            return res;
        }

        public ProductAccountCodes CreateProductAccountCodes(ProductAccountCodes accountCode, int tenantId, int userId)
        {
            var productmaster = GetProductMasterById(accountCode.ProductId ?? 0);
            accountCode.ProdAccCode = accountCode.ProdAccCode.Trim();
            accountCode.ProdDeliveryType = accountCode.ProdDeliveryType;
            accountCode.ProdOrderingNotes = accountCode.ProdOrderingNotes;
            accountCode.DateCreated = DateTime.UtcNow;
            accountCode.DateUpdated = DateTime.UtcNow;
            accountCode.TenantId = tenantId;
            accountCode.CreatedBy = userId;
            accountCode.UpdatedBy = userId;
            accountCode.IsDeleted = false;
            _currentDbContext.ProductAccountCodes.Add(accountCode);
            if (productmaster != null)
            {
                productmaster.DateUpdated = DateTime.UtcNow;
                productmaster.UpdatedBy = userId;
                _currentDbContext.ProductMaster.Attach(productmaster);
                var entry = _currentDbContext.Entry(productmaster);
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
            }
            _currentDbContext.SaveChanges();
            return accountCode;
        }

        public bool RemoveProductAccountCodes(int accountCodeId, int productId, int tenantId, int userId)
        {
            ProductAccountCodes sc = _currentDbContext.ProductAccountCodes.Find(accountCodeId);
            ProductMaster productmaster = _currentDbContext.ProductMaster.Find(productId);
            if (sc == null || productmaster == null)
            {
                return false;
            }
            sc.DateUpdated = DateTime.UtcNow;
            sc.UpdatedBy = userId;
            sc.IsDeleted = true;

            _currentDbContext.ProductAccountCodes.Attach(sc);
            var entry = _currentDbContext.Entry(sc);
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            entry.Property(e => e.IsDeleted).IsModified = true;

            productmaster.DateUpdated = DateTime.UtcNow;
            productmaster.UpdatedBy = userId;
            _currentDbContext.ProductMaster.Attach(productmaster);
            var entry2 = _currentDbContext.Entry(productmaster);
            entry2.Property(e => e.DateUpdated).IsModified = true;
            entry2.Property(e => e.UpdatedBy).IsModified = true;

            _currentDbContext.SaveChanges();

            return true;
        }

        public IEnumerable<int> GetAllProductsInALocationFromMaps(int locationId)
        {
            return _currentDbContext.ProductLocationsMap.Where(a => a.LocationId == locationId && a.IsDeleted != true).Select(a => a.ProductId);
        }

        public IEnumerable<int> GetAllProductLocationsFromMaps(int productId)
        {
            return _currentDbContext.ProductLocationsMap.Where(m => m.ProductId == productId).Select(m => m.LocationId);
        }

        public bool IsCodeAvailableForUse(string code, int tenantId, EnumProductCodeType codeType = EnumProductCodeType.All, int productId = 0)
        {
            var isDuplicateBarcodeAllowed = _currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == tenantId && x.IsDeleted != true).AllowDuplicateBarcode;

            var matchingProduct = _currentDbContext.ProductMaster
                .FirstOrDefault(x => x.IsDeleted != true && x.TenantId == tenantId &&
                          (codeType == EnumProductCodeType.All
                          ||
                          (codeType == EnumProductCodeType.SkuCode && x.SKUCode.Equals(code, StringComparison.CurrentCultureIgnoreCase)))
                          ||
                          (codeType == EnumProductCodeType.Barcode && isDuplicateBarcodeAllowed == false && x.BarCode.Equals(code, StringComparison.CurrentCultureIgnoreCase)));
            if (matchingProduct == null)
            {
                return true;
            }
            else
            {
                return matchingProduct.ProductId == productId;
            }
        }

        public bool IsNameAvailableForUse(string Name, int tenantId, EnumProductCodeType codeType = EnumProductCodeType.All, int productId = 0)
        {
            var isProductNameAllowed = _currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == tenantId && x.IsDeleted != true).AllowDuplicateProductName;

            var matchingProduct = _currentDbContext.ProductMaster
                .FirstOrDefault(x => x.IsDeleted != true && x.TenantId == tenantId && (codeType == EnumProductCodeType.All && isProductNameAllowed == false && x.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)));
            if (matchingProduct == null)
            {
                return true;
            }
            else
            {
                return matchingProduct.ProductId == productId;
            }
        }

        public ProductMaster SaveProduct(ProductMaster productMaster, List<string> productAccountCodeIds,
            List<int> productAttributesIds,
            List<int> productLocationIds, List<int> productKitIds, int userId, int tenantId, List<int> SiteId)
        {
            if (productMaster.ProductId > 0)
            {
                productMaster.UpdateCreatedInfo(userId);
                productMaster.UpdateUpdatedInfo(userId);
                productMaster.TenantId = tenantId;
                _currentDbContext.Entry(productMaster).State = EntityState.Modified;
                if (productAccountCodeIds == null)
                {
                    foreach (var entity in _currentDbContext.ProductAccountCodes.Where(x => x.ProductId == productMaster.ProductId))
                    {
                        entity.IsDeleted = true;
                        _currentDbContext.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    var ToDelete = new List<int>();
                    var parsedProductAccountCodeIds = new List<int>();

                    foreach (var item in productAccountCodeIds)
                    {
                        int AccountCodeId = Convert.ToInt32(item.ToString());
                        var accountToAdd = _currentDbContext.ProductAccountCodes.FirstOrDefault(u => u.ProdAccCodeID == AccountCodeId);
                        if (accountToAdd != null)
                        {
                            accountToAdd.ProductId = productMaster.ProductId;
                            parsedProductAccountCodeIds.Add(int.Parse(item.ToString()));
                        }

                        _currentDbContext.Entry(accountToAdd).State = EntityState.Modified;
                    }

                    ToDelete = _currentDbContext.ProductAccountCodes
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.ProdAccCodeID)
                        .ToList()
                        .Except(parsedProductAccountCodeIds)
                        .ToList();
                    foreach (int item in ToDelete)
                    {
                        var Current = _currentDbContext.ProductAccountCodes
                            .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.ProdAccCodeID == item &&
                                                 x.IsDeleted != true);
                        Current.IsDeleted = true;
                        _currentDbContext.Entry(Current).State = EntityState.Modified;
                    }

                }
                if (productAttributesIds == null)
                {
                    foreach (var entity in _currentDbContext.ProductAttributeValuesMap.Where(
                        x => x.ProductId == productMaster.ProductId))
                    {
                        entity.IsDeleted = true;
                        _currentDbContext.Entry(entity).State = EntityState.Modified;
                    }

                }
                else
                {
                    var ToDelete = new List<int>();
                    ToDelete = _currentDbContext.ProductAttributeValuesMap
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.AttributeValueId)
                        .ToList()
                        .Except(productAttributesIds)
                        .ToList();
                    var ToAdd = new List<int>();
                    ToAdd = productAttributesIds
                        .Except(_currentDbContext.ProductAttributeValuesMap
                            .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                            .Select(x => x.AttributeValueId)
                            .ToList())
                        .ToList();

                    foreach (var item in ToDelete)
                    {
                        var Current = _currentDbContext.ProductAttributeValuesMap
                            .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.AttributeValueId == item &&
                                                 x.IsDeleted != true);
                        Current.IsDeleted = true;
                        _currentDbContext.Entry(Current).State = EntityState.Modified;
                    }
                    foreach (var item in ToAdd)
                    {
                        var newItem = new ProductAttributeValuesMap()
                        {
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            AttributeValueId = item,
                            TenantId = tenantId,
                            ProductId = productMaster.ProductId,
                        };
                        _currentDbContext.ProductAttributeValuesMap.Add(newItem);
                    }

                }

                if (productLocationIds == null)
                {
                    foreach (var entity in _currentDbContext.ProductLocationsMap.Where(
                        x => x.ProductId == productMaster.ProductId))
                    {
                        entity.IsDeleted = true;
                        _currentDbContext.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    var ToDelete = new List<int>();
                    ToDelete = _currentDbContext.ProductLocationsMap
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.LocationId)
                        .ToList()
                        .Except(productLocationIds)
                        .ToList();
                    var ToAdd = new List<int>();
                    ToAdd = productLocationIds
                        .Except(_currentDbContext.ProductLocationsMap
                            .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                            .Select(x => x.LocationId)
                            .ToList())
                        .ToList();

                    foreach (int item in ToDelete)
                    {
                        var Current = _currentDbContext.ProductLocationsMap
                            .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.LocationId == item &&
                                                 x.IsDeleted != true);
                        Current.IsDeleted = true;
                        _currentDbContext.Entry(Current).State = EntityState.Modified;
                    }
                    foreach (int item in ToAdd)
                    {
                        var newItem = new ProductLocations()
                        {
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            LocationId = item,
                            TenantId = tenantId,
                            ProductId = productMaster.ProductId,
                        };
                        _currentDbContext.ProductLocationsMap.Add(newItem);
                    }
                }


                if (productMaster.Kit && productKitIds != null)
                {
                    var ToDelete = new List<int>();

                    ToDelete = _currentDbContext.ProductKitMaps
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.KitProductId)
                        .ToList()
                        .Except(productKitIds)
                        .ToList();

                    var ToAdd = new List<int>();
                    ToAdd = productKitIds
                        .Except(_currentDbContext.ProductKitMaps
                            .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                            .Select(x => x.KitProductId)
                            .ToList())
                        .ToList();

                    foreach (var item in ToDelete)
                    {
                        var Current = _currentDbContext.ProductKitMaps
                            .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.KitProductId == item &&
                                                 x.IsDeleted != true);
                        Current.IsDeleted = true;
                        _currentDbContext.Entry(Current).State = EntityState.Modified;
                    }
                    foreach (var item in ToAdd)
                    {
                        var newItem = new ProductKitMap()
                        {
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            KitProductId = item,
                            ProductId = productMaster.ProductId,
                            TenantId = tenantId,
                            Quantity = 1
                        };
                        _currentDbContext.ProductKitMaps.Add(newItem);
                    }

                }
                else
                {
                    foreach (var entity in _currentDbContext.ProductKitMaps.Where(
                        x => x.ProductId == productMaster.ProductId))
                    {
                        entity.IsDeleted = true;
                        _currentDbContext.Entry(entity).State = EntityState.Modified;
                    }
                    productMaster.Kit = false;
                }




                _currentDbContext.SaveChanges();
            }
            else
            {
                productMaster.UpdateCreatedInfo(userId);
                productMaster.TenantId = tenantId;
                _currentDbContext.ProductMaster.Add(productMaster);
                _currentDbContext.SaveChanges();

                if (productAccountCodeIds != null)
                {
                    foreach (var item in productAccountCodeIds)
                    {
                        int AccountCodeId = Convert.ToInt32(item.ToString());
                        var accountToAdd = _currentDbContext.ProductAccountCodes.FirstOrDefault(u => u.ProdAccCodeID == AccountCodeId);
                        if (accountToAdd != null)
                        {
                            accountToAdd.ProductId = productMaster.ProductId;

                        }

                        _currentDbContext.Entry(accountToAdd).State = EntityState.Modified;
                    }
                }

                if (productAttributesIds != null)
                {
                    foreach (var item in productAttributesIds)
                    {
                        var attributeMap = new ProductAttributeValuesMap
                        {
                            AttributeValueId = item,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            ProductId = productMaster.ProductId,
                            TenantId = tenantId,
                        };
                        _currentDbContext.ProductAttributeValuesMap.Add(attributeMap);
                    }

                }
                if (productLocationIds != null)
                {
                    foreach (var item in productLocationIds)
                    {
                        var pLocation = new ProductLocations
                        {
                            CreatedBy = userId,
                            LocationId = item,
                            DateCreated = DateTime.UtcNow,
                            ProductId = productMaster.ProductId,
                            TenantId = tenantId
                        };

                        _currentDbContext.ProductLocationsMap.Add(pLocation);
                    }

                }
                if (productKitIds != null && productMaster.Kit)
                {
                    foreach (var item in productKitIds)
                    {
                        var pKit = new ProductKitMap
                        {
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            ProductId = productMaster.ProductId,
                            KitProductId = item,
                            TenantId = tenantId
                        };

                        _currentDbContext.ProductKitMaps.Add(pKit);

                    }

                }
                _currentDbContext.SaveChanges();
            }

            if (SiteId != null && SiteId.Count > 0)
            {
                var isdeletedList = _currentDbContext.ProductsWebsitesMap.Where(u => u.ProductId == productMaster.ProductId && !SiteId.Contains(u.SiteID) && u.IsDeleted != true).ToList();
                if (isdeletedList.Count > 0)
                {
                    isdeletedList.ForEach(u => u.IsDeleted = true);
                    _currentDbContext.SaveChanges();
                }

                foreach (var item in SiteId)
                {
                    int siteId = Convert.ToInt32(item);
                    var websiteslist = _currentDbContext.ProductsWebsitesMap.FirstOrDefault(u => u.ProductId == productMaster.ProductId && u.SiteID == siteId && u.IsDeleted != true);
                    if (websiteslist != null)
                    {
                        websiteslist.TenantId = tenantId;
                        websiteslist.UpdateUpdatedInfo(userId);
                    }
                    else
                    {

                        var list = new ProductsWebsitesMap();
                        list.ProductId = productMaster.ProductId;
                        list.SiteID = siteId;
                        list.UpdateCreatedInfo(userId);
                        productMaster.ProductsWebsitesMap.Add(list);
                    }

                }
                _currentDbContext.SaveChanges();
            }
            else
            {
                var isdeletedList = _currentDbContext.ProductsWebsitesMap.Where(u => u.ProductId == productMaster.ProductId && u.IsDeleted != true).ToList();
                if (isdeletedList.Count > 0)
                {
                    isdeletedList.ForEach(u => u.IsDeleted = true);
                    _currentDbContext.SaveChanges();
                }
            }
            return productMaster;
        }

        public void SaveProductFile(string path, int ProductId, int tenantId, int userId)
        {
            ProductFiles productFiles = new ProductFiles();
            productFiles.FilePath = path;
            productFiles.ProductId = ProductId;
            productFiles.TenantId = tenantId;
            productFiles.CreatedBy = userId;
            productFiles.DateCreated = DateTime.UtcNow;
            _currentDbContext.ProductFiles.Add(productFiles);
            _currentDbContext.SaveChanges();


        }
        public int EditProductFile(ProductFiles productFiles, int tenantId, int userId)
        {
            var productFile = _currentDbContext.ProductFiles.FirstOrDefault(u => u.Id == productFiles.Id);
            if (productFile != null)
            {
                productFile.SortOrder = productFiles.SortOrder;
                productFile.HoverImage = productFiles.HoverImage;
                productFile.DefaultImage = productFiles.DefaultImage;
                productFile.IsDeleted = productFiles.IsDeleted;
                productFiles.UpdatedBy = userId;
                productFiles.DateUpdated = DateTime.UtcNow;
                _currentDbContext.ProductFiles.Attach(productFile);
                _currentDbContext.Entry(productFile).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return productFile.ProductId;
            }
            return 0;
        }

        public void RemoveFile(int ProductId, int tenantId, int userId, string filePath)
        {
            var productFiles = _currentDbContext.ProductFiles.FirstOrDefault(u => u.ProductId == ProductId && u.TenantId == tenantId && u.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));
            if (productFiles != null)
            {

                productFiles.IsDeleted = true;
                productFiles.UpdatedBy = userId;
                productFiles.DateUpdated = DateTime.UtcNow;
                _currentDbContext.ProductFiles.Attach(productFiles);
                _currentDbContext.Entry(productFiles).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
        }

        public IEnumerable<ProductFiles> GetProductFiles(int ProductId, int tenantId, bool sort = false)
        {
            if (sort)
            {
                return _currentDbContext.ProductFiles.Where(u => u.ProductId == ProductId && u.TenantId == tenantId && u.IsDeleted != true).OrderBy(u => u.SortOrder);
            }
            return _currentDbContext.ProductFiles.Where(u => u.ProductId == ProductId && u.TenantId == tenantId && u.IsDeleted != true);


        }

        public LocationGroup SaveLocationGroup(string locationGroupName, int userId, int tenantId)
        {
            var plocation = new LocationGroup
            {
                IsActive = true,
                DateUpdated = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
                IsDeleted = false,
                Locdescription = locationGroupName,
                TenentId = tenantId,
                UpdatedBy = userId
            };
            _currentDbContext.Entry(plocation).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return plocation;
        }

        public ProductKitMap UpdateProductKitMap(int productId, int kitProductId, int userId, int tenantId)
        {
            var map = new ProductKitMap
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                KitProductId = kitProductId,
                ProductId = productId,
                TenantId = tenantId,
            };

            _currentDbContext.Entry(map).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return map;
        }

        public ProductMaster SaveEditProduct(ProductMaster productMaster, int UserId, int TenantId)
        {
            if (productMaster.ProductId > 0)
            {
                var products = _currentDbContext.ProductMaster.FirstOrDefault(u => u.ProductId == productMaster.ProductId);
                if (products != null)
                {
                    products.UpdateUpdatedInfo(UserId);
                    products.Name = string.IsNullOrEmpty(productMaster.Name) ? products.Name : productMaster.Name;
                    products.SKUCode = string.IsNullOrEmpty(productMaster.SKUCode) ? products.SKUCode : productMaster.SKUCode;
                    products.Serialisable = productMaster.Serialisable;
                    products.ProcessByPallet = productMaster.ProcessByPallet;
                    products.TopProduct = productMaster.TopProduct;
                    products.BestSellerProduct = productMaster.BestSellerProduct;
                    products.SpecialProduct = productMaster.SpecialProduct;
                    products.OnSaleProduct = productMaster.OnSaleProduct;

                }
                products.TenantId = TenantId;
                _currentDbContext.Entry(products).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }

            return productMaster;
        }



        public void SaveProductSerials(List<string> serialList, int product, string delivery, int order, int location, int tenantId, int warehouseId, int userId)
        {
            var _orderService = DependencyResolver.Current.GetService<IOrderService>();
            //TODO: inventory type should be passed to create order process
            var oprocess = _orderService.GetOrderProcessByDeliveryNumber(order, 0, delivery, userId, warehouseId: warehouseId);
            OrderProcessDetail odet = new OrderProcessDetail
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                OrderProcessId = oprocess.OrderProcessID,
                ProductId = product,
                TenentId = tenantId,
                QtyProcessed = serialList.Count,
            };

            _currentDbContext.OrderProcessDetail.Add(odet);
            _currentDbContext.SaveChanges();

            foreach (var item in serialList)
            {
                ProductSerialis serial = new ProductSerialis
                {
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    SerialNo = item,
                    TenentId = tenantId,
                    ProductId = product,
                    CurrentStatus = InventoryTransactionTypeEnum.PurchaseOrder

                };

                _currentDbContext.ProductSerialization.Add(serial);
                _currentDbContext.SaveChanges();

                Inventory.StockTransaction(product, (int)InventoryTransactionTypeEnum.PurchaseOrder, 1, order, location, delivery, serial.SerialID, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));

            }

        }

        public void DeleteProductsAndKits(int productId, int kitProductId, int tenantId, int userId)
        {
            var productMaster = GetProductMasterById(productId);
            productMaster.IsDeleted = true;
            productMaster.UpdateUpdatedInfo(userId);

            var relatedKits = _currentDbContext.ProductKitMaps.Where(a => a.KitProductId == productId && a.ProductId == kitProductId && a.TenantId == tenantId);
            foreach (var kit in relatedKits)
            {
                kit.IsDeleted = true;
                kit.UpdatedBy = userId;
                kit.DateUpdated = DateTime.UtcNow;
            }

            _currentDbContext.Entry(productMaster).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }


        public IEnumerable<ProductAccountCodes> GetAllProductAccountCodesByProductId(int productId)
        {
            return _currentDbContext.ProductAccountCodes.Where(a => a.IsDeleted != true && a.ProductId == productId);
        }

        public InventoryStock GetInventoryStockByProductTenantLocation(int productId, int warehouseId)
        {
            return _currentDbContext.InventoryStocks.FirstOrDefault(a => a.ProductId == productId && a.IsDeleted != true && warehouseId == a.WarehouseId);
        }

        public List<InventoryStock> GetAllInventoryStocksByProductId(int productId)
        {
            return _currentDbContext.InventoryStocks.Where(a => a.ProductId == productId && a.IsDeleted != true).ToList();
        }

        public InventoryStock GetInventoryStocksByProductAndTenantLocation(int productId, int tenantLocationId)
        {
            return _currentDbContext.InventoryStocks.FirstOrDefault(a => a.ProductId == productId && a.WarehouseId == tenantLocationId && a.IsDeleted != true);
        }

        public IEnumerable<ProductMaster> GetProductByCategory(int tenantId, int NumberofProducts, bool TopProduct = false, bool BestSellerProduct = false, bool SpecialProduct = false, bool OnSaleProduct = false)
        {
            if (TopProduct)
            {
                return _currentDbContext.ProductMaster.Where(u => u.TenantId == tenantId && u.TopProduct == TopProduct && u.IsDeleted != true).Take(NumberofProducts);
            }
            else if (BestSellerProduct)
            {
                return _currentDbContext.ProductMaster.Where(u => u.TenantId == tenantId && u.BestSellerProduct == BestSellerProduct && u.IsDeleted != true).Take(NumberofProducts);
            }
            else if (SpecialProduct)
            {
                return _currentDbContext.ProductMaster.Where(u => u.TenantId == tenantId && u.SpecialProduct == SpecialProduct && u.IsDeleted != true).Take(NumberofProducts);
            }
            else if (OnSaleProduct)
            {
                return _currentDbContext.ProductMaster.Where(u => u.TenantId == tenantId && u.OnSaleProduct == OnSaleProduct && u.IsDeleted != true).Take(NumberofProducts);
            }
            else
            {
                return _currentDbContext.ProductMaster.Where(u => u.TenantId == tenantId && u.IsDeleted != true).Take(NumberofProducts);
            }
        }
        public IQueryable<InventoryStock> GetAllInventoryStocks(int tenantId, int warehouseId, DateTime? reqDate = null)
        {
            var model = _currentDbContext.InventoryStocks.Include(x => x.ProductMaster)
                .Where(x => x.TenantId == tenantId && x.WarehouseId == warehouseId &&
                            x.ProductMaster.IsDeleted != true && x.ProductMaster.DontMonitorStock != true && (!reqDate.HasValue || (x.DateUpdated ?? x.DateCreated) >= reqDate));

            return model;
        }

        public IQueryable<InventoryStockViewModel> GetAllInventoryStocksList(int tenantId, int warehouseId, int filterByProductId = 0)
        {
            IQueryable<InventoryStockViewModel> results;
            if (filterByProductId > 0)
            {
                results = (from i in _currentDbContext.InventoryStocks
                           join p in _currentDbContext.ProductMaster on i.ProductId equals p.ProductId
                           join w in _currentDbContext.TenantWarehouses on i.WarehouseId equals w.WarehouseId
                           where i.TenantId == tenantId && i.ProductId == filterByProductId
                               && (warehouseId == 0 || i.WarehouseId == warehouseId || i.TenantWarehous.ParentWarehouseId == warehouseId)
                           orderby w.WarehouseName
                           select new InventoryStockViewModel
                           {
                               Allocated = i.Allocated,
                               Available = i.Available,
                               InStock = i.InStock,
                               OnOrder = i.OnOrder,
                               Barcode = p.BarCode,
                               ProductId = i.ProductId,
                               ProductName = p.Name,
                               WarehouseId = warehouseId,
                               SkuCode = p.SKUCode,
                               WarehouseName = w.WarehouseName,
                               ProductGroup = p.ProductGroup.ProductGroup ?? null,
                               DepartmentName = p.TenantDepartment.DepartmentName ?? null,
                               PalletProduct = p.ProcessByPallet,
                               SerialProduct = p.Serialisable
                           });
            }
            else
            {
                var currentWarehouse = _currentDbContext.TenantWarehouses.FirstOrDefault(m => m.WarehouseId == warehouseId);
                if (warehouseId == 0 || (currentWarehouse != null && currentWarehouse.IsMobile != true))
                {
                    results = (from p in _currentDbContext.ProductMaster
                               join i in _currentDbContext.InventoryStocks
                               on p.ProductId equals i.ProductId
                               join w in _currentDbContext.TenantWarehouses on i.WarehouseId equals w.WarehouseId
                               where p.IsDeleted != true && i.TenantId == tenantId && (warehouseId == 0 || i.WarehouseId == warehouseId || i.TenantWarehous.ParentWarehouseId == warehouseId)
                               orderby w.WarehouseName
                               group new { p, i, w } by new { p.ProductId } into gq
                               select new InventoryStockViewModel()
                               {
                                   Allocated = gq.Sum(u => u.i.Allocated),
                                   Available = gq.Sum(u => u.i.Available),
                                   InStock = gq.Sum(u => u.i.InStock),
                                   OnOrder = gq.Sum(u => u.i.OnOrder),
                                   Barcode = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.BarCode,
                                   ProductId = gq.Key.ProductId,
                                   ProductName = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.Name,
                                   SkuCode = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.SKUCode,
                                   ProductGroup = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.ProductGroup.ProductGroup ?? null,
                                   DepartmentName = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.TenantDepartment.DepartmentName ?? null,
                                   WarehouseId = warehouseId,
                                   WarehouseName = gq.FirstOrDefault(u => u.i.ProductId == gq.Key.ProductId).i.TenantWarehous.WarehouseName ?? null,
                                   PalletProduct = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.ProcessByPallet,
                                   SerialProduct = gq.FirstOrDefault(u => u.p.ProductId == gq.Key.ProductId).p.Serialisable
                               });


                }
                else
                {

                    results = (from p in _currentDbContext.ProductMaster
                               join i in _currentDbContext.InventoryStocks
                               on p.ProductId equals i.ProductId
                               join w in _currentDbContext.TenantWarehouses on i.WarehouseId equals w.WarehouseId
                               where p.IsDeleted != true && i.TenantId == tenantId && (warehouseId == 0 || i.WarehouseId == warehouseId || i.TenantWarehous.ParentWarehouseId == warehouseId)
                               orderby w.WarehouseName
                               select new InventoryStockViewModel
                               {
                                   Allocated = i.Allocated,
                                   Available = i.Available,
                                   InStock = i.InStock,
                                   OnOrder = i.OnOrder,
                                   Barcode = p.BarCode,
                                   ProductId = i.ProductId,
                                   ProductName = p.Name,
                                   WarehouseId = warehouseId,
                                   SkuCode = p.SKUCode,
                                   WarehouseName = w.WarehouseName,
                                   ProductGroup = p.ProductGroup.ProductGroup ?? null,
                                   DepartmentName = p.TenantDepartment.DepartmentName ?? null,
                                   PalletProduct = p.ProcessByPallet,
                                   SerialProduct = p.Serialisable
                               });
                }
            }
            return results;
        }

        public IQueryable<InventoryTransaction> GetAllInventoryTransactions(int tenantId, int warehouseId)
        {
            return _currentDbContext.InventoryTransactions.AsNoTracking()
                .Where(x => x.TenentId == tenantId && (x.WarehouseId == warehouseId || x.TenantWarehouse.ParentWarehouseId == warehouseId) && x.ProductMaster.IsDeleted != true && x.IsDeleted != true)
                .Include(x => x.ProductMaster)
                .Include(x => x.ProductMaster.InventoryStocks)
                .Include(x => x.ProductSerial)
                .Include(x => x.Order)
                .Include(x => x.Order.PProperties)
                .Include(x => x.InventoryTransactionType).OrderByDescending(m => m.DateCreated);
        }

        public IEnumerable<InventoryTransaction> GetAllInventoryTransactionsByProductId(int productId, int warehouseId)
        {
            return _currentDbContext.InventoryTransactions.Where(a => a.ProductId == productId && a.IsDeleted != true && a.WarehouseId == warehouseId).Include(x => x.InventoryTransactionType);
        }


        public IEnumerable<StockTakeSnapshot> GetAllStockTakeSnapshotsByProductId(int productId)
        {
            return _currentDbContext.StockTakeSnapshot.Where(a => a.ProductId == productId && a.IsDeleted != true).ToList();
        }

        public InventoryTransaction GetInventoryTransactionById(int transactionId)
        {
            return _currentDbContext.InventoryTransactions.Find(transactionId);
        }

        public void DeleteInventoryTransactionById(int transactionId)
        {
            var trans = GetInventoryTransactionById(transactionId);
            trans.IsDeleted = true;
            _currentDbContext.SaveChanges();
        }

        public IQueryable<InventoryTransaction> GetInventoryTransactionsByPalletTrackingId(int Id)
        {
            return _currentDbContext.InventoryTransactions.Where(x => x.PalletTrackingId == Id && x.IsDeleted != true);
        }
        public decimal GetInventoryTransactionsByPalletTrackingId(int PalletTrackingId, int OrderProcessDetailId)
        {
            var purchaseOrdersTransactios = _currentDbContext.InventoryTransactions.Where(x => x.PalletTrackingId == PalletTrackingId
             && x.OrderProcessDetailId == OrderProcessDetailId &&
             x.IsDeleted != true && (x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentIn || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns
                 || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferIn)).Select(u => u.Quantity).DefaultIfEmpty(0).Sum();

            var salesOrdersTransactios = _currentDbContext.InventoryTransactions.Where(x => x.PalletTrackingId == PalletTrackingId
             && x.OrderProcessDetailId == OrderProcessDetailId &&
             x.IsDeleted != true && (x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentOut || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.DirectSales || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan
                || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferOut
                || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder || x.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Wastage)).Select(u => u.Quantity).DefaultIfEmpty(0).Sum();
            if (purchaseOrdersTransactios > salesOrdersTransactios)
            {
                return (purchaseOrdersTransactios - salesOrdersTransactios);
            }
            else
            {
                return (salesOrdersTransactios - purchaseOrdersTransactios);
            }

        }
        public IQueryable<InventoryTransaction> GetInventoryTransactionsByProductSerialId(int Id)
        {
            return _currentDbContext.InventoryTransactions.Where(x => x.SerialID == Id && x.IsDeleted != true);
        }


        public IEnumerable<InventoryTransaction> GetInventoryTransactionsReturns(int productId, int? orderId, string orderNumber, int inventoryTransactionType, string grouptoken)
        {
            return _currentDbContext.InventoryTransactions.Where(x => x.InventoryTransactionRef.Equals(grouptoken, StringComparison.CurrentCulture));
        }



        public IEnumerable<ProductSerialis> GetAllProductSerialsByProductId(int productId, int warehouseId)
        {
            return _currentDbContext.ProductSerialization.Where(a => a.ProductId == productId);
        }

        public IEnumerable<ProductSerialis> GetAllProductSerialsByTenantId(int tenantId, DateTime? reqDate = null)
        {
            return _currentDbContext.ProductSerialization.Where(a => a.TenentId == tenantId && (!reqDate.HasValue || (a.DateUpdated ?? a.DateCreated) >= reqDate));
        }


        public ProductSerialis GetProductSerialBySerialCode(string serialCode, int tenantId, bool inStockOnly = false)
        {
            return _currentDbContext.ProductSerialization.FirstOrDefault(a => (a.TenentId == tenantId && a.SerialNo.Equals(serialCode, StringComparison.CurrentCultureIgnoreCase)) && (!inStockOnly || a.CurrentStatus == InventoryTransactionTypeEnum.PurchaseOrder));
        }

        public ProductSerialis SaveProductSerial(ProductSerialis serial, int userId)
        {
            if (serial.SerialID > 0)
            {
                var item = _currentDbContext.ProductSerialization.Find(serial.SerialID);
                if (item != null)
                {

                    InventoryTransactionTypeEnum[] inStockStatus = { InventoryTransactionTypeEnum.PurchaseOrder, InventoryTransactionTypeEnum.TransferIn, InventoryTransactionTypeEnum.AdjustmentIn, InventoryTransactionTypeEnum.Returns };
                    if (!inStockStatus.Contains(serial.CurrentStatus))
                    {
                        serial.CurrentStatus = InventoryTransactionTypeEnum.AdjustmentIn;
                    }
                    serial.UpdatedBy = userId;
                    serial.DateUpdated = DateTime.UtcNow;
                    _currentDbContext.Entry(serial).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }
            else
            {
                serial.CreatedBy = userId;
                serial.DateCreated = DateTime.UtcNow;
                _currentDbContext.Entry(serial).State = EntityState.Added;
                _currentDbContext.SaveChanges();
            }
            return serial;
        }

        public ProductMaster GetProductMasterByProductCode(string productCode, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.IsDeleted != true && (e.SKUCode.Equals(productCode, StringComparison.CurrentCultureIgnoreCase)
            || e.BarCode.Equals(productCode, StringComparison.CurrentCultureIgnoreCase) || e.ManufacturerPartNo.Equals(productCode, StringComparison.CurrentCultureIgnoreCase)));
        }
        public ProductMaster GetProductMasterByOuterBarcode(string outerBarcode, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.BarCode2.Equals(outerBarcode, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public ProductMaster GetProductMasterByBarcode(string barcode, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.BarCode.Equals(barcode, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public string GenerateNextProductCode(int tenantId)
        {
            var tenant = _currentDbContext.Tenants.Find(tenantId);
            if (tenant != null && tenant.ProductCodePrefix != null)
            {
                var product = _currentDbContext.ProductMaster.Where(m => m.SKUCode.Contains(tenant.ProductCodePrefix) && m.TenantId == tenantId).OrderByDescending(m => m.SKUCode).FirstOrDefault();
                if (product != null)
                {
                    int ValidSkuCode = 0;
                    var lastCode = product.SKUCode.Split(new[] { tenant.ProductCodePrefix, tenant.ProductCodePrefix.ToLower() }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (int.TryParse(lastCode, out ValidSkuCode))
                    {
                        if (lastCode != null)
                        {

                            return tenant.ProductCodePrefix + (int.Parse(lastCode) + 1).ToString("00000");
                        }
                    }
                }
                return tenant.ProductCodePrefix + "00001";
            }

            return "ITM-100001";
        }

        public IQueryable<ProductMasterViewModel> GetAllProductMasterDetail(int tenantId, int warehouseId)
        {
            var model = _currentDbContext.ProductMaster.AsNoTracking().Where(x => x.TenantId == tenantId && x.IsDeleted != true)
                .Select(prd => new ProductMasterViewModel
                {
                    ProductId = prd.ProductId,
                    Name = prd.Name,
                    SKUCode = prd.SKUCode,
                    Description = prd.Description,
                    BarCode = prd.BarCode,
                    Serialisable = prd.Serialisable,
                    IsStockItem = prd.IsStockItem,
                    IsRawMaterial = prd.IsRawMaterial,
                    UOM = prd.GlobalUOM.UOM,
                    Kit = prd.Kit,
                    BarCode2 = prd.BarCode2,
                    ShelfLifeDays = prd.ShelfLifeDays,
                    ReorderQty = prd.ReorderQty,
                    ShipConditionCode = prd.ShipConditionCode,
                    CommodityCode = prd.CommodityCode,
                    CommodityClass = prd.CommodityClass,
                    Height = prd.Height,
                    Width = prd.Width,
                    Depth = prd.Depth,
                    Weight = prd.Weight,
                    PercentMargin = prd.PercentMargin,
                    LotOptionDescription = prd.ProductLotOptionsCodes.Description,
                    LotOption = prd.LotOption,
                    Discontinued = prd.Discontinued,
                    GlobalWeightGrpDescription = prd.GlobalWeightGroups.Description,
                    TaxName = prd.GlobalTax.TaxName,
                    ProdStartDate = prd.ProdStartDate,
                    ProductLotProcessTypeCodesDescription = prd.ProductLotProcessTypeCodes.Description,
                    Available = prd.InventoryStocks.Where(x => x.ProductId == prd.ProductId && (x.WarehouseId == warehouseId || x.TenantWarehous.ParentWarehouseId == warehouseId)).Select(x => x.Available).DefaultIfEmpty(0).Sum(),
                    Allocated = prd.InventoryStocks.Where(x => x.ProductId == prd.ProductId && (x.WarehouseId == warehouseId || x.TenantWarehous.ParentWarehouseId == warehouseId)).Select(x => x.Allocated).DefaultIfEmpty(0).Sum(),
                    InStock = prd.InventoryStocks.Where(x => x.ProductId == prd.ProductId && (x.WarehouseId == warehouseId || x.TenantWarehous.ParentWarehouseId == warehouseId)).Select(x => x.InStock).DefaultIfEmpty(0).Sum(),
                    OnOrder = prd.InventoryStocks.Where(x => x.ProductId == prd.ProductId && (x.WarehouseId == warehouseId || x.TenantWarehous.ParentWarehouseId == warehouseId)).Select(x => x.OnOrder).DefaultIfEmpty(0).Sum(),
                    ProductGroupName = prd.ProductGroup.ProductGroup ?? null,
                    DepartmentName = prd.TenantDepartment.DepartmentName,
                    Location = prd.ProductLocationsMap.Where(a => a.IsDeleted != true).Select(x => x.Locations.LocationCode).FirstOrDefault().ToString(),
                    EnableWarranty = prd.EnableWarranty ?? false,
                    EnableTax = prd.EnableTax ?? false,
                    DontMonitorStock = prd.DontMonitorStock,
                    ProcessByPallet = prd.ProcessByPallet,
                    OnSaleProduct = prd.OnSaleProduct,
                    BestSellerProduct = prd.BestSellerProduct,
                    TopProduct = prd.TopProduct,
                    SpecialProduct = prd.SpecialProduct
                }).OrderBy(x => x.Name);

            return model;
        }

        public ProductLocations AddProductLocationMap(int productId, int locationId)
        {
            ProductMaster productmaster = GetProductMasterById(productId);

            ProductLocations location = _currentDbContext.ProductLocationsMap.Find(locationId);

            if (productmaster == null || location == null)
            {
                return null;
            }

            productmaster.ProductLocationsMap.Add(location);

            _currentDbContext.SaveChanges();

            return location;
        }

        public List<ProductLocations> AddProductLocationMaps(int productId, int?[] locationIds)
        {
            if (productId == 0)
            {
                return null;
            }

            ProductMaster productmaster = _currentDbContext.ProductMaster.Find(productId);
            if (productmaster == null)
            {
                return null;
            }

            if (locationIds != null)
            {
                for (int i = 0; i < locationIds.Length; i++)
                {
                    ProductLocations loc = _currentDbContext.ProductLocationsMap.Find(locationIds[i]);
                    if (loc != null)
                    {
                        productmaster.ProductLocationsMap.Add(loc);
                    }
                }

                _currentDbContext.SaveChanges();
            }

            return productmaster.ProductLocationsMap.ToList();
        }

        public bool RemoveProductLocationMap(int productId, int locationId)
        {
            ProductLocations loc = GetProductLocationMapById(locationId);

            ProductMaster productmaster = GetProductMasterById(productId);
            if (productmaster == null || loc == null)
            {
                return false;
            }

            productmaster.ProductLocationsMap.Remove(loc);

            _currentDbContext.SaveChanges();
            return true;
        }

        public Dictionary<int, string> GetProductLocationList(long productId, int warehouseId, string code)
        {
            ProductMaster productmaster = _currentDbContext.ProductMaster.Find(productId);

            if (productmaster == null) return null;

            var pgoups = productmaster.ProductLocationsMap.Where(a => a.IsDeleted != true).Select(s => s.LocationId);

            var pg = from r in _currentDbContext.Locations
                     where (r.WarehouseId == warehouseId && (r.LocationCode.Contains(code) || String.IsNullOrEmpty(code)))
                     select new { r.LocationId, r.LocationCode };

            var res = (from p in pg
                       where !(pgoups)
                           .Contains(p.LocationId)
                       select p).ToDictionary(m => m.LocationId, m => m.LocationCode);
            return res;
        }

        public Dictionary<int, string> GetWarehouseLocationList(int warehouseId)
        {
            var pg = _currentDbContext.Locations
                .Where(r => r.WarehouseId == warehouseId)
                .ToDictionary(r => r.LocationId, r => r.LocationCode);
            return pg;
        }

        public ProductSCCCodes AddProductSccCodes(ProductSCCCodes code, int tenantId, int userId)
        {
            code.CreatedBy = userId;
            code.DateCreated = DateTime.UtcNow;
            code.DateUpdated = DateTime.UtcNow;
            code.TenantId = tenantId;
            code.UpdatedBy = userId;

            _currentDbContext.ProductSCCCodes.Add(code);
            _currentDbContext.SaveChanges();
            return code;
        }

        public bool RemoveProductSccCodes(int productId, int codeId, int tenantId, int userId)
        {
            ProductSCCCodes productscccodes = GetProductSccCodesById(codeId);

            if (productscccodes == null)
            {
                return false;
            }

            productscccodes.DateUpdated = DateTime.UtcNow;
            productscccodes.TenantId = tenantId;
            productscccodes.UpdatedBy = userId;
            productscccodes.IsDeleted = true;
            _currentDbContext.Entry(productscccodes).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return true;
        }

        public ProductSCCCodes GetProductSccCodesById(int productSccCodeId)
        {
            return _currentDbContext.ProductSCCCodes.Find(productSccCodeId);
        }

        public ProductLocations GetProductLocationMapById(int productLocationId)
        {
            return _currentDbContext.ProductLocationsMap.Find(productLocationId);
        }

        public InventoryStock GetInventoryStockById(int inventoryStockId)
        {
            return _currentDbContext.InventoryStocks.FirstOrDefault(m => m.InventoryStockId == inventoryStockId && m.IsDeleted != true);
        }

        public InventoryStock AddBlankInventoryStock(int productId, int tenantId, int warehouseId, int userId)
        {
            var inventory = new InventoryStock
            {
                ProductId = productId,
                InStock = 0,
                Available = 0,
                TenantId = tenantId,
                WarehouseId = warehouseId,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow
            };

            _currentDbContext.InventoryStocks.Add(inventory);
            _currentDbContext.SaveChanges();
            inventory = GetInventoryStockById(inventory.InventoryStockId);
            return inventory;
        }

        public bool IsValidBatchForProduct(int productId, string batchNumber)
        {
            var batchInventory = _currentDbContext.InventoryTransactions.Where(m => m.BatchNumber.Equals(batchNumber, StringComparison.CurrentCultureIgnoreCase));
            if (batchInventory.Any())
            {
                var validProductsForBatch = batchInventory.Select(m => m.ProductId);
                if (!validProductsForBatch.Contains(productId))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<OrderProcessSerialResponse> GetProductInfoBySerial(string productSerial, int tenantId)
        {
            var productSerialisation = await
                _currentDbContext.ProductSerialization.FirstOrDefaultAsync(m => m.SerialNo.Equals(productSerial, StringComparison.CurrentCultureIgnoreCase) && m.TenentId == tenantId);

            if (productSerialisation != null)
            {
                var result = new OrderProcessSerialResponse()
                {
                    TenantId = tenantId,
                    ProductName = productSerialisation.ProductMaster.Name,
                    ProductCode = productSerialisation.ProductMaster.SKUCode,
                    SerialCode = productSerialisation.SerialNo,
                    IsSerialised = true
                };
                return result;
            }
            return null;
        }

        public PalletTrackingSync SavePalletTrackings(PalletTrackingSync pallet, int tenantId)
        {
            if (pallet.PalletTrackingId > 0)
            {
                var newPallet = _currentDbContext.PalletTracking.Find(pallet.PalletTrackingId);
                newPallet.RemainingCases = pallet.RemainingCases;
                newPallet.Status = pallet.Status;
                newPallet.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(newPallet).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                _mapper.Map(newPallet, pallet);
                return pallet;
            }
            else
            {
                var newPallet = new PalletTracking();
                _mapper.Map(pallet, newPallet);

                if (newPallet.TenantId < 1)
                {
                    newPallet.TenantId = tenantId;
                }

                if (newPallet.DateCreated == null || newPallet.DateCreated == DateTime.MinValue)
                {
                    newPallet.DateCreated = DateTime.UtcNow;
                }

                newPallet.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(newPallet).State = EntityState.Added;
                _currentDbContext.SaveChanges();
                _mapper.Map(newPallet, pallet);
                return pallet;
            }

        }

        public PalletTracking GetPalletByPalletSerial(string palletSerial, int tenantId)
        {
            return _currentDbContext.PalletTracking.FirstOrDefault(x => x.PalletSerial == palletSerial && x.TenantId == tenantId);
        }

        public bool UpdateDontMonitorStockFlagForLocationproducts(int currentTenantId)
        {

            var products = this.GetAllValidProductMasters(currentTenantId).ToList();

            foreach (var prod in products)
            {
                if (prod.ProductId == 343 || prod.ProductId == 343 || prod.ProductId == 343 || prod.ProductId == 343)
                {
                    Console.Write("this should be marked as dontmoniter stock");
                }

                if (prod.ProductLocationsMap.Any(x => x.IsDeleted != true) && prod.ProductLocationsMap.Any(a => a.Locations.LocationCode != "external" && a.IsDeleted != true))
                {
                    prod.DontMonitorStock = false;
                }
                else
                {
                    prod.DontMonitorStock = true;

                    var externalLocation = _currentDbContext.Locations.Where(x => x.LocationCode == "external").FirstOrDefault();

                    var newLocation = new ProductLocations { ProductId = prod.ProductId, LocationId = externalLocation.LocationId, DateCreated = DateTime.UtcNow, CreatedBy = caCurrent.CurrentUser().UserId, TenantId = currentTenantId, IsActive = true };
                    _currentDbContext.ProductLocationsMap.Add(newLocation);
                }
            }

            _currentDbContext.SaveChanges();

            return true;

        }
        public List<Tuple<string, string, decimal, bool>> AllocatedProductDetail(int productId, int WarehouseId, int details)
        {
            if (details == 1)
            {
                var orderId = _currentDbContext.OrderDetail.Where(m => m.ProductId == productId && (m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder
                    || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Loan || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples
                    || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Exchange || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferOut) && m.Order.WarehouseId == WarehouseId &&
                                m.Order.OrderStatusID != (int)OrderStatusEnum.Complete && m.Order.OrderStatusID != (int)OrderStatusEnum.Cancelled && m.Order.OrderStatusID != (int)OrderStatusEnum.PostedToAccounts && m.Order.OrderStatusID != (int)OrderStatusEnum.Invoiced
                                && m.Order.IsDeleted != true).Select(u => u.OrderID).ToList();

                List<Tuple<string, string, decimal, bool>> detail = new List<Tuple<string, string, decimal, bool>>();
                foreach (var item in orderId)
                {

                    var OrderNumber = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.OrderNumber;
                    var AccountNumber = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.Account?.AccountCode;
                    var itemsOnSalesOrders = _currentDbContext.Order.Select(m => m.OrderDetails.Where(p => p.OrderID == item && p.ProductId == productId && p.IsDeleted != true).Select(x => x.Qty).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

                    var itemsDispatched = _currentDbContext.Order
                                      .Select(m => m.OrderProcess.Where(p => p.OrderID == item).Select(o => o.OrderProcessDetail.Where(p => p.ProductId == productId && p.IsDeleted != true).Select(q => q.QtyProcessed).DefaultIfEmpty(0)
                                  .Sum()).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

                    var itemsAllocated = itemsOnSalesOrders - itemsDispatched;
                    bool directship = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.DirectShip ?? false;
                    if (itemsAllocated > 0 && detail.Count(u=>u.Item2==OrderNumber)<=0)
                    {
                        detail.Add(new Tuple<string, string, decimal, bool>(AccountNumber, OrderNumber, itemsAllocated, directship));
                    }
                }
                return detail;
            }
            else
            {
                var orderId = _currentDbContext.OrderDetail.Where(m => m.ProductId == productId && (m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferIn
               || m.Order.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns) && m.Order.WarehouseId == WarehouseId &&
                           m.Order.OrderStatusID != (int)OrderStatusEnum.Complete && m.Order.OrderStatusID != (int)OrderStatusEnum.Cancelled && m.Order.OrderStatusID != (int)OrderStatusEnum.PostedToAccounts && m.Order.OrderStatusID != (int)OrderStatusEnum.Invoiced
                           && m.Order.ShipmentPropertyId == null && m.Order.IsDeleted != true).Select(u => u.OrderID).ToList();

                List<Tuple<string, string, decimal, bool>> detail = new List<Tuple<string, string, decimal, bool>>();

                foreach (var item in orderId)
                {

                    var OrderNumber = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.OrderNumber;
                    var AccountNumber = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.Account?.AccountCode;
                    var itemsOrdered = _currentDbContext.Order.Select(m => m.OrderDetails.Where(p => p.OrderID == item && p.ProductId == productId && p.IsDeleted != true).Select(x => x.Qty).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

                    var itemsReceived = _currentDbContext.Order
                        .Select(m => m.OrderProcess.Where(p => p.OrderID == item).Select(o => o.OrderProcessDetail.Where(p => p.ProductId == productId && p.IsDeleted != true && p.OrderDetail.DontMonitorStock != true).Select(q => q.QtyProcessed).DefaultIfEmpty(0)
                                  .Sum()).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

                    var itemsOnOrder = itemsOrdered - itemsReceived;
                    bool directship = _currentDbContext.Order.FirstOrDefault(u => u.OrderID == item)?.DirectShip ?? false;
                    if (itemsOnOrder > 0 && detail.Count(u => u.Item2 == OrderNumber) <= 0)
                    {
                        detail.Add(new Tuple<string, string, decimal, bool>(AccountNumber, OrderNumber, itemsOrdered, directship));
                    }
                }

                return detail;
            }
        }

        public IEnumerable<PalletTracking> GetAllPalletByOrderProcessDetailId(int orderprocessDetailId, int tenantId)
        {
            var palletTrackingId = _currentDbContext.InventoryTransactions.Where(u => u.OrderProcessDetailId == orderprocessDetailId && u.IsDeleted != true).Select(u => u.PalletTrackingId);
            if (palletTrackingId != null)
            {
                return _currentDbContext.PalletTracking.Where(u => palletTrackingId.Contains(u.PalletTrackingId) && u.TenantId == tenantId).ToList();
            }
            return null;
        }
        public IEnumerable<ProductSerialis> GetAllProductSerialbyOrderProcessDetailId(int orderprocessdetailId, int tenantId, bool? type)
        {
            var serialId = _currentDbContext.InventoryTransactions.Where(u => u.OrderProcessDetailId == orderprocessdetailId).Select(u => u.SerialID);
            if (serialId != null)
            {
                return _currentDbContext.ProductSerialization.Where(u => serialId.Contains(u.SerialID) && u.TenentId == tenantId && ((type.HasValue && u.CurrentStatus != InventoryTransactionTypeEnum.PurchaseOrder) || (!type.HasValue && u.CurrentStatus != InventoryTransactionTypeEnum.SalesOrder))).ToList();
            }
            return null;
        }


        public bool GetInventroyTransactionCountbyOrderProcessDetailId(int orderprocessdetailId, int tenantId)
        {

            var count = _currentDbContext.InventoryTransactions.Count(u => u.OrderProcessDetailId == orderprocessdetailId && u.IsDeleted != true);
            if (count > 1)
            {
                return false;
            }
            return true;

        }


        public IEnumerable<ProductMaster> GetAllProductProcessByPallet(int tenantId)
        {
            return _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && a.IsDeleted != true && a.ProcessByPallet == true);
        }

        public string CreatePalletTracking(PalletTracking palletTracking, int NoOfLabels)
        {
            List<int> palletTrackingId = new List<int>();
            for (int i = 0; i < NoOfLabels; i++)
            {
                PalletTracking palletTrackings = new PalletTracking();
                var date = DateTime.Now.Date.ToString("ddMMyy");
                date += DateTime.Now.ToString("HHmmss");
                date += GenerateRandomNo();
                palletTracking.PalletSerial = date;
                palletTrackings = palletTracking;
                _currentDbContext.PalletTracking.Add(palletTrackings);
                _currentDbContext.SaveChanges();
                palletTrackingId.Add(palletTrackings.PalletTrackingId);

            }
            return string.Join(",", palletTrackingId.ToArray());

        }
        public string GenerateRandomNo()
        {
            Random _random = new Random();
            return _random.Next(0, 9999).ToString("D4");
        }
    }
}