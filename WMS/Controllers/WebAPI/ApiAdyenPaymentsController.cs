using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Adyen.Model.Notification;
using Adyen.Notification;
using Adyen.Util;
using Elmah;
using Ganedata.Core.Models.AdyenPayments;
using Ganedata.Core.Services;
using Ganedata.Core.Services.Feedbacks;
using Newtonsoft.Json;

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
        public async Task<IHttpActionResult> PaymentSuccessHook(AdyenPaylinkHookNotificationRequestRoot paymentAuthorisationData)
        {
            var json = JsonConvert.SerializeObject(paymentAuthorisationData);
            var isValidPostFromAdyen = IsValidAdyanHmacSignature(json);
            if (!isValidPostFromAdyen)
            {
                return BadRequest("Failed request source verification");
            }

            var notification = paymentAuthorisationData.NotificationItems.FirstOrDefault();
            notification.NotificationRequestItem.RawJson = "";
            var response = await _paymentService.UpdateOrderPaymentAuthorisationHook(notification?.NotificationRequestItem);
            return Ok(new { Success = true, Merchant = notification?.NotificationRequestItem.MerchantAccountCode, AuthorisationID = response.AdyenOrderPaylinkID });
        }

        private bool IsValidAdyanHmacSignature(string json)
        {
            var hmacValidator = new HmacValidator();
            var notificationHandler = new NotificationHandler();
            var handleNotificationRequest = notificationHandler.HandleNotificationRequest(json);
            var notificationItem = handleNotificationRequest.NotificationItemContainers.First().NotificationItem;
            return hmacValidator.IsValidHmac(notificationItem, AdyenPaymentService.AdyenHmacKey);
        }
        public async Task<IHttpActionResult> CreateOrderPaymentLink(AdyenApiCreatePayLinkRequestModel model)
        {
            var requestModel = model.GetBase();
            var response = await _paymentService.GenerateOrderPaymentLink(requestModel);
            if (response.IsError)
            {
                return BadRequest(response.ErrorMessage);
            }
            await _paymentService.CreateOrderPaymentLink(response, model.OrderID);
            return Ok(response);
        }
    }
}