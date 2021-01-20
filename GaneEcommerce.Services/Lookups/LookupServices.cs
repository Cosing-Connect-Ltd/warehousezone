using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class LookupServices : ILookupServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly ICommonDbServices _commonDbServices;

        public LookupServices(IApplicationContext currentDbContext, ICommonDbServices commonDbServices)
        {
            _currentDbContext = currentDbContext;
            _commonDbServices = commonDbServices;
        }

        public IEnumerable<GlobalUOM> GetAllValidGlobalUoms(EnumUomType uomType = EnumUomType.All)
        {
            return _currentDbContext.GlobalUOM.Where(m => uomType == EnumUomType.All || m.UOMTypeId == (int)(uomType));
        }

        public IEnumerable<GlobalTax> GetAllValidGlobalTaxes(int? countryId = null, TaxTypeEnum? taxType = null)
        {
            return _currentDbContext.GlobalTax.Where(m => (!countryId.HasValue || m.CountryID == countryId) && (!taxType.HasValue || m.TaxType == TaxTypeEnum.All || m.TaxType == taxType));
        }

        public IEnumerable<TenantLoanTypes> GetAllValidTenantLoanTypes(int tenantId)
        {
            return _currentDbContext.TenantLoanTypes.Where(e => e.TenantId == tenantId);
        }

        public IEnumerable<TenantDepartments> GetAllValidTenantDepartments(int tenantId)
        {
            return _currentDbContext.TenantDepartments.Where(m => m.IsDeleted != true && m.TenantId == tenantId);
        }

        public IEnumerable<GlobalWeightGroups> GetAllValidGlobalWeightGroups()
        {
            return _currentDbContext.GlobalWeightGroups;
        }

        public IEnumerable<SLAPriorit> GetAllValidSlaPriorities(int tenantId)
        {
            return _currentDbContext.SLAPriorities.Where(a => a.TenantId == tenantId && a.IsDeleted != true);
        }

        public IEnumerable<LocationGroup> GetAllValidLocationGroups(int tenantId)
        {
            return _currentDbContext.LocationGroups.Where(m => m.TenentId == tenantId && m.IsDeleted != true);
        }

        public IEnumerable<LocationTypes> GetAllValidLocationTypes(int tenantId)
        {
            return _currentDbContext.LocationTypes.Where(m => m.IsDeleted != true && m.TenentId == tenantId);
        }

        public IEnumerable<ProductGroups> GetAllValidProductGroups(int tenantId, int numberofproduct = 0)
        {
            if (numberofproduct > 0)
            {
                return _currentDbContext.ProductGroups.Where(m => m.IsDeleted != true && m.TenentId == tenantId).Take(numberofproduct);
            }
            return _currentDbContext.ProductGroups.Where(m => m.IsDeleted != true && m.TenentId == tenantId);
        }

        public IEnumerable<ProductCategory> GetAllValidProductCategories(int tenantId, int numberofproduct = 0, int? ProductGroupId = null)
        {
            if (numberofproduct > 0)
            {
                return _currentDbContext.ProductCategories.Where(m => m.IsDeleted != true && m.TenantId == tenantId && (!ProductGroupId.HasValue || m.ProductGroupId == ProductGroupId)).Take(numberofproduct);
            }
            return _currentDbContext.ProductCategories.Where(m => m.IsDeleted != true && m.TenantId == tenantId && (!ProductGroupId.HasValue || m.ProductGroupId == ProductGroupId));
        }

        public IEnumerable<ProductKitType> GetAllValidProductKitType(int tenantId)
        {
            return _currentDbContext.ProductKitTypes.Where(m => m.IsDeleted != true && m.TenentId == tenantId);
        }

        public IEnumerable<PalletType> GetAllValidPalletTypes(int tenantId)
        {
            return _currentDbContext.PalletTypes.Where(m => m.IsDeleted != true && m.TenentId == tenantId);
        }
        public IEnumerable<ProductManufacturer> GetAllValidProductManufacturer(int tenantId, int? Id = null)
        {
            return _currentDbContext.ProductManufacturers.Where(m => m.IsDeleted != true && m.TenantId == tenantId && (!Id.HasValue || m.Id == Id));
        }
        public bool RemoveProductManufacturer(int Id)
        {
            var manufacturer = _currentDbContext.ProductManufacturers.FirstOrDefault(m => m.Id == Id);
            if (manufacturer != null)
            {
                manufacturer.IsDeleted = true;
                _currentDbContext.Entry(manufacturer).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public IEnumerable<ReportType> GetAllReportTypes(int tenantId)
        {
            var reportTypes = _currentDbContext.ReportTypes.Where(a => a.TenantId == tenantId && a.IsDeleted != true);
            return reportTypes;
        }

        public TenantDepartments GetTenantDepartmentById(int departmentId)
        {
            return _currentDbContext.TenantDepartments.Find(departmentId);
        }

        public TenantLocations GetWarehouseById(int tenantLocationId)
        {
            return _currentDbContext.TenantWarehouses.Find(tenantLocationId);
        }

        public List<TenantLocations> GetAllWarehousesForTenant(int tenantId, int? excludeWarehouseId = null, bool? excludeMobileLocations = false)
        {
            var warehouses = _currentDbContext.TenantWarehouses
                .Where(m => m.TenantId == tenantId && m.IsDeleted != true && (excludeMobileLocations == false || m.IsMobile == true)).ToList();
            if (excludeWarehouseId != null)
            {
                warehouses.RemoveAll(m => m.WarehouseId == excludeWarehouseId);
            }
            return warehouses;
        }

        public IEnumerable<GlobalCountry> GetAllGlobalCountries()
        {
            return _currentDbContext.GlobalCountries;
        }

        public IEnumerable<GlobalCurrency> GetAllGlobalCurrencies()
        {
            return _currentDbContext.GlobalCurrencies;
        }

        public IEnumerable<TenantWarranty> GetAllTenantWarrenties(int tenantId)
        {
            return _currentDbContext.TenantWarranty.Where(e => e.TenantId == tenantId);
        }

        public TenantDepartments SaveTenantDepartment(string departmentName, int? accountId, int userId, int tenantId)
        {
            var dept = _currentDbContext.TenantDepartments.FirstOrDefault(a => a.DepartmentName.Equals(departmentName));
            if (dept != null) return null;
            dept = new TenantDepartments() { TenantId = tenantId, DepartmentName = departmentName, AccountID = accountId };
            dept.UpdateCreatedInfo(userId);
            dept.UpdateUpdatedInfo(userId);
            _currentDbContext.Entry(dept).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return dept;
        }
        public TenantDepartments UpdateTenantDepartment(TenantDepartments tenantDepartments)
        {
            var dept = _currentDbContext.TenantDepartments.FirstOrDefault(a => a.DepartmentId == tenantDepartments.DepartmentId);
            if (dept == null) return null;
            dept.TenantId = tenantDepartments.TenantId;
            dept.DepartmentName = tenantDepartments.DepartmentName;
            dept.AccountID = tenantDepartments.AccountID;
            dept.ImagePath = tenantDepartments.ImagePath;
            dept.UpdateUpdatedInfo(tenantDepartments.CreatedBy ?? 1);
            _currentDbContext.Entry(dept).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return dept;
        }
        public TenantDepartments RemoveTenantDepartment(int departmentId, int currentUserId)
        {
            var dept = _currentDbContext.TenantDepartments.FirstOrDefault(a => a.DepartmentId == departmentId);
            if (dept == null) return null;
            dept.IsDeleted = true;
            dept.UpdateUpdatedInfo(currentUserId);
            _currentDbContext.Entry(dept).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return dept;

        }

        public IEnumerable<TenantPriceGroups> GetAllPriceGroups(int tenantId, int filterById = 0)
        {
            return _currentDbContext.TenantPriceGroups.Where(e => e.TenantId == tenantId && e.IsDeleted != true && (filterById == 0 || e.PriceGroupID == filterById));
        }

        public IEnumerable<JobType> GetAllJobTypes(int tenantId)
        {
            return _currentDbContext.JobTypes.AsNoTracking().Where(m => m.TenantId == tenantId && m.IsDeleted != true);
        }

        public IEnumerable<SlaWorksOrderListViewModel> GetAllJobTypesIncludingNavProperties(int tenantId)
        {
            return _currentDbContext.JobTypes.AsNoTracking().Where(x => x.Orders.Count > 0 && x.Orders.Any(y => y.OrderStatusID == OrderStatusEnum.NotScheduled
            || y.OrderStatusID == OrderStatusEnum.ReAllocationRequired) && x.IsDeleted != true)
               .Select(p => new SlaWorksOrderListViewModel
               {
                   JobTypeId = p.JobTypeId,
                   Name = p.Name,
                   Orders = p.Orders.Where(c => c.OrderStatusID == OrderStatusEnum.NotScheduled || c.OrderStatusID == OrderStatusEnum.ReAllocationRequired)
                   .Select(m => new SlaWorksOrderViewModel
                   {
                       OrderID = m.OrderID,
                       OrderNumber = m.OrderNumber,
                       OrderNotes = m.OrderNotes,
                       SLAPriorityId = m.SLAPriority.SLAPriorityId,
                       SlaPriorityName = m.SLAPriority.Priority,
                       JobTypeId = m.JobType.JobTypeId,
                       OrderStatusID = m.OrderStatusID,
                       PPropertyId = m.PProperties.PPropertyId,
                       AddressLine1 = m.PProperties.AddressLine1,
                       JobSubTypeName = m.JobSubType.Name,
                       ExpectedHours = m.ExpectedHours,
                       JobTypeName = m.JobType.Name,
                       Colour = m.SLAPriority.Colour,
                       TenentId = m.TenentId
                   })
                   .OrderByDescending(x => x.SLAPriorityId).ThenByDescending(x => x.PPropertyId).ToList()

               });
        }

        public IEnumerable<Locations> GetAllLocations(int tenantId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            return _currentDbContext.Locations.Where(a => (includeIsDeleted || a.IsDeleted != true) && a.TenantId == tenantId && (!reqDate.HasValue || (a.DateUpdated ?? a.DateCreated) >= reqDate));
        }

        public IEnumerable<ProductLocationStocks> GetAllLocationStocks(int tenantId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            return _currentDbContext.ProductLocationStocks.Where(a => (includeIsDeleted || a.IsDeleted != true) && a.TenantId == tenantId && (!reqDate.HasValue || (a.DateUpdated ?? a.DateCreated) >= reqDate));
        }

        public Locations GetLocationById(int locationId, int tenantId)
        {
            return _currentDbContext.Locations.FirstOrDefault(
                m => m.TenantId == tenantId && m.LocationId == locationId);
        }

        public LocationTypes GetLocationTypeById(int locationTypeId)
        {
            return _currentDbContext.LocationTypes.Find(locationTypeId);
        }

        public void DeleteLocationById(int locationId, int tenantId, int userId)
        {
            var locations = GetLocationById(locationId, tenantId);
            locations.IsDeleted = true;
            locations.UpdatedBy = userId;
            locations.DateUpdated = DateTime.UtcNow;
            _currentDbContext.SaveChanges();
        }

        public Locations GetLocationByCode(string locationCode, int tenantId)
        {
            return _currentDbContext.Locations.FirstOrDefault(
                m => m.TenantId == tenantId && m.LocationCode == locationCode);
        }

        public Locations GetLocationByName(string locationName, int tenantId)
        {
            return _currentDbContext.Locations.FirstOrDefault(
                m => m.TenantId == tenantId && m.LocationName == locationName);
        }

        public Locations CreateLocation(Locations location, List<int> ProductKitIds, int userId, int tenantId, int warehouseId)
        {
            location.CreatedBy = userId;
            location.DateCreated = DateTime.UtcNow;
            location.TenantId = tenantId;
            location.WarehouseId = warehouseId;
            _currentDbContext.Locations.Add(location);
            _currentDbContext.SaveChanges();
            if (ProductKitIds != null)
            {
                foreach (var item in ProductKitIds)
                {
                    ProductLocations locMap = new ProductLocations
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        LocationId = location.LocationId,
                        ProductId = item,
                        TenantId = tenantId,
                    };
                    _currentDbContext.ProductLocations.Add(locMap);
                }
                _currentDbContext.SaveChanges();
            }
            return location;
        }

        public Locations BulkCreateProductsLocation(Locations location, List<int> productIds, int? startValue,
            int? endValue, int tenantId,
            int warehouseId, int userId)
        {
            var code = location.LocationCode;
            var name = location.LocationName;
            for (var ctr = startValue; ctr <= endValue; ctr++)
            {
                var obj = location;
                obj.CreatedBy = userId;
                obj.TenantId = tenantId;
                obj.WarehouseId = warehouseId;
                obj.DateCreated = DateTime.UtcNow;
                obj.LocationCode = code + ctr;
                obj.LocationName = name + ctr;
                obj.PickSeq = location.PickSeq != null ? ctr : null;
                obj.PutAwaySeq = location.PutAwaySeq != null ? ctr : null;
                obj.ReplenishSeq = location.ReplenishSeq != null ? ctr : null;
                _currentDbContext.Locations.Add(obj);
                _currentDbContext.SaveChanges();
                if (productIds != null)
                {
                    foreach (var item in productIds)
                    {
                        var locMap = new ProductLocations
                        {
                            LocationId = obj.LocationId,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            ProductId = item,
                            TenantId = tenantId
                        };
                        _currentDbContext.ProductLocations.Add(locMap);
                        _currentDbContext.SaveChanges();
                    }

                }
            }
            return location;
        }

        public IEnumerable<ProductMaster> GetProductsByLocationId(int locationId, int tenantId)
        {
            return _currentDbContext.ProductLocations
                .Where(a => a.LocationId == locationId && a.TenantId == tenantId && a.IsDeleted != true)
                .Select(a => a.ProductMaster);
        }

        public LocationTypes GetLocationTypeByName(string locationTypeName, int tenantId,
            int? excludeLocationTypeId = null)
        {
            return _currentDbContext.LocationTypes.FirstOrDefault(a => a.LocTypeName == locationTypeName &&
                                                                       a.IsDeleted != true && a.TenentId == tenantId &&
                                                                       (!excludeLocationTypeId.HasValue ||
                                                                        a.LocationTypeId != excludeLocationTypeId));
        }

        public Locations BulkEditProductsLocation(Locations location, List<int> ProductKitIds, int tenantId,
            int warehouseId, int userId)
        {
            _currentDbContext.Locations.Attach(location);
            var entry = _currentDbContext.Entry(location);
            entry.Property(e => e.AllowPick).IsModified = true;
            entry.Property(e => e.AllowPutAway).IsModified = true;
            entry.Property(e => e.AllowReplenish).IsModified = true;
            entry.Property(e => e.Description).IsModified = true;
            entry.Property(e => e.DimensionUOMId).IsModified = true;

            entry.Property(e => e.LocationCode).IsModified = true;
            entry.Property(e => e.LocationName).IsModified = true;
            entry.Property(e => e.LocationGroupId).IsModified = true;
            entry.Property(e => e.LocationTypeId).IsModified = true;
            entry.Property(e => e.LocationWeight).IsModified = true;
            entry.Property(e => e.LocationWidth).IsModified = true;
            entry.Property(e => e.MixContainer).IsModified = true;
            entry.Property(e => e.PickSeq).IsModified = true;
            entry.Property(e => e.PutAwaySeq).IsModified = true;
            entry.Property(e => e.ReplenishSeq).IsModified = true;
            entry.Property(e => e.UOMId).IsModified = true;
            entry.Property(e => e.StagingLocation).IsModified = true;


            location.DateUpdated = DateTime.UtcNow;
            location.UpdatedBy = userId;
            location.WarehouseId = warehouseId;
            if (ProductKitIds != null)
            {
                var toAdd = ProductKitIds.Except(_currentDbContext.ProductLocations
                    .Where(a => a.IsDeleted != true && a.LocationId == location.LocationId).Select(a => a.ProductId)
                    .ToList());
                var toDelete = _currentDbContext.ProductLocations
                    .Where(a => a.LocationId == location.LocationId && a.IsDeleted != true).Select(a => a.ProductId)
                    .ToList()
                    .Except(ProductKitIds);
                foreach (var item in toAdd)
                {
                    var lmap = new ProductLocations
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.UtcNow,
                        ProductId = item,
                        TenantId = tenantId,
                        LocationId = location.LocationId,
                    };
                    _currentDbContext.ProductLocations.Add(lmap);
                }
                foreach (var item in toDelete)
                {
                    var cItem = _currentDbContext.ProductLocations.First(
                        a => a.ProductId == item && a.LocationId == location.LocationId && a.IsDeleted != true);
                    cItem.IsDeleted = true;
                    cItem.DateUpdated = DateTime.UtcNow;
                    cItem.UpdatedBy = userId;

                }
            }
            else
            {
                var Plist = _currentDbContext.ProductLocations.Where(a => a.LocationId == location.LocationId)
                    .ToList();
                foreach (var item in Plist)
                {

                    item.IsDeleted = true;
                    item.UpdatedBy = userId;
                    item.DateUpdated = DateTime.UtcNow;
                }

            }
            _currentDbContext.SaveChanges();

            return location;
        }


        public JobType CreateJobType(JobType jobtype, int tenantId, int userId, List<int> resourceIds)
        {
            jobtype.TenantId = tenantId;
            jobtype.CreatedBy = userId;
            jobtype.DateCreated = DateTime.UtcNow;
            if (resourceIds != null)
            {
                foreach (var item in resourceIds)
                {
                    jobtype.AppointmentResources.Add(_currentDbContext.Resources.Find(item));
                }
            }
            _currentDbContext.JobTypes.Add(jobtype);
            _currentDbContext.SaveChanges();
            return jobtype;
        }

        public JobType SaveJobType(JobType jobtype, int tenantId, int userId, List<int> resourceIds)
        {
            var newJobType = _currentDbContext.JobTypes.Find(jobtype.JobTypeId);

            if (resourceIds == null)
            {
                var toDelete = newJobType.AppointmentResources.ToList();
                foreach (var item in toDelete)
                {
                    newJobType.AppointmentResources.Remove(item);
                }

                _currentDbContext.SaveChanges();
            }
            else
            {
                var resources = newJobType.AppointmentResources.ToList();
                var toDelete = resources.Select(a => a.ResourceId).Except(resourceIds);
                var toAdd = resourceIds.Except(resources.Select(a => a.ResourceId));
                foreach (var item in toAdd)
                {
                    newJobType.AppointmentResources.Add(_currentDbContext.Resources.Find(item));
                }

                foreach (var item in toDelete)
                {
                    newJobType.AppointmentResources.Remove(_currentDbContext.Resources.Find(item));
                }
            }

            newJobType.UpdatedBy = userId;
            newJobType.Name = jobtype.Name;
            newJobType.Description = jobtype.Description;
            newJobType.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(newJobType).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
            return newJobType;
        }

        public void DeleteJobType(int jobtypeId, int tenantId)
        {
            var model = GetJobTypeById(jobtypeId, tenantId);

            model.IsDeleted = true;
            _currentDbContext.Entry(model).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public JobType GetJobTypeById(int jobTypeId, int tenantId)
        {
            return _currentDbContext.JobTypes.Include("AppointmentResources")
                .FirstOrDefault(a => a.JobTypeId == jobTypeId);
        }

        public IEnumerable<AuthActivity> GetAllActiveActivities()
        {
            return _currentDbContext.AuthActivities.Where(e => e.IsDeleted != true && e.IsActive == true);
        }

        public IEnumerable<AuthActivityGroup> GetAllActiveActivityGroups()
        {
            return _currentDbContext.AuthActivityGroups.Where(e => e.IsDeleted != true && e.IsActive == true);
        }

        public bool IsLocationCodeAvailable(string locationcode, int locationId, int tenantId)
        {
            var result = (from p in _currentDbContext.Locations
                          where (p.LocationCode == locationcode && p.LocationId != locationId && p.TenantId == tenantId &&
                                 p.IsDeleted != true)
                          select p).Count();

            return (result < 1);
        }

        public LocationTypes CreateLocationType(string locationTypeName, string locationDescription, int tenantId,
            int userId)
        {
            var locationType = new LocationTypes
            {
                LocTypeName = locationTypeName,
                LocTypeDescription = locationDescription,
                CreatedBy = userId,
                TenentId = tenantId,
                DateCreated = DateTime.UtcNow
            };
            _currentDbContext.LocationTypes.Add(locationType);
            _currentDbContext.SaveChanges();
            return locationType;
        }

        public LocationGroup CreateLocationGroup(string locationGroupName, int tenantId, int userId)
        {
            var model = new LocationGroup() { Locdescription = locationGroupName, CreatedBy = userId, DateCreated = DateTime.UtcNow, TenentId = tenantId };
            _currentDbContext.LocationGroups.Add(model);
            _currentDbContext.SaveChanges();
            return model;
        }

        public LocationGroup GetLocationGroupById(int locationGroupId)
        {
            return _currentDbContext.LocationGroups.Find(locationGroupId);
        }

        public LocationGroup GetLocationGroupByName(string locationGroupName, int tenantId, int? excludeLocGroupTypeId = null)
        {
            return _currentDbContext.LocationGroups.FirstOrDefault(a => a.Locdescription == locationGroupName && a.IsDeleted != true && a.TenentId == tenantId && (!excludeLocGroupTypeId.HasValue || excludeLocGroupTypeId != a.LocationGroupId));
        }

        public IEnumerable<object> GetProductDepartments(int accountId)
        {
            var obj = _currentDbContext.TenantDepartments.Where(u => (u.AccountID == accountId || u.AccountID == null) && u.IsDeleted != true).Select(
                u => new
                {
                    u.DepartmentId,
                    u.DepartmentName
                });
            return obj.ToList();
        }
        public IEnumerable<object> GetProductGroups(int departmentId)
        {
            var obj = _currentDbContext.ProductGroups.Where(u => (u.DepartmentId == departmentId || u.DepartmentId == null) && u.IsDeleted != true).Select(
                u => new
                {
                    u.ProductGroupId,
                    u.ProductGroup
                });
            return obj.ToList();
        }

        public IEnumerable<object> GetProductCategory(int groupId)
        {
            var obj = _currentDbContext.ProductCategories.Where(u => (u.ProductGroupId == groupId) && u.IsDeleted != true).Select(
                u => new
                {
                    u.ProductCategoryId,
                    u.ProductCategoryName
                });
            return obj.ToList();
        }

        public IEnumerable<object> GetAllValidWebsites(int currentTenantId)
        {
            var obj = _currentDbContext.TenantWebsites.Where(u => u.TenantId == currentTenantId && u.IsDeleted != true).Select(
              u => new
              {
                  u.SiteName,
                  u.SiteID
              });
            return obj.ToList();

        }

        public IQueryable<TenantEmailNotificationQueue> GetEmailNotifcationQueue(int tenantId)
        {
            return _currentDbContext.TenantEmailNotificationQueues.Where(u => u.IsNotificationCancelled != true).OrderByDescending(u => u.TenantEmailNotificationQueueId);
        }

        public bool CheckStockIssue(int ProductId, decimal InStock, bool serial, bool productPallet)
        {
            decimal palletstock = 0;
            if (productPallet)
            {
                palletstock = _currentDbContext.PalletTracking.Where(u => u.ProductId == ProductId && (u.RemainingCases > 0) && u.Status != PalletTrackingStatusEnum.Created).Select(u => u.RemainingCases).DefaultIfEmpty(0).Sum();
            }

            var product = _currentDbContext.ProductMaster.Find(ProductId);

            palletstock = palletstock * product.ProductsPerCase ?? 1;

            if (InStock == palletstock)
            {
                return false;
            }

            return true;
        }

        public ProductManufacturer SaveAndUpdateProductManufacturer(ProductManufacturer productManufacturerData, int userId)
        {
            var productManufactrer = _currentDbContext.ProductManufacturers.FirstOrDefault(u => u.Id == productManufacturerData.Id);
            if (productManufactrer == null)
            {
                productManufactrer = new ProductManufacturer();
                productManufactrer.UpdateCreatedInfo(userId);

            }
            else
            {
                productManufactrer.UpdateUpdatedInfo(userId);
            }
            productManufactrer.Name = productManufacturerData.Name;
            productManufactrer.Note = productManufacturerData.Note;
            productManufactrer.ShowInOurBrands = productManufacturerData.ShowInOurBrands;
            productManufactrer.SortOrder = productManufacturerData.SortOrder;
            productManufactrer.TenantId = productManufacturerData.TenantId;
            productManufactrer.ImagePath = productManufacturerData.ImagePath;
            productManufactrer.TenantId = productManufacturerData.TenantId;
            _currentDbContext.Entry(productManufactrer).State = productManufacturerData.Id > 0 ? EntityState.Modified : EntityState.Added;
            _currentDbContext.SaveChanges();
            return productManufactrer;
        }

        public bool UpdateStockMovement(StockMovementCollectionViewModel data)
        {
            bool status = false;
            if (data != null && (data.StockMovements != null || data.Count > 0))
            {
                foreach (var item in data.StockMovements)
                {
                    status = Inventory.AdjustStockMovementTransactions(item);
                    if (status == false) { break; }
                }

                return status;
            }

            return status;
        }

        public Guid CreateStockMovement(int userId, int TenantId, int WarehouseId)
        {
            Guid stockMovementId = Guid.NewGuid();
            StockMovement stockMovement = new StockMovement();
            stockMovement.StockMovementId = stockMovementId;
            stockMovement.TenantId = TenantId;
            stockMovement.WarehouseId = WarehouseId;
            stockMovement.UpdateCreatedInfo(userId);
            _currentDbContext.StockMovements.Add(stockMovement);
            _currentDbContext.SaveChanges();
            return stockMovementId;
        }
    }
}