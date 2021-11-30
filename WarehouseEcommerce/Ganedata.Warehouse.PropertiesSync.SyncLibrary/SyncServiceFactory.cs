using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.Services;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ganedata.Warehouse.PropertiesSync.SyncLibrary
{
    public class SyncServiceFactory
    {
        private ISiteSyncService SiteSyncService { get; set; }

        private SyncServiceHelper SyncServiceHelper { get; set; }

        public SyncServiceFactory()
        {
            SiteSyncService = UnityServicesToBeReplaced.SiteSyncService;
            SyncServiceHelper = new SyncServiceHelper();
        }
        public async Task ImportDataFromSites()
        {
            try
            {

                var syncHistory = new PSyncHistory { SyncStartTime = DateTime.UtcNow };

                var importResponse = await SiteSyncService.ExecuteSyncProcess(GanedataGlobalConfigurations.WarehouseSyncSiteId);
                syncHistory.ImportCompletedTime = DateTime.UtcNow;

                SyncLogger.WriteLog("ImportDataFromSites > ExecuteSyncProcess completed successfully");

                syncHistory.TenantsSynced = importResponse.SyncedTenants.Count();
                syncHistory.LandlordsSynced = importResponse.SyncedLandlords.Count();
                syncHistory.PropertiesSynced = importResponse.SyncedProperties.Count();

                await SyncServiceHelper.SyncImportedLandlordsToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, importResponse.SyncedLandlords);
                SyncLogger.WriteLog("Landlords sent to warehouse store successfully :" + syncHistory.LandlordsSynced);

                foreach (var item in importResponse.SyncedLandlords)
                {
                    //send info to web service and update locally as synced
                    SiteSyncService.UpdateLandlordAsSynced(item.SiteId, item.LandlordCode);
                }

                await SyncServiceHelper.SyncImportedPropertiesToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, importResponse.SyncedProperties);
                SyncLogger.WriteLog("Properties sent to warehouse store successfully: " + syncHistory.PropertiesSynced);
                foreach (var item in importResponse.SyncedProperties)
                {
                    SiteSyncService.UpdatePropertyAsSynced(item.SiteId, item.PropertyCode);
                }

                var tenantBatches = importResponse.SyncedTenants.Batches(500);
                foreach (var batch in tenantBatches)
                {
                    await SyncServiceHelper.SyncImportedTenantsToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, batch.ToList());
                }
                SyncLogger.WriteLog("Tenants sent to warehouse store successfully: " + syncHistory.TenantsSynced);

                foreach (var item in importResponse.SyncedTenants)
                {
                    //send info to web service and update locally as synced
                    SiteSyncService.UpdateTenantAsSynced(item.SiteId, item.TenantCode);
                }

                //## BELOW CODE MAY PROCESS NONE MOSTLY, BUT THIS WILL MAKE SURE ANY FAILED RECORDS WHICH ARE NOT SYNCED YET, WILL BE SENT AGAIN FOR SYNC
                Thread.Sleep(25000);

                Thread.CurrentThread.IsBackground = true;

                SyncLogger.WriteLog("Executing post sync process ExportSyncedItemsOnly.");
                await ExportSyncedItemsOnly();

                syncHistory.SyncCompletedTime = DateTime.UtcNow;
                SiteSyncService.AddSyncHistory(syncHistory);
            }
            catch (Exception ex)
            {
                SyncLogger.WriteLog("Error Occurred : " + ex.Message + "\nTrace" + ex.StackTrace);
            }
        }

        public async Task ExportSyncedItemsOnly()
        {
            await ExportLandlordsOnly();
            await ExportPropertiesOnly();
            await ExportTenantsOnly();
        }

        public async Task ExportLandlordsOnly()
        {
            var nonSyncedLandlords = SiteSyncService.GetLandlordsFromWarehouseSync(true);
            await SyncServiceHelper.SyncImportedLandlordsToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, nonSyncedLandlords);
            //send info to web service and update locally as synced
            SiteSyncService.UpdateLandlordAsSynced(GanedataGlobalConfigurations.WarehouseSyncSiteId, nonSyncedLandlords);
        }
        public async Task ExportPropertiesOnly()
        {
            var nonSyncedProperties = SiteSyncService.GetPropertiesFromWarehouseSync(true);
            foreach (var props in nonSyncedProperties.Batches(500))
            {
                var propertyBatches = props.ToList();
                await SyncServiceHelper.SyncImportedPropertiesToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, propertyBatches);
                //send info to web service and update locally as synced
                SiteSyncService.UpdatePropertiesAsSynced(GanedataGlobalConfigurations.WarehouseSyncSiteId, propertyBatches);
            }

        }

        public async Task ExportTenantsOnly()
        {
            var nonSyncedTenants = SiteSyncService.GetTenantsFromWarehouseSync(true).ToList();
            var tenantBatches = nonSyncedTenants.Batches(5000);
            foreach (var batch in tenantBatches)
            {
                await SyncServiceHelper.SyncImportedTenantsToWarehouseStore(GanedataGlobalConfigurations.WarehouseSyncSiteId, batch.ToList());
            }
            //send info to web service and update locally as synced
            SiteSyncService.UpdateTenantsAsSynced(GanedataGlobalConfigurations.WarehouseSyncSiteId, nonSyncedTenants);
        }


    }

}