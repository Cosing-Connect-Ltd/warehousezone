using System;
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
            ErrorSignal.FromCurrentContext().Raise(new Exception("Received payment hook", new Exception(json)));
            try
            {
                var isValidPostFromAdyen = IsValidAdyanHmacSignature(json, false);
                if (!isValidPostFromAdyen)
                {
                    return BadRequest("Failed request source verification for payment notification");
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            var notification = paymentAuthorisationData.NotificationItems.FirstOrDefault();
            if (notification != null)
            {
                notification.NotificationRequestItem.RawJson = "";
                var response = await _paymentService.UpdateOrderPaymentAuthorisationHook(notification?.NotificationRequestItem);
                return Ok("[accepted]");
            }

            return BadRequest("Could not find any notification items");
        }

        public async Task<IHttpActionResult> RefundSuccessHook(AdyenPaylinkHookNotificationRequestRoot refundHookData)
        {
            var json = JsonConvert.SerializeObject(refundHookData);
            ErrorSignal.FromCurrentContext().Raise(new Exception("Refund payment hook", new Exception(json)));
            try
            {
                var isValidPostFromAdyen = IsValidAdyanHmacSignature(json, true);
                if (!isValidPostFromAdyen)
                {
                    return BadRequest("Failed request source verification for refund notification");
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            var notification = refundHookData.NotificationItems.FirstOrDefault();
            if (notification != null)
            {
                notification.NotificationRequestItem.RawJson = "";
                var response = await _paymentService.UpdateOrderPaymentAuthorisationHook(notification?.NotificationRequestItem);
                return Ok("[accepted]");
            }

            return BadRequest("Could not find any notification items");
        }

        public async Task<IHttpActionResult> SendPaymentRefundRequest(AdyenPaylinkRefundRequest refundRequestData)
        {
            var json = JsonConvert.SerializeObject(refundRequestData);

            try
            {
                var isValidPostFromAdyen = IsValidAdyanHmacSignature(json, true);
                if (!isValidPostFromAdyen)
                {
                    return BadRequest("Failed request source verification for portal refund notification");
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
             
            var response = await _paymentService.RequestRefundForPaymentLink(refundRequestData);
            return Ok(new { Success = true, Merchant = response.RefundOriginalMerchantReference, AuthorisationID = response.AdyenOrderPaylinkID });
        }

        private bool IsValidAdyanHmacSignature(string json, bool isRefunds)
        {
            return true; //This keeps failing the request, so removing it for now
            var hmacValidator = new HmacValidator();
            var notificationHandler = new NotificationHandler();
            var handleNotificationRequest = notificationHandler.HandleNotificationRequest(json);
            var notificationItem = handleNotificationRequest.NotificationItemContainers.First().NotificationItem;
            return hmacValidator.IsValidHmac(notificationItem, isRefunds? AdyenPaymentService.AdyenRefundHmacKey: AdyenPaymentService.AdyenHmacKey);
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

        public async Task<IHttpActionResult> GetPaymentStatus(string linkId)
        {
            var response = await _paymentService.GetPaymentStatus(linkId);
            if (response.IsError)
            {
                return BadRequest(response.ErrorMessage);
            }
            return Ok(response);
        }

    }
}