using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Elmah;
using Ganedata.Core.Models.AdyenPayments;
using Ganedata.Core.Services;
using Ganedata.Core.Services.Feedbacks;

namespace WMS.Controllers.WebAPI
{
    public class ApiAdyenPaymentsController : BaseApiController
    {
        private readonly IAdyenPaymentService _paymentService;

        public ApiAdyenPaymentsController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, IFeedbackService feedbackService, IAdyenPaymentService paymentService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _paymentService = paymentService;
        }
        public async Task<IHttpActionResult> ReceiveAdyenPaymentCallback(AdyenPaylinkHookNotificationRequest paymentAuthorisationData)
        {
            string result = await Request.Content.ReadAsStringAsync();
            paymentAuthorisationData.RawJson = result;
            var response = await _paymentService.UpdateOrderPaymentAuthorisationHook(paymentAuthorisationData);
            return Ok(new { Success = true, Merchant = paymentAuthorisationData.MerchantAccountCode, AuthorisationID = response.AdyenOrderPaylinkID });
        }

    }
}