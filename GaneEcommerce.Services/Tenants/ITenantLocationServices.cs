using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Services
{
    public interface ITenantLocationServices
    {
        IEnumerable<TenantLocations> GetAllTenantLocations(int tenantId, bool includeDeleted = false);
        IEnumerable<TenantLocations> GetAllMobileTenantLocations(int tenantId);
        TenantLocations GetTenantLocationById(int tenantLocationsId);
        TenantLocations GetActiveTenantLocationById(int tenantLocationsId);
        int SaveTenantLocation(TenantLocations tenantLocations, int userId, int tenantId);
        void UpdateTenantLocation(TenantLocations tenantLocations, int userId, int tenantId);
        void DeleteTenantLocation(TenantLocations tenantLocations, int userId);

        IEnumerable<TenantLocations> GetTenantLocationListById(int locationId, int tenantId);

        int GetTenantIdByTenantLocationId(int tenantLocation);

        ProductLocationStockLevel UpdateProductLevelsForTenantLocation(int warehouseId, int productId, decimal stockQty, int userId);

        IQueryable<WarehouseProductLevelViewModel> GetAllStockLevelsForWarehouse(int warehouseId);
        IQueryable<WarehouseProductLevelViewModel> GetAllStockLevelsForTenant(int tenantId);
    }
}
