using Ganedata.Core.Entities.Domain.ImportModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class DeliverectWebhooksController : BaseApiController
    {
        private readonly IDeliverectSyncService _deliverectSyncService;
        public DeliverectWebhooksController(IDeliverectSyncService deliverectSyncService, ITenantLocationServices tenantLocationServices, IOrderService orderService,
            IProductServices productServices, IUserService userService, ITerminalServices terminalServices) :
                base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _deliverectSyncService = deliverectSyncService;
        }
        public async Task<IHttpActionResult> ChannelStatusUpdated(DeliverectChannelRegisterWebhookRequest request)
        {
            var currentHostUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var response = new DeliverectChannelRegisterWebhookResponse
            {
                MenuUpdateURL = currentHostUrl + "/api/deliverect/menuPushed",
                SnoozeUnsnoozeURL = currentHostUrl + "/api/deliverect/productSnoozeChanged",
                StatusUpdateURL = currentHostUrl + "/api/deliverect/orderStatusUpdated",
                BusyModeURL = string.Empty,
                DisabledProductsURL = string.Empty
            };

            await _deliverectSyncService.SyncChannelLinks();
            await _deliverectSyncService.SyncProducts(null, 0);

            return Json(response);
        }
        public async Task<IHttpActionResult> MenuPushed(List<DeliverectMenuUpdatedWebhookRequest> request)
        {
            await _deliverectSyncService.SyncChannelLinks();
            await _deliverectSyncService.SyncProducts(null, 0);
            return Ok();
        }

        public async Task<IHttpActionResult> ProductSnoozeChanged(DeliverectProductSnoozeChangedWebhookRequest request)
        {
            //TODO: snooz and unsnooz feature should be based on channels/locations, meaning thet a product could be snoozed in one location(store) but not in the other location(store).
            await _deliverectSyncService.SyncChannelLinks();
            await _deliverectSyncService.SyncProducts(null, 0);
            return Ok();
        }

        public IHttpActionResult OrderStatusUpdated(DeliverectOrderStatusUpdatedWebhookRequest request)
        {
            var orderStatus = GetOrderStatus(request.Status);

            if (orderStatus != null)
            {
                OrderService.UpdateOrderStatus(int.Parse(request.ChannelOrderId), orderStatus.Value, 0);
            }

            return Ok();
        }

        private static OrderStatusEnum? GetOrderStatus(int status)
        {
            switch (status)
            {
                case 30:   // REJECTED
                case 120:   // FAILED
                case 121:   // POS RECEIVED FAILED
                case 124:   // PARSE FAILED
                    return OrderStatusEnum.Hold;
                case 50:  // PREPARING 
                case 60:  // PREPARED
                case 70:  // READY FOR PICKUP
                    return OrderStatusEnum.Preparing;
                case 80:   // IN DELIVERY
                    return OrderStatusEnum.OutForDelivery;
                case 110:  // CANCELED
                    return OrderStatusEnum.Cancelled;
                case 90:  // FINALIZED
                    return OrderStatusEnum.Complete;
            }

            return null;
        }
    }
}
