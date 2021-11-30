using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Ganedata.Warehouse.PropertiesSync.Data.Helpers;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.SyncLibrary
{
    public class PTenantResponse
    {
        public List<PTenant> Tenants { get; set; }
    }
    public class PLandlordResponse
    {
        public List<PLandlord> Landlords { get; set; }
    }
    public class PPropertyResponse
    {
        public List<PProperty> Properties { get; set; }
    }

    public class PropertySyncItemsResponse
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public string ItemCode { get; set; }

        public string Description { get; set; }

    }

    public class SyncServiceHelper
    {
        private static readonly HttpClient Client = new HttpClient();

        private JavaScriptSerializer JSerializer { get; set; }

        public SyncServiceHelper()
        {
            Client.BaseAddress = new Uri(GanedataGlobalConfigurations.WarehouseStoreBaseUri);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.Timeout = new TimeSpan(0, 1, 0, 0);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            JSerializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
        }

        public async Task<List<PropertySyncItemsResponse>> SyncImportedTenantsToWarehouseStore(int siteId, List<PTenant> tenants)
        {
            //Prepare the request as JSON
            var request = JSerializer.Serialize(new PTenantResponse() { Tenants = tenants });
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            //Send the request to receive the Response
            var response = await Client.PostAsync("api/warehouse-sync/sync-tenants/" + GanedataGlobalConfigurations.WarehouseSyncSiteId, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SyncLogger.WriteLog("Unexpected response from api : " + response.StatusCode + " : " + response.ReasonPhrase);
            }

            return new List<PropertySyncItemsResponse>();
        }

        public async Task<List<PropertySyncItemsResponse>> SyncImportedLandlordsToWarehouseStore(int siteId,
            List<PLandlord> landlords)
        {
            //Prepare the request as JSON
            var request = JSerializer.Serialize(new PLandlordResponse() { Landlords = landlords });
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            //Send the request to receive the Response
            var response = await Client.PostAsync("api/warehouse-sync/sync-landlords/" + GanedataGlobalConfigurations.WarehouseSyncSiteId, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SyncLogger.WriteLog("Unexpected response from api : " + response.StatusCode + " : " + response.ReasonPhrase);
            }

            return new List<PropertySyncItemsResponse>();
        }

        public async Task<List<PropertySyncItemsResponse>> SyncImportedPropertiesToWarehouseStore(int siteId,
            List<PProperty> properties)
        {
            //Prepare the request as JSON
            var request = JSerializer.Serialize(new PPropertyResponse() { Properties = properties });
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            //Send the request to receive the Response
            var response = await Client.PostAsync("api/warehouse-sync/sync-properties/" + GanedataGlobalConfigurations.WarehouseSyncSiteId, content);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SyncLogger.WriteLog("Unexpected response from api : " + response.StatusCode + " : " + response.ReasonPhrase);
            }

            return new List<PropertySyncItemsResponse>();
        }


    }
}