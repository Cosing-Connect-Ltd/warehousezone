using System.Collections.Generic;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;
using Ganedata.Warehouse.PropertiesSync.Data.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Interfaces
{
    public interface ITenantsService
    {
        List<PTenant> GetTenantsForSite(int siteId);
        List<PTenant> GetAllTenantsFromAllSites();
    }
}