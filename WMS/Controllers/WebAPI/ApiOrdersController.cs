using System.Threading.Tasks;
using System.Web.Http;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ApiOrdersController : BaseApiController
    {
        private readonly IProductServices _productService;

        public ApiOrdersController(IProductServices productService, ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _productService = productService;
        }

        public async Task<IHttpActionResult> VerifyProductInfoBySerial(ProductDetailRequest request)
        {
            var result = await _productService.GetProductInfoBySerial(request.SerialCode, request.TenantId);

            return Ok(result);
        }

    }
}