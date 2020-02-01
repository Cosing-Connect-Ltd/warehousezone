using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Net;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ResourceRequestsController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ResourceRequestsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, 
            IEmployeeServices employeeServices, IUserService userService, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeServices = employeeServices;
            _userService = userService;
            _mapper = mapper;
        }
        // GET: ResourceRequests
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = new ResourceRequestsViewModel();
            return View(result);
        }

        // GET: ResourceRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var request = _employeeServices.GetResourceHolidayRequestById(id ?? 0);
            var resourceRequests = _mapper.Map(request, new ResourceRequestsViewModel());
            if (resourceRequests == null)
            {
                return HttpNotFound();
            }

            resourceRequests.ResourceName = request.Resources.Name;
            return View(resourceRequests);
        }

        // GET: ResourceRequests/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            ViewBag.ResourceId = new SelectList(_employeeServices.GetAllEmployees(CurrentTenantId), "ResourceId", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ResourceId,StartDate,EndDate,HolidayReason,RequestedDate,Label,Location,AllDay,EventType,RecurrenceInfo,ReminderInfo,Status,AcceptedBy,ActionReason,RequestType,RequestStatus,Notes,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] ResourceRequestsViewModel resourceRequests)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                _employeeServices.AddResourceHolidayRequest(resourceRequests, CurrentTenantId, CurrentUserId);
                return RedirectToAction("Index");
            }

            ViewBag.ResourceId = new SelectList(_employeeServices.GetAllEmployees(CurrentTenantId), "ResourceId", "FirstName", resourceRequests.ResourceId);
            return View(resourceRequests);
        }

        // GET: ResourceRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var resourceRequests = _mapper.Map(_employeeServices.GetResourceHolidayRequestById(id ?? 0), new ResourceRequestsViewModel());
            if (resourceRequests == null)
            {
                return HttpNotFound();
            }
            ViewBag.ResourceIds = new SelectList(_employeeServices.GetAllEmployees(CurrentTenantId), "ResourceId", "FirstName", resourceRequests.ResourceId);
            return View(resourceRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ResourceId,StartDate,EndDate,HolidayReason,ActionReason,RequestType,RequestStatus,Notes")] ResourceRequestsViewModel resourceRequests)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                _employeeServices.UpdateResourceHolidayRequest(resourceRequests, CurrentUserId);
                return RedirectToAction("Index");
            }
            ViewBag.ResourceIds = new SelectList(_employeeServices.GetAllEmployees(CurrentTenantId), "ResourceId", "FirstName", resourceRequests.ResourceId);
            return View(resourceRequests);
        }

        // GET: ResourceRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var resourceRequests = _mapper.Map(_employeeServices.GetResourceHolidayRequestById(id ?? 0), new ResourceRequestsViewModel());
            if (resourceRequests == null)
            {
                return HttpNotFound();
            }
            return View(resourceRequests);
        }

        // POST: ResourceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _employeeServices.DeleteResourceHolidayRequestById(id, CurrentUserId);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Approve(ApproveRequestViewModel model)
        {
            var result = _employeeServices.UpdateResourceHolidayRequestStatus(model, CurrentUserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult WarningBeforeApprove(ApproveRequestViewModel model)
        {
            if (model.ResourceHolidayRequestId != 0)
            {
                var result = _employeeServices.CountResourceHolidayRequestStatus(model, CurrentUserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult ResourceRequestsAwaitingGridViewPartial()
        {


            var viewModel = GridViewExtension.GetViewModel("ResourceRequestsAwaitingGridView");

            if (viewModel == null)
                viewModel = ResourceCustomBinding.CreateUnallocatedJobsGridViewModel();

            return ResourceRequestsAwaitingGridViewsJobsGridActionCore(viewModel);

        }
        public ActionResult ResourceRequestsAwaitingGridViewsJobsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ResourceCustomBinding.ResourceRequestsGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ResourceCustomBinding.ResourceRequestsGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_ResourceRequestsGridViewPartial", gridViewModel);
        }

        public ActionResult _ResourceRequestsAwaitingGridViewsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ResourceRequestsAwaitingGridView");
            viewModel.Pager.Assign(pager);
            return ResourceRequestsAwaitingGridViewsJobsGridActionCore(viewModel);
        }

        public ActionResult _ResourceRequestsAwaitingGridViewFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("ResourceRequestsAwaitingGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ResourceRequestsAwaitingGridViewsJobsGridActionCore(viewModel);
        }
        public ActionResult _ResourceRequestsAwaitingGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("ResourceRequestsAwaitingGridView");
            viewModel.ApplySortingState(column, reset);
            return ResourceRequestsAwaitingGridViewsJobsGridActionCore(viewModel);
        }

        public ActionResult SchedulerPartial()
        {

            return PartialView("_SchedulerPartial", ResourceRequestsSchedulerSettings.DataObject);
        }
        public ActionResult SchedulerPartialEditAppointment()
        {

            try
            {
                ResourceRequestsSchedulerSettings.UpdateEditableDataObject();
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            return PartialView("_SchedulerPartial", ResourceRequestsSchedulerSettings.DataObject);
        }
    }
}
