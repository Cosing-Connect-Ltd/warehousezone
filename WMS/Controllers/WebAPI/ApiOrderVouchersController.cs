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
        [HttpPost]
        public async Task<IHttpActionResult> ValidateShoppingVoucher(ShoppingVoucherValidationRequestModel data)
        {
            return Ok(_shoppingVoucherService.ValidateVoucher(data));
        }

        [HttpPost]
        public async Task<IHttpActionResult> UserPromotions(PromotionsSyncRequest request)
        {
            return Ok(_shoppingVoucherService.GetAllValidUserVouchers(request.UserId, request.Email, request.Phone));
        }
        [HttpPost]
        public async Task<IHttpActionResult> AddPromotion(ShoppingVoucherValidationRequestModel data)
        {
            return Ok(_shoppingVoucherService.AddFriendReferralVoucher(data));
        }



    }
}