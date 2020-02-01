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
        public IQueryable<ProductMaster> GetAllValidProductGroupById(int? productGroupId)
        {
            return _currentDbContext.ProductMaster
                .Where(a => (!productGroupId.HasValue || a.ProductGroupId == productGroupId) && a.IsDeleted != true);
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




        public Locations GetLocationById(int locationId)
        {
            return _currentDbContext.Locations.Find(locationId);
        }

        public ProductGroups GetProductGroupById(int productGroupId)
        {
            return _currentDbContext.ProductGroups.Find(productGroupId);
        }
        public PalletType GetPalletTypeById(int palletTypeId)
        {
            return _currentDbContext.PalletTypes.Find(palletTypeId);
        }

        public ProductGroups GetProductGroupByName(string groupName)
        {
            return _currentDbContext.ProductGroups.FirstOrDefault(p => p.ProductGroup.Equals(groupName) && p.IsDeleted != true);
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
            model.ProductGroup = model.ProductGroup.Trim();
            model.IconPath = !string.IsNullOrEmpty(model.IconPath) ? model.IconPath.Trim() : "";
            model.DateUpdated = DateTime.UtcNow;
            model.UpdatedBy = userId;
            model.DepartmentId = model.DepartmentId;
            _currentDbContext.ProductGroups.Attach(model);
            var entry = _currentDbContext.Entry(model);
            entry.Property(e => e.ProductGroup).IsModified = true;
            entry.Property(e => e.DateUpdated).IsModified = true;
            entry.Property(e => e.UpdatedBy).IsModified = true;
            entry.Property(e => e.DepartmentId).IsModified = true;
            _currentDbContext.SaveChanges();
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