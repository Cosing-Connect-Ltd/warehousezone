using System.Collections.Generic;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Interfaces
{
    public interface IPLandlordService
    {
        List<PLandlord> GetLandlordsInfoForSite(int siteId);
    }
}