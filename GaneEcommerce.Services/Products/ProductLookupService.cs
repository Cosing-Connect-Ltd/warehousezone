using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
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
        public IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValues()
        {
            return _currentDbContext.ProductAttributeValues.Where(m => m.IsDeleted != true);
        }
        public IEnumerable<ProductAttributeValues> GetAllValidProductAttributeValuesByProductId(int productId)
        {
            return _currentDbContext.ProductAttributeValuesMap
                .Where(a => a.ProductId == productId && a.IsDeleted != true).Select(a => a.ProductAttributeValues);
        }
        public IQueryable<ProductMaster> GetAllValidProductGroupById(int? productGroupId, int? departmentId = null)
        {
            return _currentDbContext.ProductMaster
                .Where(a => ((!productGroupId.HasValue || a.ProductGroupId == productGroupId) && (!departmentId.HasValue || a.DepartmentId == departmentId)) && a.IsDeleted != true).Include(u => u.ProductManufacturer);
        }
        public IQueryable<ProductMaster> GetAllValidProductGroupAndDeptByName(string productGroup, string department = "",string SubCategory="")
        {
            int? productGroupId = _currentDbContext.ProductGroups.FirstOrDefault(u => u.ProductGroup == productGroup && u.IsDeleted != true)?.ProductGroupId;
            int? departmentId = _currentDbContext.TenantDepartments.FirstOrDefault(u => u.DepartmentName == department && u.IsDeleted != true)?.DepartmentId;
            int? subCategoryId = _currentDbContext.ProductCategories.FirstOrDefault(u => u.ProductCategoryName == SubCategory && u.IsDeleted != true)?.ProductCategoryId;
            return _currentDbContext.ProductMaster
                .Where(a => ((!productGroupId.HasValue || a.ProductGroupId == productGroupId) && (!departmentId.HasValue || a.DepartmentId == departmentId) && (!subCategoryId.HasValue || a.ProductCategoryId == subCategoryId)) && a.IsDeleted != true).Include(u => u.ProductManufacturer);
        }

        public IEnumerable<ProductManufacturer> GetAllValidProductManufacturerGroupAndDeptByName(string productGroup, string department = "", string SubCategory = "")
        {
            int? productGroupId = _currentDbContext.ProductGroups.FirstOrDefault(u => u.ProductGroup == productGroup && u.IsDeleted != true)?.ProductGroupId;
            int? departmentId = _currentDbContext.TenantDepartments.FirstOrDefault(u => u.DepartmentName == department && u.IsDeleted != true)?.DepartmentId;
            int? subCategoryId = _currentDbContext.ProductCategories.FirstOrDefault(u => u.ProductCategoryName == SubCategory && u.IsDeleted != true)?.ProductCategoryId;
            var productmanufacturerId=  _currentDbContext.ProductMaster.Where(a => ((!productGroupId.HasValue || a.ProductGroupId == productGroupId) && (!departmentId.HasValue || a.DepartmentId == departmentId) && (!subCategoryId.HasValue || a.ProductCategoryId == subCategoryId)) && a.IsDeleted != true).Select(u=>u.ManufacturerId).ToList();
            return _currentDbContext.ProductManufacturers.Where(u => productmanufacturerId.Contains(u.Id));

        }
        public Tuple<string, string> AllPriceListAgainstGroupAndDept(string productGroup, string department = "")
        {
            int? productGroupId = _currentDbContext.ProductGroups.FirstOrDefault(u => u.ProductGroup == productGroup && u.IsDeleted != true)?.ProductGroupId;
            int? departmentId = _currentDbContext.TenantDepartments.FirstOrDefault(u => u.DepartmentName == department && u.IsDeleted != true)?.DepartmentId;
            var avarageValue= _currentDbContext.ProductMaster.Where(a => ((!productGroupId.HasValue || a.ProductGroupId == productGroupId) && (!departmentId.HasValue || a.DepartmentId == departmentId)) && a.IsDeleted != true).Select(u => u.SellPrice).ToList();
            Tuple<string,string> Prices = null;
            var avgValue = avarageValue.Average();
            var minValue = avarageValue.Min();
            var maxValue = avarageValue.Max();
            var centervalue = avarageValue.Find(u => u.Value > avgValue && u.Value < maxValue);
            var firstValue = new Tuple<string, string>(minValue.ToString(), avgValue.ToString());
            var secondValue = new Tuple<string, string>(avgValue.ToString(), centervalue.ToString());
            Prices= new Tuple<string, string>(centervalue.ToString(), maxValue.ToString());
            return Prices;
        }
        private List<List<decimal[]>> SplitList(List<decimal[]> locations, int nSize = 20)
        {
            var list = new List<List<decimal[]>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
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
                            productMaster = productMaster.Where(u => ((u.InventoryStocks.Sum(rv => rv.Available)) > 0) || (u.InventoryStocks.Any(rv => rv.InStock <= 0)));
                        }
                        else if (item.Value.Contains("in_stock"))
                        {
                            productMaster = productMaster.Where(u => (u.InventoryStocks.Sum(rv => rv.Available)) > 0);
                        }
                        else if (item.Value.Contains("out_stock"))
                        {
                            productMaster = productMaster.Where(u => u.InventoryStocks.Any(rv => rv.InStock <= 0));
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
                        item.Value.ForEach(u => u.Replace("_", " "));
                        productMaster = productMaster.Where(u => item.Value.Contains(u.ProductManufacturer.Name));

                    }



                }




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




        public ProductAttributes SaveProductAttribute(string attributeName)
        {
            var chkAttribute = _currentDbContext.ProductAttributes.FirstOrDefault(a => a.AttributeName.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase));
            if (chkAttribute != null) return null;

            var att = new ProductAttributes()
            {
                AttributeName = attributeName
            };
            _currentDbContext.Entry(att).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return att;
        }

        public ProductAttributeValues SaveProductAttributeValue(int attributeId, string attributeValue, int userId = 0)
        {
            var value = _currentDbContext.ProductAttributeValues.FirstOrDefault(m => m.AttributeId == attributeId && m.Value == attributeValue);
            if (value == null && userId > 0)
            {
                value = new ProductAttributeValues()
                {
                    AttributeId = attributeId,
                    Value = attributeValue
                };

                value.UpdateCreatedInfo(userId);
                _currentDbContext.Entry(value).State = EntityState.Added;
                _currentDbContext.SaveChanges();
                return value;
            }

            else if (userId == 0)
            {
                throw new Exception("Unable to create attribute : User information not available");
            }


            return value;


        }

        public ProductAttributeValues SaveProductAttributeValueMap(ProductAttributeValues model, int userId, int tenantId, int productId)
        {
            if (model.AttributeValueId == 0)
            {
                var valuetoAdd = SaveProductAttributeValue(model.AttributeId, model.Value, userId);
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
    }
}