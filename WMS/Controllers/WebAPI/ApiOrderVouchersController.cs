using System.Threading.Tasks;
using System.Web.Http;
using Ganedata.Core.Models.AdyenPayments;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiOrderVouchersController : BaseApiController
    {
        
        private readonly IShoppingVoucherService _shoppingVoucherService;
        public ApiOrderVouchersController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, 
            IProductServices productServices, IUserService userService, IShoppingVoucherService shoppingVoucherService) :
            base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _shoppingVoucherService = shoppingVoucherService;
        }

        public async Task<IHttpActionResult> ValidateShoppingVoucher(ShoppingVoucherValidationRequestModel data)
        {
            return Ok(_shoppingVoucherService.ValidateVoucher(data));
        }
        public async Task<IHttpActionResult> UserPromotions(int uid, string em, string ph)
        {
            return Ok(_shoppingVoucherService.GetAllValidUserVouchers(uid, em, ph));
        }

    }
}