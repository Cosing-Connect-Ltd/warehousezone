using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class OrderScheduleController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IAppointmentsService _appointmentsService;
        private readonly IUserService _userService;
        private readonly IGaneConfigurationsHelper _emailNotificationsHelper;
        private readonly IPalletingService _palletingService;
        private readonly IMapper _mapper;

        public OrderScheduleController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IEmployeeServices employeeServices,
            IAppointmentsService appointmentsService, IUserService userService, IGaneConfigurationsHelper emailNotificationsHelper, IMapper mapper,IPalletingService palletingService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeServices = employeeServices;
            _appointmentsService = appointmentsService;
            _userService = userService;
            _emailNotificationsHelper = emailNotificationsHelper;
            _palletingService = palletingService;
            _mapper = mapper;

        }
        // GET: OrderSchedule
        public ActionResult Index()
        {

            return View();

        }

        public ActionResult _PalletToDispatch()
        {


            var allOrders = _palletingService.GetAllPalletsDispatch(); 


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


        public async Task<ActionResult> CreateAppointment(string start, string end, string subject, string resourceId, int? orderId, int? joblabel, int tenantId, int dispatchId)
        {
            if (!caSession.AuthoriseSession()) { return Json(new { Message = "You are not authorised to perofrm this operation" }); }

            var appointment = _appointmentsService.CreateOrderScheduleAppointment(start, end, subject, resourceId,joblabel??0, tenantId,dispatchId);
            if (appointment != null)
            {
                var order = _palletingService.UpdatePalletsDispatchStatus(dispatchId,CurrentUserId);

            }

            // send resources as per filter values
           return PartialView("_SchedulerPartial", OrderSchedulerSettings.DataObject);
        }

        public ActionResult FixAppts()
        {
            _appointmentsService.UpdateAllAppointmentSubjects();
            return RedirectToAction("Index", "Appointments");
        }

    }
}
