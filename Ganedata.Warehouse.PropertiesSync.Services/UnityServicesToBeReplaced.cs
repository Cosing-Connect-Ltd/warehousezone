using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Warehouse.PropertiesSync.Data;
using Ganedata.Warehouse.PropertiesSync.Services.Implementations;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData;

namespace Ganedata.Warehouse.PropertiesSync.Services
{
    public class UnityServicesToBeReplaced
    {
        public static SyncDataDbContext SyncDataDbContext = new SyncDataDbContext();

        public static PTenantsService PTenantsService = new PTenantsService();
        public static PPropertyService PPropertyService = new PPropertyService();
        public static PLandlordService PLandlordService = new PLandlordService();
        public static SiteSyncService SiteSyncService = new SiteSyncService();
        public static Repository Repository = new Repository();
         
    }
}
