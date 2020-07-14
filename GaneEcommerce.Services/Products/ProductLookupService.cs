using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class ProductLookupService : IProductLookupService
    {
        private readonly IApplicationContext _currentDbContext;
        public ProductLookupService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
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
                locationIds = _currentDbContext.ProductLocationsMap.Where(x => x.IsDeleted != true && x.ProductId == filterForProductId).Select(y => y.LocationId).Distinct().ToList();
            }
            return _currentDbContext.Locations.Where(m => m.TenentId == tenantId && m.WarehouseId == warehouseId && m.IsDeleted != true && (filterForProductId == 0 || locationIds.Contains(m.LocationId)));
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
        public IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValuesByProductId(int productId)
        {
            return _currentDbContext.ProductAttributeValuesMap
                .Where(a => a.ProductId == productId && a.IsDeleted != true).Select(a => a.ProductAttributeValues);
        }

        public IEnumerable<ProductKitType> GetProductKitTypes(List<int?> kitIds)
        {
            return _currentDbContext.ProductKitTypes.Where(u => kitIds.Contains(u.Id)).Distinct().ToList();
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

        public IEnumerable<string> GetAllValidSubCategoriesByDepartmentAndGroup(IQueryable<ProductMaster> productMasters)
        {

            var productcategoriesId = productMasters.Select(u => u.ProductCategoryId).ToList();
            return _currentDbContext.ProductCategories.Where(u => productcategoriesId.Contains(u.ProductCategoryId)).Select(u => u.ProductCategoryName);

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
            return _currentDbContext.ProductLocationsMap.Where(
                a => a.Locations.IsDeleted != true && a.IsDeleted != true && a.ProductId == productId);
        }
        public IQueryable<ProductMaster> FilterProduct(IQueryable<ProductMaster> productMaster, string filterstring)
        {
            Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
            try
            {
                if (!string.IsNullOrEmpty(filterstring))
                {
                    var data = filterstring.Split('/');
                    foreach (var item in data)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var filtersData = item.Split('-');
                            List<string> value = new List<string>();
                            if (filtersData.Length >= 2)
                            {
                                value = filtersData[1].Split(',').Select(i => i.ToLower()).ToList();
                                if (!filters.Keys.Contains(filtersData[0]))
                                {
                                    filters.Add(filtersData[0], value);
                                }
                            }


                        }

                    }
                    foreach (var item in filters)
                    {
                        if (item.Key.Equals("stockS"))
                        {

                            if (item.Value.Count > 1)
                            {
                                productMaster = productMaster.Where(u => ((u.InventoryStocks.Sum(rv => rv.Available)) > 0 ||  (u.InventoryStocks.Sum(rv => rv.Available) <= 0 || u.ProductKitItems.Any(a => a.KitProductMaster.InventoryStocks.Sum(c => c.Available) <= 0))));

                            }

                            else if (item.Value.Contains("in_stock"))
                            {
                                productMaster = productMaster.Where(u => (u.InventoryStocks.Sum(rv => rv.Available)) > 0 || u.ProductKitItems.Any(a=>a.KitProductMaster.InventoryStocks.Sum(c=>c.Available)>0));
                            }
                            else if (item.Value.Contains("out_stock"))
                            {
                                productMaster = productMaster.Where(u => u.InventoryStocks.Sum(rv => rv.Available) <= 0 ||  u.ProductKitItems.Any(a => a.KitProductMaster.InventoryStocks.Sum(c => c.Available) <= 0));
                            }

                        }
                        if (item.Key.Equals("priceS"))
                        {
                            List<decimal> prices = new List<decimal>();
                            if (item.Value.Count > 0)
                            {
                                foreach (var price in item.Value)
                                {
                                    var range = price.Split('_');
                                    if (range.Length >= 2)
                                    {
                                        prices.Add(Convert.ToDecimal(range[0]));
                                        prices.Add(Convert.ToDecimal(range[1]));
                                    }

                                }
                            }
                            if (prices.Count > 0)
                            {
                                decimal? min = prices.Min();
                                decimal? max = prices.Max();
                                productMaster = productMaster.Where(u => u.SellPrice >= min && u.SellPrice <= max);
                            }
                        }
                        if (item.Key.Contains("BrandS"))
                        {
                            var result = item.Value.Select(s => s.Replace("_", " ").Replace("^", ",")).ToList();
                            productMaster = productMaster.Where(u => result.Contains(u.ProductManufacturer.Name));

                        }
                        if (item.Key.Contains("TypeS"))
                        {
                            var result = item.Value.Select(s => s.Replace("_", " ").Replace("^", ",")).ToList();
                            productMaster = productMaster.Where(u => result.Contains(u.ProductCategory.ProductCategoryName));

                        }
                        if (item.Key.Length > 0 && (!item.Key.Equals("BrandS") && !item.Key.Equals("priceS") && !item.Key.Equals("stockS") && !item.Key.Equals("TypeS")))
                        {
                            var result = item.Value.Select(s => s.Replace("_", " ").Replace("??", ",")).ToList();
                            var attributeValueId = _currentDbContext.ProductAttributeValues.Where(u => result.Contains(u.Value)).Select(u => u.AttributeValueId).ToList();
                            var productids = _currentDbContext.ProductAttributeValuesMap.Where(u => attributeValueId.Contains(u.AttributeValueId)).Select(u => u.ProductId).ToList();
                            productMaster = productMaster.Where(u => productids.Contains(u.ProductId));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return productMaster;

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

        public ProductAttributeValuesMap GetProductAttributeValueMap(int productId, int attributeValueId)
        {
            return _currentDbContext.ProductAttributeValuesMap.FirstOrDefault(
                a => a.ProductId == productId && a.AttributeValueId == attributeValueId);
        }




        public ProductAttributes SaveProductAttribute(string attributeName, int sortOrder, bool isColorTyped, int? attributeId = null)
        {
            var att = _currentDbContext.ProductAttributes.FirstOrDefault(a => a.AttributeName.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase) && (!attributeId.HasValue || a.AttributeId == attributeId));
            if (att != null)
            {

                att.AttributeName = attributeName;
                att.SortOrder = sortOrder;
                att.IsColorTyped = isColorTyped;
                _currentDbContext.Entry(att).State = EntityState.Modified;


            }
            else
            {
                ;

                att = new ProductAttributes()
                {
                    AttributeName = attributeName,
                    SortOrder = sortOrder,
                    IsColorTyped = isColorTyped
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






        public ProductAttributeValues SaveProductAttributeValueMap(ProductAttributeValues model, int userId, int tenantId, int productId)
        {
            if (model.AttributeValueId == 0)
            {
                var valuetoAdd = SaveProductAttributeValue(model.AttributeId, model.Value, userId, model.Color);
                var valueMap = new ProductAttributeValuesMap
                {
                    AttributeValueId = valuetoAdd.AttributeValueId,
                    CreatedBy = userId,
                    TenantId = tenantId,
                    DateCreated = DateTime.UtcNow,
                    ProductId = productId,
                };
                _currentDbContext.ProductAttributeValuesMap.Add(valueMap);
            }
            else
            {
                model.UpdateUpdatedInfo(userId);
                _currentDbContext.ProductAttributeValues.Attach(model);
                var entry = _currentDbContext.Entry(model);
                entry.Property(e => e.Value).IsModified = true;
                entry.Property(e => e.AttributeId).IsModified = true;
            }
            _currentDbContext.SaveChanges();
            return model;
        }

        public void DeleteProductAttributeValue(int productId, int attributeValueId, int userId, int tenantId)
        {
            var currentm = GetProductAttributeValueMap(productId, attributeValueId);

            currentm.IsDeleted = true;
            currentm.UpdatedBy = userId;
            currentm.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(currentm).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            var currentV = _currentDbContext.ProductAttributeValues.Find(attributeValueId);
            currentV.IsDeleted = true;
            currentV.UpdateUpdatedInfo(userId);
            _currentDbContext.Entry(currentV).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

        }




        public Locations SaveProductLocation(Locations model, int warehouseId, int tenantId, int userId, int productId = 0)
        {
            if (model.LocationId < 1)
            {

                var cLocation =
                    _currentDbContext.Locations.FirstOrDefault(a => a.WarehouseId == warehouseId &&
                                                                    a.TenentId == tenantId && a.LocationCode ==
                                                                    model.LocationCode);
                if (cLocation != null)
                    throw new Exception("Location Code already exists!");
                model.IsActive = true;
                model.IsDeleted = false;
                model.TenentId = tenantId;

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
                    _currentDbContext.ProductLocationsMap.Add(map);
                }
            }
            else
            {
                var cLocation =
                    _currentDbContext.Locations.AsNoTracking().FirstOrDefault(a => a.WarehouseId == warehouseId &&
                                                                    a.TenentId == tenantId && a.LocationCode ==
                                                                    model.LocationCode);

                model.UpdatedBy = userId;
                model.DateUpdated = DateTime.UtcNow;
                model.DateCreated = cLocation.DateCreated;
                model.WarehouseId = cLocation.WarehouseId;
                model.TenentId = cLocation.TenentId;
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

            var maps = _currentDbContext.ProductLocationsMap.Where(a => a.LocationId == locationId && a.ProductId == productId);
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
                UpdatedBy = userId
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
    }
}