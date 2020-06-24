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

        // GET http://localhost:8005/api/websites/AbandonedCarts/SendNotification}
        [HttpPost]
        public IHttpActionResult SendNotificationForAbandonedCarts()
        {
            _tenantWebsiteService.SendNotificationForAbandonedCarts();

            return Ok();
        }
    }
}