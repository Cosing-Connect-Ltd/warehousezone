using System.Collections.Generic;
using System.Threading.Tasks;
using Ganedata.Warehouse.PropertiesSync.Services.HelperModels;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Interfaces
{
    public interface ISiteSyncService
    {
        /// <summary>
        /// Method returning Synced objects
        /// </summary>
        /// <returns>Returns list of all the tenants that are already synced</returns>
        List<PTenant> GetTenantsFromWarehouseSync(bool onlyRequireSync=false);

        /// <summary>
        /// Method returning Synced objects
        /// </summary>
        /// <returns>Returns list of all the landlords that are already synced</returns>
        List<PLandlord> GetLandlordsFromWarehouseSync(bool onlyRequireSync = false);


        /// <summary>
        /// Method returning Synced objects
        /// </summary>
        /// <returns>Returns list of all the properties that are already synced</returns>
        List<PProperty> GetPropertiesFromWarehouseSync(bool onlyRequireSync = false);

        /// <summary>
        /// Method to update tenants info at regular intervals set in the configuration
        /// </summary>
        /// <param name="siteId">SiteID decides, which set of data needs updating</param>
        /// <returns>Returns set of items that has been added or changed</returns>
        Task<List<PTenant>> UpdateTenantInformationForSite(int siteId);

        /// <summary>
        /// Method to update Landlords info at regular intervals set in the configuration
        /// </summary>
        /// <param name="siteId">SiteID decides, which set of data needs updating</param>
        /// <returns>Returns set of items that has been added or changed</returns>
        Task<List<PLandlord>> UpdateLandlordInformationForSite(int siteId);

        /// <summary>
        /// Method to update properties info at regular intervals set in the configuration
        /// </summary>
        /// <param name="siteId">SiteID decides, which set of data needs updating</param>
        /// <returns>Returns set of items that has been added or changed</returns>
        Task<List<PProperty>> UpdatePropertiesInformationForSite(int siteId);

        Task<PropertySyncFinalResponse> ExecuteSyncProcess(int siteId);

        void UpdateTenantAsSynced(int siteId, string tenantCode);
        void UpdateTenantsAsSynced(int siteId, List<PTenant> tenantsToSet);

        void UpdateLandlordAsSynced(int siteId, string landlordCode);
        void UpdateLandlordAsSynced(int siteId, List<PLandlord> landlordsToSet);

        void UpdatePropertyAsSynced(int siteId, string propertyCode);
        void UpdatePropertiesAsSynced(int siteId, List<PProperty> propertiesToSet);

        void AddSyncHistory(PSyncHistory syncHistory);
    }
}