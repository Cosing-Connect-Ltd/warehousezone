using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class MarketJobController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public MarketJobController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMarketServices marketServices, IEmployeeServices employeeServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult MarketJobsListPartial()
        {
            int id = int.Parse(!string.IsNullOrEmpty(Request.Params["id"]) ? Request.Params["id"] : "0");
            ViewBag.MarketJobStatus = id;
            var viewModel = GridViewExtension.GetViewModel("_GridPartial");

            if (viewModel == null)
                viewModel = MarketListCustomBinding.CreateMarketGridViewModel();

            return _MarketGridViewsJobsGridActionCore(viewModel);

        }
        public ActionResult _MarketGridViewsJobsGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    MarketListCustomBinding.MarketGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        MarketListCustomBinding.MarketGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );

            return PartialView("_GridPartial", gridViewModel);
        }

        public ActionResult _MarketGridViewsPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("MarketJobsGrid");
            viewModel.Pager.Assign(pager);
            return _MarketGridViewsJobsGridActionCore(viewModel);
        }

        public ActionResult _MarketGridViewFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("MarketJobsGrid");
            viewModel.ApplyFilteringState(filteringState);
            return _MarketGridViewsJobsGridActionCore(viewModel);
        }
        public ActionResult _MarketGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("MarketJobsGrid");
            viewModel.ApplySortingState(column, reset);
            return _MarketGridViewsJobsGridActionCore(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = new MarketJobViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketJobById(id.Value);
            }
            model.AllCustomerAccounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).Select(m => new SelectListItem() { Text = m.CompanyName, Value = m.AccountID.ToString() }).ToList();
            model.AllResources = _employeeServices.GetAllEmployees(CurrentTenantId).Select(m => new SelectListItem() { Text = m.Name, Value = m.ResourceId.ToString() }).ToList();
            model.AllResources.Insert(0, new SelectListItem() { Text = "Un-Allocated", Value = "0" });
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveMarketJob(MarketJobViewModel model)
        {
            model.TenantId = CurrentTenantId;

            MarketJobStatusEnum status = MarketJobStatusEnum.UnAllocated;

            if (model.ResourceID != null)
            {
                status = MarketJobStatusEnum.Allocated;
            }

            _marketServices.SaveMarketJob(_mapper.Map(model, new MarketJob()), model.ResourceID, status, CurrentUserId, CurrentTenantId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _marketServices.CancelMarketJob(id, CurrentUserId, CurrentTenantId, "Cancelled by the administrator.");
            return Json(new { success = true });
        }

        public ActionResult _AllocateMarketJobPartial(int id)
        {
            var marketJob = _marketServices.GetMarketJobById(id);

            marketJob.AllResources = _employeeServices.GetAllEmployees(CurrentTenantId).Select(m => new SelectListItem() { Text = m.Name, Value = m.ResourceId.ToString() }).ToList();
            marketJob.AllResources.Insert(0, new SelectListItem() { Text = "Un-Allocated", Value = "0" });
            return PartialView("_AllocateMarketJobPartial", marketJob);
        }

        public ActionResult AllocateMarketJob(MarketJobAllocationModel model)
        {
            var marketJob = new MarketJob() { Id = model.MarketJobId };
            var job = _marketServices.UpdateMarketJobAllocation(marketJob.Id, model.ResourceId, CurrentUserId, CurrentTenantId, model.LatestJobStatusId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _MarketJobStatus()
        {
            var Cancelled = _marketServices.GetAllValidMarketJobs(CurrentTenantId, MarketJobStatusEnum.Cancelled).Count();

            int Declined = _marketServices.GetAllValidMarketJobs(CurrentTenantId, MarketJobStatusEnum.Declined).Count();
            int Completed = _marketServices.GetAllValidMarketJobs(CurrentTenantId, MarketJobStatusEnum.Completed).Count();
            int Unallocated = _marketServices.GetAllValidMarketJobs(CurrentTenantId, MarketJobStatusEnum.UnAllocated).Count();

            ViewBag.Cancelled = Cancelled;
            ViewBag.Declined = Declined;
            ViewBag.Completed = Completed;
            ViewBag.Unallocated = Unallocated;

            return PartialView("_MarketJobStatus");
        }

    }
}