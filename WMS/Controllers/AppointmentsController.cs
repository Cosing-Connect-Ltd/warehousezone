using AutoMapper;
using DevExpress.Web.Mvc;
using Elmah;
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
    public class AppointmentsController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IAppointmentsService _appointmentsService;
        private readonly IUserService _userService;
        private readonly IGaneConfigurationsHelper _emailNotificationsHelper;
        private readonly IMapper _mapper;

        public AppointmentsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IEmployeeServices employeeServices,
            IAppointmentsService appointmentsService, IUserService userService, IGaneConfigurationsHelper emailNotificationsHelper, IMapper mapper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeServices = employeeServices;
            _appointmentsService = appointmentsService;
            _userService = userService;
            _emailNotificationsHelper = emailNotificationsHelper;
            _mapper = mapper;

        }
        // GET: Appointments
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var resources = _employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId).ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.ResourceId.ToString() }).ToList();
            resources.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            ViewBag.WorksResources = resources;

            var jobTypes = LookupServices.GetAllJobTypes(CurrentTenantId).Select(x => new SelectListItem() { Text = x.Name, Value = x.JobTypeId.ToString() }).ToList();
            jobTypes.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            ViewBag.ResourcejobTypes = jobTypes;

            return View();

        }

        public ActionResult _ListWorksOrderJobs()
        {

            var result = LookupServices.GetAllJobTypesIncludingNavProperties(CurrentTenantId).ToList();

            List<SlaWorksOrderViewModel> allOrders = new List<SlaWorksOrderViewModel>();

            for (int i = 0; i < result.Count(); i++)
            {
                allOrders.AddRange(result[i].Orders);
            }

            ViewBag.orderByPriority = allOrders.OrderByDescending(x => x.SLAPriorityId).ThenByDescending(x => x.PPropertyId).ToList();

            return PartialView(result);
        }

        public ActionResult _JobAlocationStats()
        {

            var allOrders = OrderService.GetAllOrderIdsWithStatus(CurrentTenantId).ToList();

            var allocated = allOrders.Count(c => c.OrderStatusID == OrderStatusEnum.Scheduled);
            int unallocated = allOrders.Count(c => c.OrderStatusID == OrderStatusEnum.NotScheduled);
            int reallocation = allOrders.Count(c => c.OrderStatusID == OrderStatusEnum.ReAllocationRequired);

            ViewBag.Allocated = allocated;
            ViewBag.Unallocated = unallocated;
            ViewBag.Reallocation = reallocation;

            return PartialView();
        }

        public ActionResult SchedulerPartial()
        {
            // send resources as per filter values
            var selectedJobType = int.Parse(!string.IsNullOrEmpty(Request.Params["SelectedJobType"]) ? Request.Params["SelectedJobType"] : "0");
            Session["selectedJobType"] = selectedJobType;
            return PartialView("_SchedulerPartial", AppointmentsSchedulerSettings.DataObject);

        }

        public ActionResult SchedulerPartialEditAppointment()
        {
            try
            {
                AppointmentsSchedulerSettings.UpdateEditableDataObject();
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                ViewData["SchedulerErrorText"] = e.Message;
            }

            var selectedJobType = int.Parse(!string.IsNullOrEmpty(Request.Params["SelectedJobType"]) ? Request.Params["SelectedJobType"] : "0");

            Session["selectedJobType"] = selectedJobType;


            return PartialView("_SchedulerPartial", AppointmentsSchedulerSettings.DataObject);
        }


        public async Task<ActionResult> CreateAppointment(string start, string end, string subject, string resourceId, int orderId, int joblabel, int tenantId)
        {
            if (!caSession.AuthoriseSession()) { return Json(new { Message = "You are not authorised to perofrm this operation" }); }

            var appointment = _appointmentsService.CreateAppointment(start, end, subject, resourceId, orderId, joblabel, tenantId);
            if (appointment != null)
            {
                var order = OrderService.UpdateOrderStatus(orderId, OrderStatusEnum.Scheduled, CurrentUserId);

                var result = await _emailNotificationsHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Works order scheduled", _mapper.Map(order, new OrderViewModel()), worksOrderNotificationType:
                    WorksOrderNotificationTypeEnum.WorksOrderScheduledTemplate, appointment: appointment, sendImmediately: false);

                if (result != "Success")
                {
                    ViewBag.Error = result;
                }
            }

            // send resources as per filter values
            var selectedJobType = int.Parse(!string.IsNullOrEmpty(Request.Params["SelectedJobType"]) ? Request.Params["SelectedJobType"] : "0");

            Session["selectedJobType"] = selectedJobType;
            return PartialView("_SchedulerPartial", AppointmentsSchedulerSettings.DataObject);
        }

        public ActionResult FixAppts()
        {
            _appointmentsService.UpdateAllAppointmentSubjects();
            return RedirectToAction("Index", "Appointments");
        }



        //unallocated jobs grid

        [ValidateInput(false)]
        public ActionResult UnallocatedJobsPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("UnallocatedJobs");

            if (viewModel == null)
                viewModel = AppointmentsCustomBinding.CreateUnallocatedJobsGridViewModel();

            return UnallocatedJobsGridActionCore(viewModel);
        }

        public ActionResult _UnallocatedJobsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("UnallocatedJobs");
            viewModel.Pager.Assign(pager);
            return UnallocatedJobsGridActionCore(viewModel);
        }

        public ActionResult _UnallocatedJobsFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("UnallocatedJobs");
            viewModel.ApplyFilteringState(filteringState);
            return UnallocatedJobsGridActionCore(viewModel);
        }
        public ActionResult _UnallocatedJobsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("UnallocatedJobs");
            viewModel.ApplySortingState(column, reset);
            return UnallocatedJobsGridActionCore(viewModel);
        }

        public ActionResult UnallocatedJobsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    AppointmentsCustomBinding.UnallocatedJobsGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AppointmentsCustomBinding.UnallocatedJobsGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_UnallocatedJobsPartial", gridViewModel);
        }

        [ValidateInput(false)]
        public ActionResult ReallocationJobsPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("UnallocatedJobs");

            if (viewModel == null)
                viewModel = AppointmentsCustomBinding.CreateReallocationJobsGridViewModel();

            return ReallocationJobsGridActionCore(viewModel);

        }

        public ActionResult _ReallocationJobsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ReallocationJobs");
            viewModel.Pager.Assign(pager);
            return ReallocationJobsGridActionCore(viewModel);
        }

        public ActionResult _ReallocationJobsFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("ReallocationJobs");
            viewModel.ApplyFilteringState(filteringState);
            return ReallocationJobsGridActionCore(viewModel);
        }
        public ActionResult _ReallocationJobsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("ReallocationJobs");
            viewModel.ApplySortingState(column, reset);
            return ReallocationJobsGridActionCore(viewModel);
        }
        public ActionResult ReallocationJobsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    AppointmentsCustomBinding.ReallocatedJobsGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AppointmentsCustomBinding.ReallocatedJobsGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_ReallocationJobsPartial", gridViewModel);
        }


        public ActionResult AllocatedJobsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    AppointmentsCustomBinding.AllocatedJobsGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AppointmentsCustomBinding.AllocatedJobsGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_AllocatedJobsPartial", gridViewModel);
        }


        [ValidateInput(false)]
        public ActionResult AllocatedJobsPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("AllocatedJobs");

            if (viewModel == null)
                viewModel = AppointmentsCustomBinding.CreateAllocatedJobsGridViewModel();

            return AllocatedJobsGridActionCore(viewModel);
        }

        public ActionResult _AllocatedJobsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("AllocatedJobs");
            viewModel.Pager.Assign(pager);
            return AllocatedJobsGridActionCore(viewModel);
        }

        public ActionResult _AllocatedJobsFiltering(GridViewFilteringState filteringState)

        {
            var viewModel = GridViewExtension.GetViewModel("AllocatedJobs");
            viewModel.ApplyFilteringState(filteringState);
            return AllocatedJobsGridActionCore(viewModel);
        }
        public ActionResult _AllocatedJobsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("AllocatedJobs");
            viewModel.ApplySortingState(column, reset);
            return AllocatedJobsGridActionCore(viewModel);
        }
    }
}
