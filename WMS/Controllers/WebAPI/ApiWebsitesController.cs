using Ganedata.Core.Services;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class ApiWebsitesController : BaseApiController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;

        public ApiWebsitesController(ITerminalServices terminalServices,
            ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ITenantWebsiteService tenantWebsiteService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }

        // POST http://localhost:8005/api/websites/SendNotificationForAbandonedCarts}
        [HttpPost]
        public IHttpActionResult SendNotificationForAbandonedCarts()
        {
            _tenantWebsiteService.SendNotificationForAbandonedCarts();

            return Ok();
        }

        // POST http://localhost:8005/api/websites/SendProductAvailabilityNotifications}
        [HttpPost]
        public IHttpActionResult SendProductAvailabilityNotifications()
        {
            _tenantWebsiteService.SendProductAvailabilityNotifications();

            return Ok();
        }
    }
}