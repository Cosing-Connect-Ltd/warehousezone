using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class OrderScheduleController : BaseController
    {
        private readonly IAppointmentsService _appointmentsService;

        public OrderScheduleController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAppointmentsService appointmentsService) : 
                                  base(orderService, propertyService, accountServices, lookupServices)
        {
            _appointmentsService = appointmentsService;
        }

        // GET: OrderSchedule
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _OrdersToSchedule()
        {
            var allOrders = OrderService.GetAllOrders(CurrentTenantId, CurrentWarehouseId)
                                        .Where(o => (o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder ||
                                                    o.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder) &&
                                                    (o.OrderStatusID == OrderStatusEnum.Active || 
                                                    o.OrderStatusID == OrderStatusEnum.Complete ||
                                                    o.OrderStatusID == OrderStatusEnum.BeingPicked ||
                                                    o.OrderStatusID == OrderStatusEnum.Approved ||
                                                    o.OrderStatusID == OrderStatusEnum.NotScheduled ||
                                                    o.OrderStatusID == OrderStatusEnum.Scheduled) &&
                                                    o.OrderProcess.All(op => op.OrderProcessStatusId < OrderProcessStatusEnum.Delivered))
                                        .ToList();

            return PartialView(allOrders);
        }

        public ActionResult SchedulerPartial()
        {
            return PartialView("_SchedulerPartial", OrderSchedulerSettings.DataObject);
        }

        public ActionResult SchedulerPartialEditAppointment()
        {
            try
            {
                OrderSchedulerSettings.UpdateEditableDataObject();
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            return PartialView("_SchedulerPartial", OrderSchedulerSettings.DataObject);
        }

        public ActionResult CreateAppointment(string start, string end, string subject, string resourceId, int orderId, int? joblabel, int tenantId)
        {
            if (!caSession.AuthoriseSession()) { return Json(new { Message = "You are not authorised to perofrm this operation" }); }

            _appointmentsService.CreateOrderScheduleAppointment(start, end, subject, resourceId, joblabel ?? 0, tenantId, orderId);

            return PartialView("_SchedulerPartial", OrderSchedulerSettings.DataObject);
        }

        public ActionResult FixAppts()
        {
            _appointmentsService.UpdateAllAppointmentSubjects();
            return RedirectToAction("Index", "Appointments");
        }
    }
}
