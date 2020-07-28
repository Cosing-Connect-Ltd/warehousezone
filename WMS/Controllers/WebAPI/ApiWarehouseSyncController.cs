using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Ganedata.Core.Services;
using WMS.Helpers;
using Ganedata.Core.Entities.Enums;

namespace WMS.Controllers
{
    public class ApiWarehouseSyncController : BaseApiController
    {
        private readonly IWarehouseSyncService _warehouseSyncService;
        private readonly IGaneConfigurationsHelper _configurationsHelper;

        public ApiWarehouseSyncController(IWarehouseSyncService warehouseSyncService, IGaneConfigurationsHelper configurationsHelper, ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _warehouseSyncService = warehouseSyncService;
            _configurationsHelper = configurationsHelper;
        }

        //WEB API URL : api/warehouse-sync/sync-tenants/{siteId}
        public async Task<IHttpActionResult> SyncPTenants(int siteId)
        {
            var postedData = PTenantResponse.BindJson(new StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());

            var processedItems = await _warehouseSyncService.UpdateTenantInformationForSite(siteId, postedData.Tenants);

            var result = PropertySyncItemsResponse.MapAll(processedItems, SyncEntityTypeEnum.Tenants);

            return Ok(result);
        }
        //WEB API URL : api/warehouse-sync/sync-landlords/{siteId}
        public async Task<IHttpActionResult> SyncPLandlords(int siteId)
        {
            var postedData = PLandlordResponse.BindJson(new StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());

            var processedItems = await _warehouseSyncService.UpdateLandlordInformationForSite(siteId, postedData.Landlords);

            var result = PropertySyncItemsResponse.MapAll(processedItems, SyncEntityTypeEnum.Landlords);

            return Ok(result);
        }

        //WEB API URL :api/warehouse-sync/sync-properties/{siteId}
        public async Task<IHttpActionResult> SyncPProperties(int siteId)
        {
            var postedData = PPropertyResponse.BindJson(new StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd());

            var processedItems = await _warehouseSyncService.UpdatePropertiesInformationForSite(siteId, postedData.Properties);

            var result = PropertySyncItemsResponse.MapAll(processedItems, SyncEntityTypeEnum.Properties);

            return Ok(result);
        }

        //WEB API URL : api/warehouse-emails/send-notifications
        public async Task<IHttpActionResult> SendOutEmailNotificationsFromQueue(int tenantId)
        {
            await _configurationsHelper.DispatchTenantEmailNotificationQueues(tenantId);
            return Ok(true);
        }
    }
}
