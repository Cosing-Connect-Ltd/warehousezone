﻿using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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
            return _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && (includeIsDeleted || a.IsDeleted != true) && (!lastUpdated.HasValue || (a.DateUpdated ?? a.DateCreated) >= lastUpdated))
                .Include(x => x.ProductKitItems);
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

        public IQueryable<ProductMaster> GetAllValidProducts(int tenantId, string args = null, int OrderId = 0, int departmentId = 0, int groupId = 0, int ProductId = 0)
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
                    else if(type == 2)
                    {
                        palletProduct.Status = PalletTrackingStatusEnum.Active;
                    }
                    else if (type == 3)
                    {
                        palletProduct.Status = PalletTrackingStatusEnum.Archived;
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
            var product = _currentDbContext.ProductMaster.Include(m => m.ProductAttributeValuesMap).FirstOrDefault(a => a.ProductId == productId && a.IsDeleted != true);
            if (product != null)
            {
                product.ProductKitMap = _currentDbContext.ProductKitMaps.Where(m => m.ProductId == productId && m.IsDeleted != true).ToList();
            }
            return product;
        }

        public IEnumerable<ProductMaster> GetProductMasterDropdown(int productId)
        {
            var product = _currentDbContext.ProductMaster.Where(a => a.ProductId == productId && a.IsDeleted != true);

            return product;
        }

        public IEnumerable<ProductKitMap> GetAllProductKitMapsByProductId(int productId, ProductKitTypeEnum productKitType)
        {
            return _currentDbContext.ProductKitMaps.Where(a => a.ProductId == productId && a.IsActive == true && a.ProductKitType == productKitType).Include(u => u.ProductKitTypes)
                .Distinct().ToList();

            //if (disProdIds.Count > 0)
            //    return _currentDbContext.ProductKitMaps.Where(a => disProdIds.Contains(a.ProductId)).ToList();

            //return new List<ProductKitMap>();
        }

        public IEnumerable<ProductMaster> GetAllProductInKitsByProductId(int productId)
        {
            return GetAllProductInKitsByProductIds(new List<int> { productId });
        }

        public IEnumerable<ProductMaster> GetAllProductInKitsByProductIds(List<int> productIds)
        {
            var kitProductIds = _currentDbContext.ProductKitMaps.Where(a => productIds.Contains(a.ProductId) &&
                                                                (a.ProductKitTypes.UseInParentCalculations == true || a.ProductMaster.ProductType == ProductKitTypeEnum.ProductByAttribute) &&
                                                                a.IsDeleted != true &&
                                                                a.IsActive)
                                                 .Select(a => a.KitProductMaster.ProductId)
                                                 .Distinct()
                                                 .ToList();

            var kitProducts = _currentDbContext.ProductMaster.Where(a => kitProductIds.Contains(a.ProductId)).ToList();

            kitProducts.ForEach(r => r.ProductAttributeValuesMap = r.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).ToList());

            return kitProducts;
        }

        public IEnumerable<ProductMaster> GetAllProductByAttributeKitsByProductId(int productId)
        {
            var kitProductIds = _currentDbContext.ProductKitMaps.Where(a => a.ProductId == productId &&
                                                                            a.IsDeleted != true &&
                                                                            a.IsActive &&
                                                                            a.ProductKitType == ProductKitTypeEnum.ProductByAttribute)
                                                             .Select(a => a.KitProductMaster.ProductId)
                                                             .Distinct()
                                                             .ToList();

            var kitProducts = kitProductIds.Any() ? _currentDbContext.ProductMaster.Where(a => kitProductIds.Contains(a.ProductId) &&
                                                                a.IsActive &&
                                                                a.ProductType != ProductKitTypeEnum.ProductByAttribute &&
                                                                a.IsDeleted != true).ToList() : new List<ProductMaster>();

            kitProducts.ForEach(r => r.ProductAttributeValuesMap = r.ProductAttributeValuesMap.Where(p => p.IsDeleted != true).ToList());

            return kitProducts;
        }

        public List<ProductMaster> GetRelatedProductsByProductId(int productId, int tenantId, int siteId, int? parentProductId = null)
        {
            var relatedProducts = _currentDbContext.ProductKitMaps.Where(u => u.ProductKitType == ProductKitTypeEnum.RelatedProduct &&
                                                                                u.IsDeleted != true &&
                                                                                u.TenantId == tenantId &&
                                                                                (u.ProductId == productId || u.KitProductId == productId || u.ProductId == parentProductId || u.KitProductId == parentProductId) &&
                                                                                u.IsActive &&
                                                                                u.KitProductMaster.IsDeleted != true)
                                                                    .Select(u => (u.ProductId == productId || u.ProductId == parentProductId ? u.KitProductMaster : u.ProductMaster))
                                                                    .ToList();

            return relatedProducts.GroupBy(p => p.ProductId)
                                  .Select(p => p.First())
                                  .Where(p => _currentDbContext.ProductsWebsitesMap.Any(w => w.IsDeleted != true &&
                                                                                             w.SiteID == siteId &&
                                                                                             w.TenantId == tenantId &&
                                                                                             w.IsActive &&
                                                                                             w.ProductId == p.ProductId))
                                  .ToList();
        }

        public IEnumerable<ProductMaster> GetAllProductInKitsByKitProductId(int kitProductId)
        {
            var disProdIds = _currentDbContext.ProductKitMaps.Where(a => a.KitProductId == kitProductId && a.IsDeleted != true && a.IsActive)
                .Select(a => a.KitProductMaster.ProductId).Distinct().ToList();

            if (disProdIds.Count > 0)
                return _currentDbContext.ProductMaster.Where(a => disProdIds.Contains(a.ProductId)).ToList();

            return new List<ProductMaster>();
        }

        public IEnumerable<ProductMaster> GetParentProductsByKitProductId(int kitProductId)
        {
            return _currentDbContext.ProductKitMaps.Where(a => a.KitProductId == kitProductId &&
                                                                a.IsDeleted != true &&
                                                                a.IsActive)
                                                   .GroupBy(p => p.ProductId)
                                                   .Select(a => a.FirstOrDefault().ProductMaster)
                                                   .ToList();
        }

        public ProductKitMap GetProductInKitsByKitId(int KitId)
        {
            return _currentDbContext.ProductKitMaps.FirstOrDefault(a => a.Id == KitId && a.IsDeleted != true);
        }

        public void SaveSelectedProductRecipeItems(int productId, List<RecipeProductItemRequest> recipeItems, int currentUserId)
        {
            var productMaster = GetProductMasterById(productId);
            var recipeProducts = productMaster.ProductKitMap.Where(m => m.IsDeleted != true && m.ProductKitType == ProductKitTypeEnum.Recipe).Select(m => m.KitProductMaster).ToList();

            var removedItems = recipeProducts.Where(m => !recipeItems.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in removedItems)
            {
                var recipeLink = _currentDbContext.ProductKitMaps.First(m => m.ProductId == productId && m.KitProductId == item.ProductId && m.IsDeleted != true);
                recipeLink.IsDeleted = true;
                recipeLink.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(recipeLink).State = EntityState.Modified;
            }

            var addedItems = recipeItems.Where(m => !recipeProducts.Select(x => x.ProductId).Contains(m.ProductId));
            foreach (var item in addedItems)
            {
                var recipeItem = new ProductKitMap
                {
                    ProductId = productId,
                    KitProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                recipeItem.UpdateCreatedInfo(currentUserId);
                _currentDbContext.Entry(recipeItem).State = EntityState.Added;
            }

            var modifiedItems = productMaster.ProductKitMap.Where(m => recipeItems.Select(x => x.ProductId).Contains(m.KitProductId));
            foreach (var item in modifiedItems)
            {
                var recipeItem = recipeItems.First(m => m.ProductId == item.KitProductId);
                item.Quantity = recipeItem.Quantity;
                item.UpdateUpdatedInfo(currentUserId);
                _currentDbContext.Entry(item).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
        }

        public void SaveSelectedProductKitItems(int productId, RecipeProductItemRequest kitItems, int currentUserId, int tenantId)
        {
            var recipeProducts = _currentDbContext.ProductKitMaps.Where(m => m.IsDeleted != true && m.ProductKitType == (ProductKitTypeEnum)kitItems.KitType && m.KitProductId == kitItems.ProductId).ToList();
            if (recipeProducts.Count > 0)
            {
                foreach (var item in recipeProducts)
                {
                    item.Quantity = item.Quantity > 0 ? item.Quantity : 1;
                    if (kitItems.ProductkitType.HasValue)
                    {
                        item.ProductKitTypeId = kitItems.ProductkitType;
                    }
                    item.UpdatedBy = currentUserId;
                    item.DateUpdated = DateTime.UtcNow;
                    _currentDbContext.Entry(item).State = EntityState.Modified;
                }
            }
            else
            {
                var recipeItem = new ProductKitMap
                {
                    ProductId = kitItems.ParentProductId,
                    KitProductId = kitItems.ProductId,
                    Quantity = kitItems.Quantity > 0 ? kitItems.Quantity : 1,
                    TenantId = tenantId,
                    CreatedBy = currentUserId,
                    DateCreated = DateTime.UtcNow,
                    ProductKitType = (ProductKitTypeEnum)kitItems.KitType,
                };
                if (kitItems.ProductkitType.HasValue)
                {
                    recipeItem.ProductKitTypeId = kitItems.ProductkitType;
                }
                _currentDbContext.Entry(recipeItem).State = EntityState.Added;
            }

            _currentDbContext.SaveChanges();
        }

        public void RemoveRecipeItemProduct(int KitProductId, int currentUserId)
        {
            var productRecipeItem = _currentDbContext.ProductKitMaps.FirstOrDefault(m => m.Id == KitProductId && m.IsDeleted != true);

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
            var productRecipeItem = _currentDbContext.ProductKitMaps.FirstOrDefault(m => m.ProductId == productId && m.KitProductId == recipeItemProductId && m.IsDeleted != true && m.ProductKitType == ProductKitTypeEnum.Recipe);

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
            //TODO: check new location calculation implementation here
            bool status = false;
            var cTransaction = _currentDbContext.InventoryTransactions.Find(transactionId);

            cTransaction.IsCurrentLocation = false;
            var cTransactions = _currentDbContext.InventoryTransactions
                .Where(a => a.LocationId == cTransaction.LocationId &&
                            a.ProductId == cTransaction.ProductId && a.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder
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

                status = Inventory.StockTransaction(cTransaction.ProductId, InventoryTransactionTypeEnum.PurchaseOrder, 1, cTransaction.OrderID, locationId, null, productSerial.SerialID);
            }
            else
            {
                var cQuantity = _commonDbServices.GetQuantityInLocation(cTransaction);

                status = Inventory.StockTransaction(cTransaction.ProductId, InventoryTransactionTypeEnum.PurchaseOrder, moveQuantity, cTransaction.OrderID, locationId, null, null);

                if ((cQuantity - moveQuantity) > 0)
                {
                    status = Inventory.StockTransaction(cTransaction.ProductId, InventoryTransactionTypeEnum.PurchaseOrder, (cQuantity - moveQuantity), cTransaction.OrderID, locationId, null, null);
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
                    select new OrderPriceViewModel()
                    {
                        ProductId = pd.ProductId,
                        CompanyName = s.CompanyName,
                        Price = pd.Price,
                        CurrencyName = c.CurrencyName,
                        OrderID = Order.OrderID,
                        OrderNumber = Order.OrderNumber,
                        Status = Order.OrderStatusID.ToString()
                    }).ToList();
        }

        public List<ProductAttributes> GetAllProductAttributes()
        {
            return _currentDbContext.ProductAttributes.Where(u => u.IsDeleted != true).ToList();
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
            return _currentDbContext.ProductLocations.Where(a => a.LocationId == locationId && a.IsDeleted != true).Select(a => a.ProductId);
        }

        public IEnumerable<int> GetAllProductLocationsFromMaps(int productId)
        {
            return _currentDbContext.ProductLocations.Where(m => m.ProductId == productId).Select(m => m.LocationId);
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
            List<int> productLocationIds, List<int> AttributeIds, int userId, int tenantId, List<int> SiteId, List<RecipeProductItemRequest> recipeProductItems)
        {
            if (productMaster.ProductGroupId == 0)
            {
                productMaster.ProductGroupId = null;
            }
            if (productMaster.ProductId > 0)
            {


                productMaster.ProductCategoryId = productMaster.ProductCategoryId > 0 ? productMaster.ProductCategoryId : null;
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
                //if (productAttributesIds == null)
                //{
                //    foreach (var entity in _currentDbContext.ProductAttributeValuesMap.Where(
                //        x => x.ProductId == productMaster.ProductId))
                //    {
                //        entity.IsDeleted = true;
                //        _currentDbContext.Entry(entity).State = EntityState.Modified;
                //    }
                //}
                //else
                //{
                //    var ToDelete = new List<int>();
                //    ToDelete = _currentDbContext.ProductAttributeValuesMap
                //        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                //        .Select(x => x.AttributeValueId)
                //        .ToList()
                //        .Except(productAttributesIds)
                //        .ToList();
                //    var ToAdd = new List<int>();
                //    ToAdd = productAttributesIds
                //        .Except(_currentDbContext.ProductAttributeValuesMap
                //            .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                //            .Select(x => x.AttributeValueId)
                //            .ToList())
                //        .ToList();

                //    foreach (var item in ToDelete)
                //    {
                //        var Current = _currentDbContext.ProductAttributeValuesMap
                //            .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.AttributeValueId == item &&
                //                                 x.IsDeleted != true);
                //        Current.IsDeleted = true;
                //        _currentDbContext.Entry(Current).State = EntityState.Modified;
                //    }
                //    foreach (var item in ToAdd)
                //    {
                //        var newItem = new ProductAttributeValuesMap()
                //        {
                //            CreatedBy = userId,
                //            DateCreated = DateTime.UtcNow,
                //            AttributeValueId = item,
                //            TenantId = tenantId,
                //            ProductId = productMaster.ProductId,
                //        };
                //        _currentDbContext.ProductAttributeValuesMap.Add(newItem);
                //    }
                //}

                if (productLocationIds == null)
                {
                    foreach (var entity in _currentDbContext.ProductLocations.Where(
                        x => x.ProductId == productMaster.ProductId))
                    {
                        entity.IsDeleted = true;
                        _currentDbContext.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    var ToDelete = new List<int>();
                    ToDelete = _currentDbContext.ProductLocations
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.LocationId)
                        .ToList()
                        .Except(productLocationIds)
                        .ToList();
                    var ToAdd = new List<int>();
                    ToAdd = productLocationIds
                        .Except(_currentDbContext.ProductLocations
                            .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                            .Select(x => x.LocationId)
                            .ToList())
                        .ToList();

                    foreach (int item in ToDelete)
                    {
                        var Current = _currentDbContext.ProductLocations
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
                        _currentDbContext.ProductLocations.Add(newItem);
                    }
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

                if (productAttributesIds != null && productMaster.ProductType == ProductKitTypeEnum.Simple)
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

                        _currentDbContext.ProductLocations.Add(pLocation);
                    }
                }

                _currentDbContext.SaveChanges();
            }

            if (SiteId != null && SiteId.Count > 0)
            {
                var isdeletedList = _currentDbContext.ProductTagMaps.Where(u => u.ProductId == productMaster.ProductId && !SiteId.Contains(u.TagId) && u.IsDeleted != true).ToList();
                if (isdeletedList.Count > 0)
                {
                    isdeletedList.ForEach(u => u.IsDeleted = true);
                    _currentDbContext.SaveChanges();
                }

                foreach (var item in SiteId)
                {
                    int siteId = Convert.ToInt32(item);
                    var websiteslist = _currentDbContext.ProductTagMaps.FirstOrDefault(u => u.ProductId == productMaster.ProductId && u.TagId == siteId && u.IsDeleted != true);
                    if (websiteslist != null)
                    {
                        websiteslist.TenantId = tenantId;
                        websiteslist.UpdateUpdatedInfo(userId);
                    }
                    else
                    {
                        var list = new ProductTagMap();
                        list.ProductId = productMaster.ProductId;
                        list.TagId = siteId;
                        list.TenantId = tenantId;
                        list.UpdateCreatedInfo(userId);
                        productMaster.ProductTagMaps.Add(list);
                    }
                }
                _currentDbContext.SaveChanges();
            }
            else
            {
                var isdeletedList = _currentDbContext.ProductTagMaps.Where(u => u.ProductId == productMaster.ProductId && u.IsDeleted != true).ToList();
                if (isdeletedList.Count > 0)
                {
                    isdeletedList.ForEach(u => u.IsDeleted = true);
                    _currentDbContext.SaveChanges();
                }
            }
            if (AttributeIds != null && AttributeIds.Count > 0 && productMaster.ProductType == ProductKitTypeEnum.ProductByAttribute)
            {
                var ToDelete = new List<int>();
                ToDelete = _currentDbContext.ProductAttributeMaps
                    .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                    .Select(x => x.AttributeId)
                    .ToList()
                    .Except(AttributeIds)
                    .ToList();
                var ToAdd = new List<int>();
                ToAdd = AttributeIds
                    .Except(_currentDbContext.ProductAttributeMaps
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.AttributeId)
                        .ToList())
                    .ToList();

                foreach (int item in ToDelete)
                {
                    var Current = _currentDbContext.ProductAttributeMaps
                        .FirstOrDefault(x => x.ProductId == productMaster.ProductId && x.AttributeId == item &&
                                             x.IsDeleted != true);
                    Current.IsDeleted = true;
                    _currentDbContext.Entry(Current).State = EntityState.Modified;
                }
                foreach (int item in ToAdd)
                {
                    var newItem = new ProductAttributeMap()
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        AttributeId = item,
                        TenantId = tenantId,
                        ProductId = productMaster.ProductId,
                    };
                    _currentDbContext.ProductAttributeMaps.Add(newItem);
                }
                _currentDbContext.SaveChanges();
            }
            else
            {
                var isdeletedList = _currentDbContext.ProductAttributeMaps.Where(u => u.ProductId == productMaster.ProductId && u.IsDeleted != true).ToList();
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

        public ProductFiles GetProductFilesById(int Id)
        {
            return _currentDbContext.ProductFiles.FirstOrDefault(u => u.Id == Id && u.IsDeleted != true);
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

        public ProductMaster SaveEditProduct(ProductMasterViewModel productMaster, int userId, int tenantId)
        {
            var product = _currentDbContext.ProductMaster.FirstOrDefault(u => u.ProductId == productMaster.ProductId);
            if (product != null)
            {
                product.UpdateUpdatedInfo(userId);
                product.Name = productMaster.Name;
                product.SKUCode = productMaster.SKUCode;
                product.Serialisable = productMaster.Serialisable;
                product.ProcessByPallet = productMaster.ProcessByPallet;
                product.SellPrice = productMaster.SellPrice;
                product.BuyPrice = productMaster.BuyPrice;
                product.LandedCost = productMaster.LandedCost;
                product.PercentMargin = productMaster.PercentMargin;
            }

            if (!string.IsNullOrEmpty(productMaster.TagIds))
            {
                var tagIds = productMaster.TagIds.Split(',').Select(Int32.Parse).ToList();
                var ToDelete = new List<int>();
                ToDelete = _currentDbContext.ProductTagMaps
                    .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                    .Select(x => x.TagId)
                    .ToList()
                    .Except(tagIds)
                    .ToList();
                var ToAdd = new List<int>();
                ToAdd = tagIds
                    .Except(_currentDbContext.ProductTagMaps
                        .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true)
                        .Select(x => x.TagId)
                        .ToList())
                    .ToList();
                foreach (var itemToDelete in ToDelete)
                {
                    var itemdelete = _currentDbContext.ProductTagMaps.FirstOrDefault(u =>
                        u.ProductId == productMaster.ProductId && u.TagId == itemToDelete && u.IsDeleted != true);
                    if (itemdelete != null)
                    {
                        itemdelete.IsDeleted = true;
                        itemdelete.UpdateUpdatedInfo(userId);
                        _currentDbContext.Entry(itemdelete).State = EntityState.Modified;
                    }
                }
                foreach (var itemToAdd in ToAdd)
                {
                    var ProductTagMap = new ProductTagMap
                    {
                        ProductId = productMaster.ProductId,
                        TagId = itemToAdd,
                        TenantId = tenantId
                    };
                    ProductTagMap.UpdateCreatedInfo(userId);
                    _currentDbContext.ProductTagMaps.Add(ProductTagMap);
                }
            }
            else
            {
                var producTagMaps = _currentDbContext.ProductTagMaps
                    .Where(x => x.ProductId == productMaster.ProductId && x.IsDeleted != true);
                foreach (var itemToDelete in producTagMaps)
                {
                    itemToDelete.IsDeleted = true;
                    itemToDelete.UpdateUpdatedInfo(userId);
                    _currentDbContext.Entry(itemToDelete).State = EntityState.Modified;
                }
            }

            product.TenantId = tenantId;
            _currentDbContext.Entry(product).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return product;
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

                Inventory.StockTransaction(product, InventoryTransactionTypeEnum.PurchaseOrder, 1, order, location, delivery, serial.SerialID, orderprocessId: (oprocess?.OrderProcessID), orderProcessDetailId: (odet?.OrderProcessDetailID));
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

        public IEnumerable<ProductMaster> GetProductByCategory(int siteId, int tenantId, int numberofProducts, string tagName, int? tagId = null)
        {
            var products = _currentDbContext.ProductsWebsitesMap.Where(u => u.TenantId == tenantId && u.SiteID == siteId && u.IsDeleted != true && u.IsActive).Select(u => u.ProductMaster);
            var productTagIds = _currentDbContext.ProductTags.Where(u => (u.TagName.Equals(tagName, StringComparison.CurrentCultureIgnoreCase) || u.Id == tagId) && u.IsDeleted != true).Select(u => u.Id).ToList();
            var productIds = _currentDbContext.ProductTagMaps.Where(u => productTagIds.Contains(u.TagId) && u.IsDeleted != true).Select(u => u.ProductId).ToList();
            return products.Where(u => productIds.Contains(u.ProductId) && u.IsDeleted != true).Take(numberofProducts);
        }

        public IQueryable<InventoryStock> GetAllInventoryStocks(int tenantId, int warehouseId, DateTime? reqDate = null)
        {
            var model = _currentDbContext.InventoryStocks.Include(x => x.ProductMaster)
                .Where(x => x.TenantId == tenantId && x.WarehouseId == warehouseId &&
                            x.ProductMaster.IsDeleted != true && x.ProductMaster.DontMonitorStock != true && (!reqDate.HasValue || (x.DateUpdated ?? x.DateCreated) >= reqDate));

            return model;
        }

        public IQueryable<ProductLocationStocks> GetAllProductLocationStocks(int tenantId, int warehouseId, DateTime? reqDate = null)
        {
            var model = _currentDbContext.ProductLocationStocks
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
                .Include(x => x.Order.PProperties);
        }

        public IEnumerable<InventoryTransaction> GetAllInventoryTransactionsByProductId(int productId, int warehouseId)
        {
            return _currentDbContext.InventoryTransactions.Where(a => a.ProductId == productId && a.IsDeleted != true && a.WarehouseId == warehouseId);
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
             x.IsDeleted != true && (x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns
                 || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn)).Select(u => u.Quantity).DefaultIfEmpty(0).Sum();

            var salesOrdersTransactios = _currentDbContext.InventoryTransactions.Where(x => x.PalletTrackingId == PalletTrackingId
             && x.OrderProcessDetailId == OrderProcessDetailId &&
             x.IsDeleted != true && (x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Loan
                || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Samples || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut
                || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder || x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage)).Select(u => u.Quantity).DefaultIfEmpty(0).Sum();
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
            var product = _currentDbContext.ProductMaster
                .Include(p => p.ProductKitMap.Select(k => k.ProductMaster.ProductAttributeValuesMap.Select(a => a.ProductAttributeValues)))
                .Include(p => p.ProductManufacturer)
                .Include(p => p.ProductKitItems.Select(k => k.KitProductMaster.ProductManufacturer))
                .Include(p => p.ProductKitItems.Select(k => k.ProductKitTypes))
                .FirstOrDefault(e => e.TenantId == tenantId &&
                                     e.IsDeleted != true &&
                                     (e.SKUCode.Equals(productCode, StringComparison.CurrentCultureIgnoreCase) ||
                                      e.BarCode.Equals(productCode, StringComparison.CurrentCultureIgnoreCase) ||
                                      e.ManufacturerPartNo.Equals(productCode, StringComparison.CurrentCultureIgnoreCase)));

            if (product != null)
            {
                product.ProductKitItems = product.ProductKitItems.Where(k => k.IsActive &&
                                                                         k.IsDeleted != true &&
                                                                         _currentDbContext.ProductsWebsitesMap.Any(m => m.ProductId == k.KitProductId &&
                                                                                                                        m.IsActive &&
                                                                                                                        m.IsDeleted != true))
                                                                                                                        .ToList();
            }

            return product;
        }

        public ProductMaster GetProductMasterByOuterBarcode(string outerBarcode, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.BarCode2.Equals(outerBarcode, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public ProductMaster GetProductMasterByBarcode(string barcode, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.BarCode.Equals(barcode, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public ProductMaster GetProductMasterByName(string name, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public ProductMaster GetProductMasterBySKU(string SKU, int tenantId)
        {
            return _currentDbContext.ProductMaster.FirstOrDefault(e => e.TenantId == tenantId && e.SKUCode.Equals(SKU, StringComparison.CurrentCultureIgnoreCase) && e.IsDeleted != true);
        }

        public string GenerateNextProductCode(int tenantId)
        {
            var tenant = _currentDbContext.Tenants.Find(tenantId);
            if (tenant != null && tenant.ProductCodePrefix != null)
            {
                var product = _currentDbContext.ProductMaster.Where(m => m.SKUCode.Contains(tenant.ProductCodePrefix) && m.TenantId == tenantId).OrderByDescending(m => m.SKUCode).FirstOrDefault();
                if (product != null)
                {
                    int validSkuCode = 0;
                    var lastCode = product.SKUCode.Split(new[] { tenant.ProductCodePrefix, tenant.ProductCodePrefix.ToLower() }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (int.TryParse(lastCode, out validSkuCode))
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

        public IQueryable<ProductMasterViewModel> GetAllProductMasterDetail(int tenantId, int warehouseId, ProductKitTypeEnum? KitType = null, int? productId = null)
        {
            var productAttributeIds = new List<int>();

            if (productId != null && KitType == ProductKitTypeEnum.ProductByAttribute)
            {
                productAttributeIds = _currentDbContext.ProductAttributeMaps.Where(x => x.ProductId == productId && x.IsDeleted != true).Select(y => y.ProductAttributes.AttributeId).ToList();
            }

            var model = _currentDbContext.ProductMaster.AsNoTracking()
                .Include(u => u.ProductKitMap)
                .Include(x => x.ProductAttributeValuesMap)
                .Where(x => x.TenantId == tenantId && x.IsDeleted != true && (!productId.HasValue || x.ProductId != productId)
                && ((!productAttributeIds.Any() && KitType != ProductKitTypeEnum.ProductByAttribute)
                || x.ProductAttributeValuesMap.Where(a => a.IsDeleted != true).Select(m => m.ProductAttributeValues.ProductAttributes.AttributeId).Any(n => productAttributeIds.Contains(n))))
                .Select(prd => new ProductMasterViewModel
                {
                    ProductId = prd.ProductId,
                    Name = prd.Name,
                    SKUCode = prd.SKUCode,
                    Description = prd.Description,
                    BarCode = prd.BarCode,
                    Serialisable = prd.Serialisable,
                    IsStockItem = prd.IsStockItem,
                    ProductType = prd.ProductType,
                    UOM = prd.GlobalUOM.UOM,
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
                    BuyPrice = prd.BuyPrice,
                    SellPrice = prd.SellPrice,
                    LandedCost = prd.LandedCost,
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
                    ProductGroupName = prd.ProductGroup == null ? "" : prd.ProductGroup.ProductGroup,
                    ProductCategoryName = prd.ProductCategory == null ? "" : prd.ProductCategory.ProductCategoryName,
                    DepartmentName = prd.TenantDepartment.DepartmentName,
                    Location = prd.ProductLocations.Where(a => a.IsDeleted != true).Select(x => x.Locations.LocationCode).FirstOrDefault().ToString(),
                    EnableWarranty = prd.EnableWarranty ?? false,
                    EnableTax = prd.EnableTax ?? false,
                    DontMonitorStock = prd.DontMonitorStock,
                    IsPreOrderAccepted = prd.IsPreOrderAccepted,
                    MinDispatchDays = prd.MinDispatchDays,
                    MaxDispatchDays = prd.MaxDispatchDays,
                    ProcessByPallet = prd.ProcessByPallet,
                    Qty = prd.ProductKitMap.Where(x => x.KitProductId == prd.ProductId && x.IsDeleted != true && x.TenantId == prd.TenantId && x.ProductKitType == KitType).Select(u => u.Quantity).DefaultIfEmpty(0).Sum(),
                    Id = prd.ProductKitMap.FirstOrDefault(x => x.KitProductId == prd.ProductId && x.IsDeleted != true && x.TenantId == prd.TenantId && x.ProductKitType == KitType).ProductKitTypeId,
                    IsActive = prd.ProductKitMap.FirstOrDefault(x => x.KitProductId == prd.ProductId && x.IsDeleted != true && x.TenantId == prd.TenantId && x.ProductKitType == KitType && (productId == null || x.ProductId == productId)).IsActive,
                    AttributeValueNames = prd.ProductAttributeValuesMap.Where(m => m.IsDeleted != true).Select(x => x.ProductAttributeValues.ProductAttributes.AttributeName + ": " + x.ProductAttributeValues.Value).ToList(),
                    ProductTagMap = prd.ProductTagMaps.Where(u => u.IsDeleted != true).Select(u => u.TagId.ToString()).ToList()
                });

            return model;
        }

        public ProductLocations AddProductLocationMap(int productId, int locationId)
        {
            ProductMaster productmaster = GetProductMasterById(productId);

            ProductLocations location = _currentDbContext.ProductLocations.Find(locationId);

            if (productmaster == null || location == null)
            {
                return null;
            }

            productmaster.ProductLocations.Add(location);

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
                    ProductLocations loc = _currentDbContext.ProductLocations.Find(locationIds[i]);
                    if (loc != null)
                    {
                        productmaster.ProductLocations.Add(loc);
                    }
                }

                _currentDbContext.SaveChanges();
            }

            return productmaster.ProductLocations.ToList();
        }

        public bool RemoveProductLocationMap(int productId, int locationId)
        {
            ProductLocations loc = GetProductLocationMapById(locationId);

            ProductMaster productmaster = GetProductMasterById(productId);
            if (productmaster == null || loc == null)
            {
                return false;
            }

            productmaster.ProductLocations.Remove(loc);

            _currentDbContext.SaveChanges();
            return true;
        }

        public Dictionary<int, string> GetProductLocationList(long productId, int warehouseId, string code)
        {
            ProductMaster productmaster = _currentDbContext.ProductMaster.Find(productId);

            if (productmaster == null) return null;

            var pgoups = productmaster.ProductLocations.Where(a => a.IsDeleted != true).Select(s => s.LocationId);

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
            return _currentDbContext.ProductLocations.Find(productLocationId);
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

                if (prod.ProductLocations.Any(x => x.IsDeleted != true) && prod.ProductLocations.Any(a => a.Locations.LocationCode != "external" && a.IsDeleted != true))
                {
                    prod.DontMonitorStock = false;
                }
                else
                {
                    prod.DontMonitorStock = true;

                    var externalLocation = _currentDbContext.Locations.Where(x => x.LocationCode == "external").FirstOrDefault();

                    var newLocation = new ProductLocations { ProductId = prod.ProductId, LocationId = externalLocation.LocationId, DateCreated = DateTime.UtcNow, CreatedBy = caCurrent.CurrentUser().UserId, TenantId = currentTenantId, IsActive = true };
                    _currentDbContext.ProductLocations.Add(newLocation);
                }
            }

            _currentDbContext.SaveChanges();

            return true;
        }

        public List<Tuple<string, string, decimal, bool>> AllocatedProductDetail(int productId, int WarehouseId, int details)
        {
            if (details == 1)
            {
                var allocatedOrders = _currentDbContext.OrderDetail.Where(m => m.ProductId == productId &&
                                                                               (m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder
                    || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Loan || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Samples
                    || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Exchange || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut || m.Order.InventoryTransactionTypeId ==
                    InventoryTransactionTypeEnum.Wastage)
                                                                               && m.Order.WarehouseId == WarehouseId &&
                                m.Order.OrderStatusID != OrderStatusEnum.Complete && m.Order.OrderStatusID != OrderStatusEnum.Cancelled && m.Order.OrderStatusID != OrderStatusEnum.PostedToAccounts && m.Order.OrderStatusID != OrderStatusEnum.Invoiced
                                && m.Order.IsDeleted != true).Select(u => u.Order).ToList();

                List<Tuple<string, string, decimal, bool>> detail = new List<Tuple<string, string, decimal, bool>>();
                foreach (var item in allocatedOrders)
                {
                    var OrderNumber = item.OrderNumber;
                    var AccountNumber = item.Account?.AccountCode;
                    var itemsOnSalesOrders = item.OrderDetails.Where(p => p.ProductId == productId && p.IsDeleted != true).Select(x => x.Qty).DefaultIfEmpty(0).Sum();

                    var itemsDispatched = _currentDbContext.Order
                .Where(m => m.OrderID==item.OrderID &&
                    (m.InventoryTransactionTypeId ==
                        InventoryTransactionTypeEnum.SalesOrder || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.WorksOrder
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Loan ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Samples
                                                                || m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Exchange ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.TransferOut ||
                                                                m.InventoryTransactionTypeId ==
                                                                InventoryTransactionTypeEnum.Wastage) &&
                    m.WarehouseId == WarehouseId &&
                    m.OrderStatusID != OrderStatusEnum.Complete && m.OrderStatusID != OrderStatusEnum.Cancelled &&
                    m.OrderStatusID != OrderStatusEnum.PostedToAccounts && m.OrderStatusID != OrderStatusEnum.Invoiced
                    && m.IsDeleted != true && m.IsCancel != true && m.DirectShip != true)
                .Select(m => m.OrderProcess.Where(u => u.IsDeleted != true).Select(o => o.OrderProcessDetail
                    .Where(p => p.ProductId == productId && p.IsDeleted != true).Select(q => q.QtyProcessed)
                    .DefaultIfEmpty(0)
                    .Sum()).DefaultIfEmpty(0).Sum()).DefaultIfEmpty(0).Sum();

                    var itemsAllocated = itemsOnSalesOrders - itemsDispatched;
                    bool directship = item?.DirectShip ?? false;
                    if (itemsAllocated > 0 && detail.Count(u => u.Item2 == OrderNumber) <= 0)
                    {
                        detail.Add(new Tuple<string, string, decimal, bool>(AccountNumber, OrderNumber, itemsAllocated, directship));
                    }
                }
                return detail;
            }
            else
            {
                var orderId = _currentDbContext.OrderDetail.Where(m => m.ProductId == productId && (m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn
               || m.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns) && m.Order.WarehouseId == WarehouseId &&
                           m.Order.OrderStatusID != OrderStatusEnum.Complete && m.Order.OrderStatusID != OrderStatusEnum.Cancelled && m.Order.OrderStatusID != OrderStatusEnum.PostedToAccounts && m.Order.OrderStatusID != OrderStatusEnum.Invoiced
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

        public ProductKitMap SaveProductKit(int kitId, decimal quantity, int ProductId, int UserId, int? ProductKitTypeId)
        {
            var productKit = _currentDbContext.ProductKitMaps.FirstOrDefault(u => u.Id == kitId);
            if (productKit != null)
            {
                productKit.KitProductId = ProductId;
                productKit.Quantity = quantity;
                if (ProductKitTypeId.HasValue)
                {
                    productKit.ProductKitTypeId = ProductKitTypeId;
                }
                productKit.UpdateCreatedInfo(UserId);
                _currentDbContext.Entry(productKit).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return productKit;
        }

        public IEnumerable<ProductMaster> GetAllProductProcessByPallet(int tenantId)
        {
            return _currentDbContext.ProductMaster.Where(a => a.TenantId == tenantId && a.IsDeleted != true && a.ProcessByPallet == true);
        }

        public string CreatePalletTracking(PalletTracking palletTrackingData, int NoOfLabels)
        {
            List<int> palletTrackingId = new List<int>();
            for (int i = 0; i < NoOfLabels; i++)
            {
                var palletTracking = palletTrackingData.DeepClone();
                var palletSerials = new List<string>();
                var palletSerial = string.Empty;
                while (palletSerials.Contains(palletSerial) || string.IsNullOrEmpty(palletSerial))
                {
                    
                    palletSerial = DateTime.Now.Date.ToString("ddMMyy") +
                                              DateTime.Now.ToString("HHmmss") +
                                              GenerateRandomNo();
                }

                palletTracking.PalletSerial = palletSerial;

                _currentDbContext.PalletTracking.Add(palletTracking);
                _currentDbContext.SaveChanges();
                palletTrackingId.Add(palletTracking.PalletTrackingId);
                palletSerials.Add(palletTracking.PalletSerial);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            return string.Join(",", palletTrackingId.ToArray());
        }

        public List<string> CreatePalletTracking(LabelPrintViewModel requestData, int tenantId, int warehouseId)
        {
            var palletTrackingData = new PalletTracking
            {
                ProductId = requestData.ProductId,
                BatchNo = requestData.BatchNumber,
                TotalCases = requestData.Cases.Value,
                RemainingCases = requestData.Cases.Value,
                Comments = requestData.Comments,
                Status = PalletTrackingStatusEnum.Created,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                TenantId = tenantId,
                WarehouseId = warehouseId
            };
            if (requestData.RequiresExpiryDate.HasValue && requestData.RequiresExpiryDate==true)
            {
                palletTrackingData.ExpiryDate = requestData.LabelDate;
            }
            var palletSerials = new List<string>();
            var palletSerial = string.Empty;

            for (int i = 0; i < requestData.PalletsCount; i++)
            {
                var palletTracking = palletTrackingData.DeepClone();
                while (palletSerials.Contains(palletSerial) || string.IsNullOrEmpty(palletSerial))
                {
                    palletSerial = DateTime.Now.Date.ToString("ddMMyy") +
                                              DateTime.Now.ToString("HHmmss") +
                                              GenerateRandomNo();
                }

                palletTracking.PalletSerial = palletSerial;

                _currentDbContext.PalletTracking.Add(palletTracking);
                _currentDbContext.SaveChanges();
                palletSerials.Add(palletTracking.PalletSerial);
            }

            return palletSerials;
        }

        public string GenerateRandomNo()
        {
            Random _random = new Random();
            return _random.Next(0, 9999).ToString("D4");
        }

        public PalletTracking GetVerifedPalletAsync(VerifyPalletTrackingSync verifyPalletTracking)
        {

            var palletTracking = new PalletTracking();

            if (verifyPalletTracking.InventoryTransactionType == (int)InventoryTransactionTypeEnum.PurchaseOrder)
            {
                palletTracking = _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == verifyPalletTracking.PalletSerial && (!verifyPalletTracking.ProductId.HasValue || u.ProductId == verifyPalletTracking.ProductId) && u.TenantId == verifyPalletTracking.TenantId && u.WarehouseId == verifyPalletTracking.WarehouseId && u.Status == PalletTrackingStatusEnum.Created);
            }
            else if (verifyPalletTracking.InventoryTransactionType == (int)InventoryTransactionTypeEnum.SalesOrder)
            {
                palletTracking = _currentDbContext.PalletTracking.AsNoTracking().FirstOrDefault(u => u.PalletSerial == verifyPalletTracking.PalletSerial && (!verifyPalletTracking.ProductId.HasValue || u.ProductId == verifyPalletTracking.ProductId) && u.TenantId == verifyPalletTracking.TenantId && u.WarehouseId == verifyPalletTracking.WarehouseId && u.Status == PalletTrackingStatusEnum.Active);
            }

            return palletTracking;
        }

        public List<OrderDetail> GetOrderDetailsByProductId(int productId)
        {
            return _currentDbContext.OrderDetail.Where(u => u.ProductId == productId && u.IsDeleted != true).ToList();
            
        }
        public List<InvoiceDetail> GetInvoiceDetailsByProductId(int productId)
        {
            return _currentDbContext.InvoiceDetails.Where(u => u.ProductId == productId && u.IsDeleted != true).ToList();

        }
    }
}