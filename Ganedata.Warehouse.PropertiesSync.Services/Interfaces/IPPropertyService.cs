using System.Collections.Generic;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Interfaces
{
    public interface IPPropertyService
    {
        List<PProperty> GetPropertiesInfoForSite(int siteId);
    }

    
}