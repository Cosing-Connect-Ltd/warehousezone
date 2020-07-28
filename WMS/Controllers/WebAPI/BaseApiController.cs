using Ganedata.Core.Services;
using System.Web.Http;

namespace WMS.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly ITerminalServices TerminalServices;
        protected readonly ITenantLocationServices TenantLocationServices;
        protected readonly IOrderService OrderService;
        protected readonly IProductServices ProductServices;
        protected readonly IUserService UserService;

        public BaseApiController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
        {
            TerminalServices = terminalServices;
            TenantLocationServices = tenantLocationServices;
            OrderService = orderService;
            ProductServices = productServices;
            UserService = userService;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
    }
}