using Ganedata.Warehouse.PropertiesSync.Data.Entities;
using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.SyncData;
using System;

namespace Ganedata.Warehouse.PropertiesSync.Data
{
    public interface IRepository : IDisposable
    {
        PropertiesSyncEntities GetContext(int siteId);
        SyncDataDbContext GetSyncDbContext();
    }

    public class Repository : IRepository
    {
        public void Dispose()
        {

        }

        public PropertiesSyncEntities GetContext(int siteId)
        {
            if (GanedataGlobalConfigurations.WarehouseSyncSiteId < 1)
            {
                return new PropertiesSyncEntities("name=PropertiesSyncEntities");
            }
            return new PropertiesSyncEntities($"name=PropertiesSyncEntities");
        }
        public SyncDataDbContext GetSyncDbContext()
        {
            return new SyncDataDbContext();
        }
    }
}