using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Elmah;
using Elmah.ContentSyndication;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models.AdyenPayments;
using Microsoft.Ajax.Utilities;

namespace Ganedata.Core.Services
{
    public class ProductLookupService : IProductLookupService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        public ProductLookupService(IApplicationContext currentDbContext,ITenantWebsiteService tenantWebsiteService)
        {
            _currentDbContext = currentDbContext;
            _tenantWebsiteService= tenantWebsiteService;
        }

        public IEnumerable<ProductLotOptionsCodes> GetAllValidProductLotOptionsCodes()
        {
            return _currentDbContext.ProductLotOptionsCodes;
        }
        public IEnumerable<ProductLotProcessTypeCodes> GetAllValidProductLotProcessTypeCodes()
        {
            return _currentDbContext.ProductLotProcessTypeCodes;
        }
        public IEnumerable<Locations> GetAllValidProductLocations(int tenantId, int warehouseId, int filterForProductId = 0)
        {
            var locationIds = new List<int>();
            if (filterForProductId > 0)
            {
                locationIds = _currentDbContext.ProductLocations.Where(x => x.IsDeleted != true && x.ProductId == filterForProductId).Select(y => y.LocationId).Distinct().ToList();
            }
            return _currentDbContext.Locations.Where(m => m.TenantId == tenantId && m.WarehouseId == warehouseId && m.IsDeleted != true && (filterForProductId == 0 || locationIds.Contains(m.LocationId)));
        }
        public IEnumerable<ProductAttributes> GetAllValidProductAttributes()
        {
            return _currentDbContext.ProductAttributes.OrderBy(o => o.AttributeName);
        }

        public IEnumerable<ProductAttributeMap> GetAllValidProductAttributeMaps(int TenantId)
        {
            return _currentDbContext.ProductAttributeMaps.Where(u => u.IsDeleted != true && u.TenantId == TenantId);
        }
        public IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValues()
        {
            return _currentDbContext.ProductAttributeValues.Where(m => m.IsDeleted != true);
        }
        public IEnumerable<ProductAttributeValuesMap> GetAllValidProductAttributeValuesMapsByProductId(int productId)
        {
            return _currentDbContext.ProductAttributeValuesMap
                .Where(a => a.ProductId == productId && a.IsDeleted != true);
        }

        public List<ProductKitType> GetProductKitTypes(List<int?> kitIds)
        {
            return _currentDbContext.ProductKitTypes.Where(u => kitIds.Contains(u.Id) && u.IsDeleted != true && u.IsActive == true)
                                                    .OrderBy(u => u.SortOrder)
                                                    .ToList();
        }

        public IEnumerable<ProductKitType> GetProductKitTypes(int TenantId)
        {
            return _currentDbContext.ProductKitTypes.Where(u => u.TenentId == TenantId && u.IsDeleted != true);
        }

        public ProductKitType GetProductKitTypeById(int productKitTypeId)
        {
            return _currentDbContext.ProductKitTypes.Find(productKitTypeId);
        }

        public Dictionary<string, List<ProductAttributeValues>> GetAllValidProductAttributeValuesByProductIds(IQueryable<ProductMaster> product)
        {
            var AttributeValueId = (from prd in product
                                    join pavm in _currentDbContext.ProductAttributeValuesMap
                                    on prd.ProductId equals pavm.ProductId
                                    join pav in _currentDbContext.ProductAttributeValues
                                    on pavm.AttributeValueId equals pav.AttributeValueId
                                    select new
                                    {
                                        AttributeId = pavm.ProductAttributeValues.AttributeId.ToString(),
                                        ProductAttributeValues = pavm.ProductAttributeValues

                                    }).Distinct().ToList();


            //var valueAttributes = AttributeValueId.Select(u => u.AttributeValueId).ToList();
            //var productAttributs=  _currentDbContext.ProductAttributeValues.Where(u => valueAttributes.Contains(u.AttributeValueId)).ToList();
            return AttributeValueId.GroupBy(o => o.AttributeId).ToDictionary(g => g.Key.ToString(), g => g.Select(u => u.ProductAttributeValues).ToList());

        }

        public IQueryable<ProductMaster> GetAllValidProductGroupById(int? productGroupId, int? departmentId = null)
        {
            return _currentDbContext.ProductMaster
                .Where(a => ((!productGroupId.HasValue || a.ProductGroupId == productGroupId) && (!departmentId.HasValue || a.DepartmentId == departmentId)) && a.IsDeleted != true).Include(u => u.ProductManufacturer);
        }
        public IQueryable<ProductMaster> GetAllValidProductGroupAndDeptByName(int siteId, string category = "", string manufacturerName = "", string ProductName = "")
        {
            ProductName = ProductName?.Trim();
            int? categoryId = _currentDbContext.WebsiteNavigations.FirstOrDefault(u => u.Name == category && u.IsDeleted != true)?.Id;
            int? manufacturerId = _currentDbContext.ProductManufacturers.FirstOrDefault(u => u.Name == manufacturerName && u.IsDeleted != true)?.Id;
            List<int> websiteProductIds = _currentDbContext.ProductsWebsitesMap.Where(x => x.SiteID == siteId && x.IsActive == true && x.IsDeleted != true).Select(a => a.ProductId).ToList();
            List<int> productIds = _currentDbContext.ProductsNavigationMaps.Where(u => (u.NavigationId == categoryId || !categoryId.HasValue) && u.IsDeleted != true).Select(x => x.ProductsWebsitesMap.ProductId).ToList();

            return _currentDbContext.ProductMaster
                .Where(a => websiteProductIds.Contains(a.ProductId) && a.IsActive == true && a.IsDeleted != true && (productIds.Contains(a.ProductId) || string.IsNullOrEmpty(category))
                && (!manufacturerId.HasValue || a.ManufacturerId == manufacturerId) && (string.IsNullOrEmpty(ProductName) || a.Name.Contains(ProductName))
                && (string.IsNullOrEmpty(ProductName) || a.SKUCode.Contains(ProductName) || a.Description.Contains(ProductName) || a.ProductGroup.ProductGroup.Contains(ProductName)))
                .Include(u => u.ProductManufacturer);
        }

        public IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(IQueryable<ProductMaster> productMasters)
        {

            var productmanufacturerId = productMasters.Select(u => u.ManufacturerId).ToList();
            return _currentDbContext.ProductManufacturers.Where(u => productmanufacturerId.Contains(u.Id));

        }
        public List<Tuple<string, string>> AllPriceListAgainstGroupAndDept(IQueryable<ProductMaster> productMasters)
        {
            List<Tuple<string, string>> Prices = new List<Tuple<string, string>>();

            var avarageValue = productMasters.Select(u => u.SellPrice);
            var avgValue = avarageValue.Average();
            var averageInt = Convert.ToInt32(avgValue);
            var minValue = Convert.ToInt32(avarageValue.Min());
            var maxValue = Convert.ToInt32(avarageValue.Max());
            var centervalue = Convert.ToInt32(avarageValue.FirstOrDefault(u => u.Value > avgValue && u.Value < maxValue) == null ? 0 : avarageValue.FirstOrDefault(u => u.Value > avgValue && u.Value < maxValue));
            Prices.Add(new Tuple<string, string>(minValue.ToString(), averageInt.ToString()));
            Prices.Add(new Tuple<string, string>(averageInt.ToString(), centervalue.ToString()));
            Prices.Add(new Tuple<string, string>(centervalue.ToString(), maxValue.ToString()));
            return Prices;
        }

        public Dictionary<int, string> GetAllValidSubCategoriesByDepartmentAndGroup(List<int> productIds)
        {
            var productsCategorys = _currentDbContext.ProductMaster.Where(p => productIds.Contains(p.ProductId) && p.ProductCategory != null)
                                                                   .Select(u => u.ProductCategory)
                                                                   .ToList();

            productsCategorys.AddRange(_currentDbContext.ProductKitMaps.Where(a => productIds.Contains(a.ProductId) &&
                                                                a.IsDeleted != true &&
                                                                a.ProductKitTypes != null && a.ProductKitTypes.UseInParentCalculations == true &&
                                                                a.IsActive &&
                                                                a.KitProductMaster.ProductCategory != null)
                                                 .Select(a => a.KitProductMaster.ProductCategory)
                                                 .ToList());

            return productsCategorys
                    .GroupBy(c => c)
                    .Select(c => c.Key)
                    .OrderBy(c => c.ProductCategoryName)
                    .ToDictionary(c => c.ProductCategoryId, c => c.ProductCategoryName); ;
        }

        public IEnumerable<WebsiteNavigation> GetWebsiteNavigationCategoriesList(int? parentCategoryId, int siteId)
        {
            return _currentDbContext.WebsiteNavigations.Where(u => u.Parent.Id == parentCategoryId &&
                                                                   u.Type == WebsiteNavigationType.Category &&
                                                                   u.IsDeleted != true &&
                                                                   u.SiteID == siteId &&
                                                                   u.IsActive);
        }

        public IEnumerable<ProductAttributeValuesMap> GetAllValidProductAttributeValuesMap()
        {
            return _currentDbContext.ProductAttributeValuesMap.Where(m => m.IsDeleted != true);
        }
        public IEnumerable<ProductSCCCodes> GetAllProductSccCodesByProductId(int productId, int tenantId)
        {
            return _currentDbContext.ProductSCCCodes.Where(a => a.ProductId == productId && a.IsDeleted != true && a.TenantId == tenantId);
        }
        public IEnumerable<ProductLocations> GetAllProductLocationsByProductId(int productId, int warehouseId)
        {
            return _currentDbContext.ProductLocations.Where(
                a => a.Locations.IsDeleted != true && a.IsDeleted != true && a.ProductId == productId);
        }
        public IQueryable<ProductMaster> ApplyFixedFilters(IQueryable<ProductMaster> products, string filterString, int siteId, int? accountId)
        {
            try
            {
                var filters = ReadFiltersString(filterString);

                if (filters == null || filters.Count <= 0)
                {
                    return products;
                }

                var selectedProducts = products.Select(p => new
                {
                    p.ProductId,
                    p.SellPrice,
                    p.ManufacturerId,
                    KitProducts = p.ProductKitItems.Select(k => new
                    {
                        k.ProductKitTypes.UseInParentCalculations,
                        k.KitProductMaster.ProductId,
                        k.KitProductMaster.ManufacturerId,
                        CategoryId = k.KitProductMaster.ProductCategoryId
                    }),
                    p.ProductCategoryId,
                    p.InventoryStocks
                }).ToList();

                if (filters.TryGetValue("prices", out List<string> pricesData))
                {
                    var prices = pricesData.FirstOrDefault()?.Split('-').Select(p => Convert.ToDecimal(p)).ToList();
                    if (prices.Count() > 0)
                    {
                        var filteredProductsIds = selectedProducts
                                                            .Select(u =>
                                                            {
                                                                var sellPrice = _tenantWebsiteService.GetPriceForProduct(u.ProductId, siteId, accountId);
                                                                return (sellPrice == null || (sellPrice >= prices.Min() && sellPrice <= prices.Max())) ? u.ProductId : (int?)null;
                                                            })
                                                            .Where(p => p != null)
                                                            .Distinct()
                                                            .ToList();
                        selectedProducts = selectedProducts.Where(p => filteredProductsIds.Contains(p.ProductId)).ToList();
                    }
                }

                if (filters.TryGetValue("brands", out List<string> brandsData))
                {
                    var selectedBrands = brandsData.Select(v => Convert.ToInt32(v)).Where(i => i > 0).ToList();
                    var filteredProductsIds = selectedProducts
                                                    .Where(u => selectedBrands.Contains(u.ManufacturerId ?? 0) || u.KitProducts.Any(k => k.UseInParentCalculations == true && selectedBrands.Contains(k.ManufacturerId ?? 0)))
                                                    .Select(p => p.ProductId)
                                                    .Distinct()
                                                    .ToList();

                    selectedProducts = selectedProducts.Where(p => filteredProductsIds.Contains(p.ProductId)).ToList();
                }

                if (filters.TryGetValue("types", out List<string> typessData))
                {
                    var selectedTypes = typessData.Select(v => Convert.ToInt32(v)).Where(i => i > 0).ToList();
                    var filteredProductsIds = selectedProducts
                                                    .Where(u => selectedTypes.Contains(u.ProductCategoryId ?? 0) || u.KitProducts.Any(k => k.UseInParentCalculations == true && selectedTypes.Contains(k.CategoryId ?? 0)))
                                                    .Select(p => p.ProductId)
                                                    .Distinct()
                                                    .ToList();

                    selectedProducts = selectedProducts.Where(p => filteredProductsIds.Contains(p.ProductId)).ToList();
                }

                if (filters.TryGetValue("instockonly", out List<string> inStockData))
                {
                    if(inStockData != null && inStockData.Any(s => Convert.ToBoolean(s)))
                    selectedProducts = selectedProducts.Where(p => p.InventoryStocks != null && p.InventoryStocks.Any()).ToList();
                }

                var selectedProductsIds = selectedProducts.Select(p => p.ProductId).ToList();

                products = products.Where(u => selectedProductsIds.Contains(u.ProductId));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return products;
        }

        public IQueryable<ProductMaster> ApplyAttributeFilters(IQueryable<ProductMaster> products, string filterString, int siteId)
        {
            try
            {
                var filters = ReadFiltersString(filterString);

                if (filters == null || filters.Count <= 0)
                {
                    return products;
                }

                var attributesFilters = filters.Where(i => i.Key.Length > 0 && !i.Key.Equals("brands") && !i.Key.Equals("prices") && !i.Key.Equals("types"));

                if (products?.Count() > 0 && attributesFilters.Count() > 0)
                {
                    var selectedProducts = products.Select(p => new
                    {
                        p.ProductId,
                        p.ProductType,
                        KitProducts = p.ProductKitItems.Select(k => new
                        {
                            k.ProductKitTypes.UseInParentCalculations,
                            k.KitProductMaster.ProductId
                        })
                    }).ToList();

                    var allChildAndParentProductsIds = selectedProducts.Where(p => p.ProductType == ProductKitTypeEnum.Grouped)
                                                                        .SelectMany(p => p.KitProducts)
                                                                        .Where(k => k.UseInParentCalculations == true)
                                                                        .Select(k => k.ProductId)
                                                                        .ToList();

                    allChildAndParentProductsIds.AddRange(selectedProducts.Where(p => p.ProductType == ProductKitTypeEnum.ProductByAttribute)
                                                                          .SelectMany(p => p.KitProducts)
                                                                          .Select(k => k.ProductId)
                                                                          .ToList());

                    allChildAndParentProductsIds.AddRange(selectedProducts.Select(k => k.ProductId));

                    allChildAndParentProductsIds = allChildAndParentProductsIds.Distinct().ToList();

                    var kitProductsAttributes = _currentDbContext.ProductMaster.Where(p => allChildAndParentProductsIds.Contains(p.ProductId))
                                                                                .Select(p => new
                                                                                {
                                                                                    p.ProductId,
                                                                                    ProductAttributeValuesMaps = p.ProductAttributeValuesMap.Select(a => new
                                                                                    {
                                                                                        a.AttributeValueId,
                                                                                        a.ProductAttributeValues.AttributeId,
                                                                                        a.IsDeleted
                                                                                    })
                                                                                })
                                                                                .ToList();

                    foreach (var item in attributesFilters)
                    {
                        if (int.TryParse(item.Key, out int attributeId))
                        {
                            var selectedItemsIds = item.Value.Select(v => Convert.ToInt32(v)).Where(i => i > 0).ToList();
                            var filteredProductsIds = kitProductsAttributes.Where(p => p.ProductAttributeValuesMaps.Any(u => selectedItemsIds.Contains(u.AttributeValueId) &&
                                                                                                                                attributeId == u.AttributeId &&
                                                                                                                                u.IsDeleted != true))
                                                                        .Select(p => p.ProductId)
                                                                        .ToList();

                            kitProductsAttributes = kitProductsAttributes.Where(p => filteredProductsIds.Contains(p.ProductId)).ToList();
                            selectedProducts = selectedProducts.Where(p => filteredProductsIds.Contains(p.ProductId) ||
                                                                           p.KitProducts.Any(k => (k.UseInParentCalculations == true || p.ProductType == ProductKitTypeEnum.ProductByAttribute) &&
                                                                                                   filteredProductsIds.Contains(k.ProductId)))
                                                               .ToList();
                        }
                    }

                    var selectedProductsIds = selectedProducts.Select(p => p.ProductId).ToList();

                    products = products.Where(u => selectedProductsIds.Contains(u.ProductId));
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return products;
        }

        public Dictionary<string, List<string>> ReadFiltersString(string filterString)
        {
            if (string.IsNullOrEmpty(filterString))
            {
                return null;
            }

            return filterString.Split('|')
                                    .Where(i => !string.IsNullOrEmpty(i))
                                    .Select(a =>
                                    {
                                        var filterData = a.Split('>').Where(s => !string.IsNullOrEmpty(s?.Trim())).ToList();

                                        return filterData != null && filterData.Count() == 2 ?
                                                new
                                                {
                                                    Key = filterData.First().Replace("_", " ").Replace("^", ",").Split(':').Last(),
                                                    Values = filterData.Last().Split(',')
                                                                                    .Select(s => s.Replace("_", " ").Replace("^", ",").Split(':').LastOrDefault())
                                                                                    .Where(v => !string.IsNullOrEmpty(v?.Trim()))
                                                                                    .Distinct()
                                                                                    .ToList()
                                                } :
                                                null;
                                    })
                                    .GroupBy(a => a.Key)
                                    .ToDictionary(a => a.Key, a => a.SelectMany(b => b.Values).GroupBy(d => d).Select(d => d.Key).ToList());
        }

        public Locations GetLocationById(int locationId)
        {
            return _currentDbContext.Locations.Find(locationId);
        }

        public ProductGroups GetProductGroupById(int productGroupId)
        {
            return _currentDbContext.ProductGroups.Find(productGroupId);
        }

        public ProductCategory GetProductCategoryById(int productCategoryId)
        {
            return _currentDbContext.ProductCategories.Find(productCategoryId);
        }

        public PalletType GetPalletTypeById(int palletTypeId)
        {
            return _currentDbContext.PalletTypes.Find(palletTypeId);
        }

        public ProductGroups GetProductGroupByName(string groupName)
        {
            return _currentDbContext.ProductGroups.FirstOrDefault(p => p.ProductGroup.Equals(groupName) && p.IsDeleted != true);
        }

        public ProductCategory GetProductCategoryByName(string categoryName)
        {
            return _currentDbContext.ProductCategories.FirstOrDefault(p => p.ProductCategoryName.Equals(categoryName) && p.IsDeleted != true);
        }

        public ProductKitType GetProductKitTypeByName(string productKitTypeName)
        {
            return _currentDbContext.ProductKitTypes.FirstOrDefault(p => p.Name.Equals(productKitTypeName) && p.IsDeleted != true);
        }
        public PalletType GetPalletTypeByName(string palletType)
        {
            return _currentDbContext.PalletTypes.FirstOrDefault(p => p.Description.Equals(palletType) && p.IsDeleted != true);
        }

        public ProductSCCCodes GetProductSccCodesById(int productSccCodesId)
        {
            return _currentDbContext.ProductSCCCodes.Find(productSccCodesId);
        }

        public ProductAttributeValues GetProductAttributeValueById(int productAttributeValueId)
        {
            return _currentDbContext.ProductAttributeValues.Find(productAttributeValueId);
        }
        public ProductAttributes GetProductAttributeById(int productAttributeId)
        {
            return _currentDbContext.ProductAttributes.Find(productAttributeId);
        }

        public ProductSpecialAttributePrice SaveProductAttributeValuesMap(ProductSpecialAttributePrice model, int userId, int tenantId)
        {
            var priceGroupDetail = _currentDbContext.ProductSpecialPrices.FirstOrDefault(m=> m.ProductID==model.ProductId && m.PriceGroupID==model.PriceGroupID);
            if (priceGroupDetail == null)
            {
                priceGroupDetail = new TenantPriceGroupDetail()
                {
                    ProductID = model.ProductId,
                    PriceGroupID = model.PriceGroupID,
                    CreatedBy = userId,
                    DateCreated = DateTime.Now,
                    SpecialPrice = model.AttributeSpecificPrice??0,
                    TenantId = tenantId
                };
                _currentDbContext.ProductSpecialPrices.Add(priceGroupDetail);
                _currentDbContext.SaveChanges();
            }

            var specialPrice = _currentDbContext.ProductSpecialAttributePrices.FirstOrDefault(m => m.ProductId==model.ProductId && m.ProductAttributeValueId==model.ProductAttributeValueId && m.PriceGroupID==model.PriceGroupID && m.IsDeleted !=true);
            if (specialPrice == null)
            {
                specialPrice = new ProductSpecialAttributePrice()
                {
                    AttributeSpecificPrice = model.AttributeSpecificPrice,
                    TenantId = tenantId,
                    ProductId = model.ProductId,
                    PriceGroupID = model.PriceGroupID,
                    ProductAttributeValueId = model.ProductAttributeValueId,
                    ProductAttributeId = model.ProductAttributeId,
                    PriceGroupDetailID = priceGroupDetail.PriceGroupDetailID,
                    CreatedBy = userId,
                    DateCreated = DateTime.Now
                };

                _currentDbContext.ProductSpecialAttributePrices.Add(specialPrice);
            }
            else
            {
                specialPrice.AttributeSpecificPrice = model.AttributeSpecificPrice;
                specialPrice.UpdatedBy = userId;
                specialPrice.DateUpdated = DateTime.Now;
                _currentDbContext.Entry(specialPrice);
            }

            _currentDbContext.SaveChanges();
            return specialPrice;
        }
        public bool DeleteProductAttributeValuesMap(int id, int userId)
        {
            var map = _currentDbContext.ProductSpecialAttributePrices.FirstOrDefault(m => m.ProductSpecialAttributePriceId == id);
            if (map == null) return false;

            map.IsDeleted = true;
            map.DateUpdated = DateTime.Now;
            map.UpdatedBy = userId;

            _currentDbContext.Entry(map).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return true;
        }

        public ProductAttributeValuesMap GetProductAttributeValueMap(int productId, int attributeValueId)
        {
            return _currentDbContext.ProductAttributeValuesMap.FirstOrDefault(a => a.ProductId == productId && a.AttributeValueId == attributeValueId);
        }
        public ProductAttributeValuesMap GetProductAttributeValuesMapById(int id)
        {
            return _currentDbContext.ProductAttributeValuesMap.FirstOrDefault(a => a.Id == id && a.IsDeleted != true);
        }

        public ProductAttributes SaveProductAttribute(string attributeName, int sortOrder, bool isColorTyped, int? attributeId = null,
            bool isPriced=false)
        {
            var att = _currentDbContext.ProductAttributes.FirstOrDefault(a => a.AttributeName.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase) && (!attributeId.HasValue || a.AttributeId == attributeId));
            if (att != null)
            {

                att.AttributeName = attributeName;
                att.SortOrder = sortOrder;
                att.IsColorTyped = isColorTyped;
                att.IsPriced = isPriced;
                _currentDbContext.Entry(att).State = EntityState.Modified;
            }
            else
            {
                att = new ProductAttributes()
                {
                    AttributeName = attributeName,
                    SortOrder = sortOrder,
                    IsColorTyped = isColorTyped,
                    IsPriced = isPriced
                };

                _currentDbContext.Entry(att).State = EntityState.Added;
            }

            _currentDbContext.SaveChanges();
            return att;
        }

        public ProductAttributeValues SaveProductAttributeValue(int attributeId, string attributeValue, int sortOrder, string color, int userId = 0, int? attributeValueId = null)
        {
            var value = _currentDbContext.ProductAttributeValues.FirstOrDefault(m => m.AttributeId == attributeId && m.Value == attributeValue && (!attributeValueId.HasValue || m.AttributeValueId == attributeValueId));
            if (value == null && userId > 0)
            {
                value = new ProductAttributeValues()
                {
                    AttributeId = attributeId,
                    Value = attributeValue,
                    SortOrder = sortOrder,
                    Color = color
                };
                value.UpdateCreatedInfo(userId);
                _currentDbContext.Entry(value).State = EntityState.Added;


            }
            else
            {

                value.AttributeId = attributeId;
                value.Value = attributeValue;
                value.SortOrder = sortOrder;
                value.Color = color;
                value.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(value).State = EntityState.Modified;

            }


            _currentDbContext.SaveChanges();
            return value;
        }

         


        public bool SaveProductAttributeValueMap(int attributeValueId, int userId, int tenantId, int productId, int? productAttributeValueMapId)
        {
            if (productAttributeValueMapId != null && productAttributeValueMapId > 0 && attributeValueId > 0)
            {
                var previousAttributeMap = _currentDbContext.ProductAttributeValuesMap.FirstOrDefault(p => p.Id == productAttributeValueMapId &&
                                                                                                           p.TenantId == tenantId);

                if (previousAttributeMap != null)
                {
                    previousAttributeMap.IsDeleted = false;
                    previousAttributeMap.UpdatedBy = userId;
                    previousAttributeMap.AttributeValueId = attributeValueId;
                    previousAttributeMap.DateUpdated = DateTime.Now;
                }

            }
            else if (attributeValueId > 0)
            {
                var previousAttributeMap = _currentDbContext.ProductAttributeValuesMap.FirstOrDefault(p => p.ProductId == productId &&
                                                                                                           p.AttributeValueId == attributeValueId &&
                                                                                                           p.TenantId == tenantId);

                if (previousAttributeMap != null)
                {
                    previousAttributeMap.IsDeleted = false;
                    previousAttributeMap.UpdatedBy = userId;
                    previousAttributeMap.DateUpdated = DateTime.Now;
                }
                else
                {
                    var valueMap = new ProductAttributeValuesMap
                    {
                        AttributeValueId = attributeValueId,
                        CreatedBy = userId,
                        TenantId = tenantId,
                        DateCreated = DateTime.UtcNow,
                        ProductId = productId,
                    };

                    _currentDbContext.ProductAttributeValuesMap.Add(valueMap);
                }
            }

            _currentDbContext.SaveChanges();

            return true;
        }

        public void DeleteProductAttributeValuesMap(int productId, int attributeValueId, int userId, int tenantId)
        {
            var currentm = GetProductAttributeValueMap(productId, attributeValueId);

            currentm.IsDeleted = true;
            currentm.UpdatedBy = userId;
            currentm.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(currentm).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }




        public Locations SaveProductLocation(Locations model, int warehouseId, int tenantId, int userId, int productId = 0)
        {
            if (model.LocationId < 1)
            {

                var cLocation =
                    _currentDbContext.Locations.FirstOrDefault(a => a.WarehouseId == warehouseId &&
                                                                    a.TenantId == tenantId && a.LocationCode ==
                                                                    model.LocationCode);
                if (cLocation != null)
                    throw new Exception("Location Code already exists!");
                model.IsActive = true;
                model.IsDeleted = false;
                model.TenantId = tenantId;

                model.DateUpdated = DateTime.UtcNow;
                model.DateCreated = DateTime.UtcNow;
                model.CreatedBy = userId;
                model.UpdatedBy = userId;

                model.WarehouseId = warehouseId;

                _currentDbContext.Entry(model).State = EntityState.Added;
                _currentDbContext.SaveChanges();

                if (productId > 0)
                {
                    var map = new ProductLocations
                    {
                        LocationId = model.LocationId,
                        ProductId = productId,
                        TenantId = tenantId,
                        DateCreated = DateTime.UtcNow,
                        CreatedBy = userId
                    };
                    _currentDbContext.ProductLocations.Add(map);
                }
            }
            else
            {
                var cLocation =
                    _currentDbContext.Locations.AsNoTracking().FirstOrDefault(a => a.WarehouseId == warehouseId &&
                                                                    a.TenantId == tenantId && a.LocationCode ==
                                                                    model.LocationCode);

                model.UpdatedBy = userId;
                model.DateUpdated = DateTime.UtcNow;
                model.DateCreated = cLocation.DateCreated;
                model.WarehouseId = cLocation.WarehouseId;
                model.TenantId = cLocation.TenantId;
                model.CreatedBy = cLocation.CreatedBy;
                model.IsActive = cLocation.IsActive;
            }

            _currentDbContext.Entry(model).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return model;
        }

        public void DeleteProductLocation(int productId, int locationId, int warehouseId, int tenantId, int userId)
        {
            var current = _currentDbContext.Locations.Find(locationId);
            current.IsDeleted = true;
            current.UpdatedBy = userId;
            current.DateUpdated = DateTime.UtcNow;

            var maps = _currentDbContext.ProductLocations.Where(a => a.LocationId == locationId && a.ProductId == productId);
            foreach (var cMap in maps)
            {
                cMap.IsDeleted = true;
                cMap.UpdatedBy = userId;
                cMap.DateUpdated = DateTime.UtcNow;
            }
            _currentDbContext.SaveChanges();
        }
        public ProductSCCCodes SaveSccCode(ProductSCCCodes model, int productId, int userId, int tenantId)
        {
            if (model.ProductSCCCodeId == 0)
            {
                model.CreatedBy = userId;
                model.DateCreated = DateTime.UtcNow;
                model.ProductId = productId;
                model.TenantId = tenantId;
                _currentDbContext.ProductSCCCodes.Add(model);
            }
            else
            {
                model.UpdatedBy = userId;
                model.DateUpdated = DateTime.UtcNow;
                _currentDbContext.ProductSCCCodes.Attach(model);
                var entry = _currentDbContext.Entry(model);
                entry.Property(e => e.SCC).IsModified = true;
                entry.Property(e => e.Quantity).IsModified = true;
            }
            _currentDbContext.SaveChanges();

            return model;
        }

        public void DeleteSccCode(int productSccCodesId, int userId)
        {
            var current = GetProductSccCodesById(productSccCodesId);
            current.IsDeleted = true;
            current.UpdatedBy = userId;
            current.DateUpdated = DateTime.UtcNow;
            _currentDbContext.SaveChanges();
        }
        public ProductGroups CreateProductGroup(ProductGroups model, int userId, int tenantId)
        {
            var pgrp = _currentDbContext.ProductGroups.FirstOrDefault(a => a.ProductGroup.Equals(model.ProductGroup, StringComparison.InvariantCultureIgnoreCase));
            if (pgrp != null)
                throw new Exception("Product group already exsists");

            var pGroup = new ProductGroups()
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                ProductGroup = model.ProductGroup,
                DepartmentId = model.DepartmentId,
                TenentId = tenantId,
                UpdatedBy = userId
            };
            _currentDbContext.ProductGroups.Add(pGroup);
            _currentDbContext.SaveChanges();
            return pGroup;
        }

        public ProductGroups UpdateProductGroup(ProductGroups model, int userId)
        {
            var productGroup = _currentDbContext.ProductGroups.FirstOrDefault(u => u.ProductGroupId == model.ProductGroupId);
            if (productGroup != null)
            {
                productGroup.ProductGroup = model.ProductGroup.Trim();
                productGroup.IconPath = !string.IsNullOrEmpty(model.IconPath) ? model.IconPath.Trim() : "";
                productGroup.DateUpdated = DateTime.UtcNow;
                productGroup.UpdatedBy = userId;
                productGroup.DepartmentId = model.DepartmentId;
                _currentDbContext.ProductGroups.Attach(productGroup);
                var entry = _currentDbContext.Entry(productGroup);
                entry.Property(e => e.ProductGroup).IsModified = true;
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
                entry.Property(e => e.DepartmentId).IsModified = true;
                entry.Property(e => e.IconPath).IsModified = true;
                _currentDbContext.SaveChanges();
            }
            return model;
        }

        public void DeleteProductGroup(int productGroupId, int userId)
        {
            ProductGroups productgroups = GetProductGroupById(productGroupId);

            productgroups.IsDeleted = true;
            productgroups.DateUpdated = DateTime.UtcNow;
            productgroups.UpdatedBy = userId;
            _currentDbContext.ProductGroups.Attach(productgroups);

            var entry = _currentDbContext.Entry(productgroups);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
        }

        public ProductCategory CreateProductCategory(ProductCategory model, int userId, int tenantId)
        {
            var pctg = _currentDbContext.ProductCategories.FirstOrDefault(a => a.ProductCategoryName.Equals(model.ProductCategoryName, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted != true);
            if (pctg != null)
                throw new Exception("Product category already exsists");

            var pCategory = new ProductCategory()
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ProductCategoryName = model.ProductCategoryName,
                ProductGroupId = model.ProductGroupId,
                TenantId = tenantId,
                UpdatedBy = userId
            };
            _currentDbContext.ProductCategories.Add(pCategory);
            _currentDbContext.SaveChanges();
            return pCategory;
        }

        public ProductCategory UpdateProductCategory(ProductCategory model, int userId, int tenantId)
        {
            var productCategory = _currentDbContext.ProductCategories.FirstOrDefault(u => u.ProductCategoryId == model.ProductCategoryId);
            if (productCategory != null)
            {
                productCategory.ProductCategoryName = model.ProductCategoryName.Trim();
                productCategory.TenantId = model.TenantId;
                productCategory.SortOrder = model.SortOrder;
                productCategory.DateUpdated = DateTime.UtcNow;
                productCategory.UpdatedBy = userId;
                _currentDbContext.ProductCategories.Attach(productCategory);
                var entry = _currentDbContext.Entry(productCategory);
                entry.Property(e => e.ProductCategoryName).IsModified = true;
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
                _currentDbContext.SaveChanges();
            }
            return model;
        }

        public void DeleteProductCategory(int productCategoryId, int userId)
        {
            ProductCategory productcategory = GetProductCategoryById(productCategoryId);

            productcategory.IsDeleted = true;
            productcategory.DateUpdated = DateTime.UtcNow;
            productcategory.UpdatedBy = userId;
            _currentDbContext.ProductCategories.Attach(productcategory);

            var entry = _currentDbContext.Entry(productcategory);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
        }

        public ProductKitType CreateProductKitType(ProductKitType model, int userId, int tenantId)
        {
            var pctg = _currentDbContext.ProductKitTypes.FirstOrDefault(a => a.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted != true);
            if (pctg != null)
                throw new Exception("Product Kit Type already exists");

            var pKitType = new ProductKitType()
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                Name = model.Name,
                TenentId = tenantId,
                UpdatedBy = userId,
                UseInParentCalculations = model.UseInParentCalculations,
                SortOrder = model.SortOrder,
                IsActive = model.IsActive
            };
            _currentDbContext.ProductKitTypes.Add(pKitType);
            _currentDbContext.SaveChanges();
            return pKitType;
        }

        public ProductKitType UpdateProductKitType(ProductKitType model, int userId, int tenantId)
        {
            var productKitType = _currentDbContext.ProductKitTypes.FirstOrDefault(u => u.Id == model.Id);
            if (productKitType != null)
            {
                productKitType.Name = model.Name.Trim();
                productKitType.TenentId = model.TenentId;
                productKitType.SortOrder = model.SortOrder;
                productKitType.IsActive = model.IsActive;
                productKitType.UseInParentCalculations = model.UseInParentCalculations;
                productKitType.DateUpdated = DateTime.UtcNow;
                productKitType.UpdatedBy = userId;
                _currentDbContext.ProductKitTypes.Attach(productKitType);
                var entry = _currentDbContext.Entry(productKitType);
                entry.Property(e => e.Name).IsModified = true;
                entry.Property(e => e.DateUpdated).IsModified = true;
                entry.Property(e => e.UpdatedBy).IsModified = true;
                _currentDbContext.SaveChanges();
            }
            return model;
        }

        public void DeleteProductKitType(int productKitTypeId, int userId)
        {
            ProductKitType productKitType = GetProductKitTypeById(productKitTypeId);

            productKitType.IsDeleted = true;
            productKitType.DateUpdated = DateTime.UtcNow;
            productKitType.UpdatedBy = userId;
            _currentDbContext.ProductKitTypes.Attach(productKitType);

            var entry = _currentDbContext.Entry(productKitType);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
        }

        public List<WastageReason> GetAllWastageReasons()
        {
            return _currentDbContext.WastageReasons.ToList();
        }

        public PalletType CreatePalletType(PalletType model, int tenantId, int userId)
        {
            var pgrp = _currentDbContext.PalletTypes.FirstOrDefault(a => a.Description.Equals(model.Description, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted != true);
            if (pgrp != null)
                throw new Exception("Pallet type already exsists");

            var palletType = new PalletType()
            {
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                Description = model.Description,
                TenentId = tenantId,
                UpdatedBy = userId
            };
            _currentDbContext.PalletTypes.Add(palletType);
            _currentDbContext.SaveChanges();
            return palletType;
        }
        public PalletType UpdatePalletType(PalletType model, int userId)
        {
            model.Description = model.Description.Trim();
            model.DateUpdated = DateTime.UtcNow;
            model.UpdatedBy = userId;
            _currentDbContext.PalletTypes.Attach(model);
            var entry = _currentDbContext.Entry(model);
            entry.Property(e => e.Description).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
            return model;
        }
        public void DeletePalletType(int palletTypeId, int userId)
        {
            PalletType palletType = GetPalletTypeById(palletTypeId);

            palletType.IsDeleted = true;
            palletType.DateUpdated = DateTime.UtcNow;
            palletType.UpdatedBy = userId;
            _currentDbContext.PalletTypes.Attach(palletType);

            var entry = _currentDbContext.Entry(palletType);
            entry.Property(e => e.IsDeleted).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            _currentDbContext.SaveChanges();
        }
        public IQueryable<MarketVehicle> GetAllTrucks(int TenantId)
        {
            return _currentDbContext.MarketVehicles.Where(u => u.IsDeleted != true && u.TenantId == TenantId);
        }
        public MarketVehicle CreateOrUpdateTruck(MarketVehicle truck, int UserId, int TenantId)
        {
            if (truck.Id <= 0)
            {
                truck.TenantId = TenantId;
                truck.DateCreated = DateTime.UtcNow;
                truck.CreatedBy = UserId;
                _currentDbContext.MarketVehicles.Add(truck);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var trucks = _currentDbContext.MarketVehicles.AsNoTracking().FirstOrDefault(u => u.Id == truck.Id);
                if (truck != null)
                {
                    truck.CreatedBy = trucks.CreatedBy;
                    truck.DateCreated = trucks.DateCreated;
                    truck.TenantId = TenantId;
                    truck.DateUpdated = DateTime.UtcNow;
                    truck.UpdatedBy = truck.UpdatedBy;
                    _currentDbContext.Entry(truck).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }

            }
            return truck;
        }

        public MarketVehicle RemoveTruck(int Id, int UserId)
        {
            var productTags = _currentDbContext.MarketVehicles.FirstOrDefault(u => u.Id == Id);
            if (productTags != null)
            {
                productTags.IsDeleted = true;
                productTags.DateUpdated = DateTime.Now;
                productTags.UpdatedBy = productTags.UpdatedBy;
                _currentDbContext.Entry(productTags).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return productTags;
        }

        //ProductTags
        public IEnumerable<ProductTag> GetAllValidProductTag(int TenantId)
        {
            return _currentDbContext.ProductTags.Where(u => u.IsDeleted != true && u.TenantId == TenantId);
        }
       
        public ProductTag CreateOrUpdateProductTag(ProductTag productTag, int UserId, int TenantId)
        {
            if (productTag.Id <= 0)
            {
                productTag.TenantId = TenantId;
                productTag.DateCreated = DateTime.Now;
                productTag.CreatedBy = UserId;
                _currentDbContext.ProductTags.Add(productTag);
                _currentDbContext.SaveChanges();
            }
            else
            {
                var productTags = _currentDbContext.ProductTags.AsNoTracking().FirstOrDefault(u => u.Id == productTag.Id);
                if (productTags != null)
                {
                    productTag.CreatedBy = productTags.CreatedBy;
                    productTag.DateCreated = productTags.DateCreated;
                    productTag.TenantId = TenantId;
                    productTag.DateUpdated = DateTime.Now;
                    productTag.UpdatedBy = productTags.UpdatedBy;
                    _currentDbContext.Entry(productTag).State = System.Data.Entity.EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }

            }
            return productTag;
        }
        public ProductTag RemoveProductTag(int Id, int UserId)
        {
            var productTags = _currentDbContext.ProductTags.FirstOrDefault(u => u.Id == Id);
            if (productTags != null)
            {
                productTags.IsDeleted = true;
                productTags.DateUpdated = DateTime.Now;
                productTags.UpdatedBy = productTags.UpdatedBy;
                _currentDbContext.Entry(productTags).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return productTags;
        }

        public bool RemoveProductAttriubteValueMap(int id, int userId)
        {
            var productAttributeValues = _currentDbContext.ProductAttributeValues.FirstOrDefault(u => u.AttributeValueId == id);
            if (productAttributeValues != null)
            {
                productAttributeValues.IsDeleted = true;
                productAttributeValues.DateUpdated = DateTime.Now;
                productAttributeValues.UpdatedBy = userId;
                _currentDbContext.Entry(productAttributeValues).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return true;



        }
        public bool RemoveProductAttriubte(int id)
        {
            var productAttributeValues = _currentDbContext.ProductAttributes.FirstOrDefault(u => u.AttributeId == id);
            if (productAttributeValues != null)
            {
                productAttributeValues.IsDeleted = true;
                _currentDbContext.Entry(productAttributeValues).State = System.Data.Entity.EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return true;
        }

        public ProductTag GetProductTagById(int Id)
        {
            return _currentDbContext.ProductTags.FirstOrDefault(u => u.Id == Id);
        }

        public bool CreateOrUpdateKitMap(ProductMasterViewModel productMaster, int ProductId, ProductKitTypeEnum productKitType, int UserId, int TenantId)
        {
            var productKitMaps = _currentDbContext.ProductKitMaps.FirstOrDefault(u => u.ProductId == ProductId && u.KitProductId == productMaster.ProductId && u.ProductKitType == productKitType && u.IsDeleted != true);
            if (productKitMaps == null)
            {
                var newItem = new ProductKitMap()
                {
                    CreatedBy = UserId,
                    DateCreated = DateTime.UtcNow,
                    KitProductId = productMaster.ProductId,
                    ProductId = ProductId,
                    TenantId = TenantId,
                    ProductKitType = productKitType,
                    Quantity = productMaster.Qty ?? 1,
                    ProductKitTypeId = productMaster.Id,
                    IsActive = productMaster.IsActive ?? false

                };
                _currentDbContext.ProductKitMaps.Add(newItem);
            }
            else
            {
                productKitMaps.UpdateUpdatedInfo(UserId);
                productKitMaps.KitProductId = productMaster.ProductId;
                productKitMaps.ProductId = ProductId;
                productKitMaps.TenantId = TenantId;
                productKitMaps.ProductKitType = productKitType;
                productKitMaps.Quantity = productMaster.Qty ?? 1;
                productKitMaps.ProductKitTypeId = productMaster.Id;
                productKitMaps.IsActive = productMaster.IsActive ?? false;
                _currentDbContext.Entry(productKitMaps).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return true;

        }

        private IQueryable<ProductMaster> FilterStockItemsProduct(IQueryable<ProductMaster> productMasters, bool inStock = false, bool outOfStock = false)
        {
            if (inStock && outOfStock)
            {
                return productMasters;
            }
            else if (inStock)
            {
                var productAvailable = productMasters.Where(u =>
                    u.IsStockItem == true &&
                    u.DontMonitorStock == true &&
                    u.IsDeleted != true);
                var stockProduct = productMasters.Where(u => u.InventoryStocks.Sum(c => c.Available) > 0);

                var childProductAvailablity = productMasters.Select(u => u.ProductKitItems.Where(c=>c.IsDeleted != true)).ToList();
            }

            return productMasters;
        }

    }
}