using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Services
{
    public class TenantLocationServices : ITenantLocationServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IProductServices _productServices;
        private readonly IMarketServices _marketServices;

        public TenantLocationServices(IApplicationContext currentDbContext, IProductServices productServices, IMarketServices marketServices)
        {
            _currentDbContext = currentDbContext;
            _productServices = productServices;
            _marketServices = marketServices;
        }

        public IEnumerable<TenantLocations> GetAllTenantLocations(int tenantId)
        {
            return _currentDbContext.TenantWarehouses.Where(e => e.TenantId == tenantId && e.IsDeleted != true).ToList();
        }

        public IEnumerable<TenantLocations> GetAllMobileTenantLocations(int tenantId)
        {
            return _currentDbContext.TenantWarehouses.Where(e => e.TenantId == tenantId && e.IsMobile == true && e.IsDeleted != true).ToList();
        }

        public TenantLocations GetTenantLocationById(int tenantLocationsId)
        {
            return _currentDbContext.TenantWarehouses.Find(tenantLocationsId);
        }

        public IEnumerable<TenantLocations> GetTenantLocationListById(int locationId, int tenantId)
        {
            return _currentDbContext.TenantWarehouses.Where(e => (e.WarehouseId == locationId || e.ParentWarehouseId == locationId) && e.IsDeleted != true);
        }

        public TenantLocations GetActiveTenantLocationById(int tenantLocationsId)
        {
            return _currentDbContext.TenantWarehouses.FirstOrDefault(e => e.WarehouseId == tenantLocationsId && e.IsDeleted != true && e.IsActive == true);
        }

        public int SaveTenantLocation(TenantLocations tenantLocations, int userId, int tenantId)
        {
            tenantLocations.DateCreated = DateTime.UtcNow;
            tenantLocations.DateUpdated = DateTime.UtcNow;
            tenantLocations.CreatedBy = userId;
            tenantLocations.UpdatedBy = userId;
            tenantLocations.TenantId = tenantId;

            _currentDbContext.Entry(tenantLocations).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return tenantLocations.WarehouseId;

        }

        public void UpdateTenantLocation(TenantLocations tenantLocations, int userId, int tenantId)
        {
            tenantLocations.DateUpdated = DateTime.UtcNow;
            tenantLocations.UpdatedBy = userId;
            tenantLocations.TenantId = tenantId;
            tenantLocations.PalletTrackingScheme = tenantLocations.PalletTrackingScheme;
            _currentDbContext.Entry(tenantLocations).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public void DeleteTenantLocation(TenantLocations tenantLocations, int userId)
        {
            tenantLocations.IsDeleted = true;
            tenantLocations.UpdatedBy = userId;
            tenantLocations.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(tenantLocations).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public int GetTenantIdByTenantLocationId(int tenantLocation)
        {
            return _currentDbContext.TenantWarehouses.FirstOrDefault(e => e.WarehouseId == tenantLocation && e.IsActive == true && e.IsDeleted != true).TenantId;
        }

        public ProductLocationStockLevel UpdateProductLevelsForTenantLocation(int warehouseId, int productId, decimal stockQty, int userId)
        {
            var stockLevel = _currentDbContext.ProductLocationStockLevels.FirstOrDefault(m => m.ProductMasterID == productId && m.TenantLocationID == warehouseId && m.IsDeleted != true);
            var warehouse = _currentDbContext.TenantWarehouses.Find(warehouseId);
            if (stockLevel == null)
            {
                stockLevel = new ProductLocationStockLevel()
                {
                    ProductMasterID = productId,
                    TenantId = warehouse.TenantId,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    MinStockQuantity = stockQty,
                    TenantLocationID = warehouseId,
                    UpdatedBy = userId
                };
                _currentDbContext.ProductLocationStockLevels.Add(stockLevel);
            }
            else
            {
                stockLevel.MinStockQuantity = stockQty;
                stockLevel.UpdatedBy = userId;
                stockLevel.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(stockLevel).State = EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
            return stockLevel;
        }

        public IQueryable<WarehouseProductLevelViewModel> GetAllStockLevelsForWarehouse(int warehouseId)
        {
            var warehouse = _currentDbContext.TenantWarehouses.Find(warehouseId);
            var allProducts = _currentDbContext.ProductMaster.Where(u => u.TenantId == warehouse.TenantId && u.IsDeleted != true);
            var allStockLevels = _currentDbContext.ProductLocationStockLevels.Where(m => m.TenantLocationID == warehouseId && m.IsDeleted != true);
            var levels =
                from p in allProducts
                join a in allStockLevels on p.ProductId equals a.ProductMasterID into tmpGroups
                from g in tmpGroups.DefaultIfEmpty()
                select new WarehouseProductLevelViewModel()
                {
                    ProductName = p.Name,
                    SKUCode = p.SKUCode,
                    ProductID = p.ProductId,
                    TenantLocationID = warehouseId,
                    ReOrderQuantity = p.ReorderQty ?? 0,
                    MinStockQuantity = g == null ? 0 : g.MinStockQuantity,
                    ProductLocationStockLevelID = g == null ? 0 : g.ProductLocationStockLevelID
                };

            return levels;
        }
    }
}