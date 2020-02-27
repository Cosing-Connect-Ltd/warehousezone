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
    public class MarketsController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public MarketsController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMarketServices marketServices, IEmployeeServices employeeServices, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }
        // GET: Appointments
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var results = _marketServices.GetAllValidMarkets(CurrentTenantId);
            return View(results);
        }

        public ActionResult MarketsListPartial()
        {
            var results = _marketServices.GetAllValidMarkets(CurrentTenantId);
            return PartialView("_GridPartial", results);
        }

        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new MarketViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketById(id.Value);
            }
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveMarket(MarketViewModel model)
        {
            model.TenantId = CurrentTenantId;
            var market = _mapper.Map(model, new Market());
            _marketServices.SaveMarket(market, CurrentUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _marketServices.DeleteMarket(id, CurrentUserId);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.InnerException });
            }
        }

        #region Routes

        public ActionResult MarketCustomers(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new MarketCustomersViewModel() { MarketId = id };


            if (id > 0)
            {
                ViewBag.MarketName = _marketServices.GetMarketById(id).Name;
                model = _marketServices.GetMarketCustomersById(id, CurrentTenantId, null);
                model.MarketCustomerEntries = Newtonsoft.Json.JsonConvert.SerializeObject(model.SelectedCustomers);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveMarketCustomers(MarketCustomersViewModel model)
        {
            model.MarketCustomerAccounts = (List<SelectedAccountViewModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.MarketCustomerEntries, typeof(List<SelectedAccountViewModel>));

            _marketServices.SaveMarketCustomer(model, CurrentUserId,CurrentTenantId);

            return RedirectToAction("MarketCustomers", new { id = model.MarketId });
        }


        public ActionResult SearchAvailable(int id)
        {
            return PartialView("_SearchAvailable", id);
        }

        [HttpPost]
        public ActionResult SearchAvailable(int id, string query)
        {
            var model = new MarketCustomersViewModel() { MarketId = id };

            if (query != null)
            {
                model = _marketServices.GetMarketCustomersById(id, CurrentTenantId, query);
            }

            return PartialView("_SearchAvailableResult", model);
        }
        #endregion

        #region Vehicles
        public ActionResult Vehicles()
        {
            var results = _marketServices.GetAllValidMarketVehicles(CurrentTenantId);

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            
            return View("~/Views/MarketsVehicle/Index.cshtml", results);
        }

        public ActionResult MarketsVehicleListPartial()
        {
            var result = _marketServices.GetAllValidMarketVehicles(CurrentTenantId);
            return PartialView("~/Views/MarketsVehicle/_GridPartial.cshtml", result);
        }

        public ActionResult VehiclesEdit(int? id)
        {
            var model = new MarketVehicleViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketVehicleById(id.Value);
            }
            return View("~/Views/MarketsVehicle/_CreateEdit.cshtml", model);
        }
        [HttpPost]
        public ActionResult SaveVehicle(MarketVehicleViewModel model)
        {
            model.TenantId = CurrentTenantId;
            _marketServices.SaveMarketVehicle(_mapper.Map(model, new MarketVehicle()), CurrentUserId);
            return RedirectToAction("Vehicles");
        }
        [HttpPost]
        public JsonResult DeleteVehicle(int id)
        {
            _marketServices.DeleteMarketVehicle(id, CurrentUserId);
            return Json(new { success = true });
        }
        #endregion

        #region Market Jobs
        public ActionResult MarketJobs()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View("~/Views/MarketJob/Index.cshtml");
        }

        //public ActionResult MarketJobsListPartial(int id)
        //{
        //    ViewBag.MarketJobStatus = id;
        //    var results = _marketServices.GetAllValidMarketJobs(CurrentTenantId, (MarketJobStatusEnum)id);
        //    return PartialView("~/Views/MarketJob/_GridPartial.cshtml", results);
        //}

        
        public ActionResult MarketJobsListPartial()
        {
            int id= int.Parse(!string.IsNullOrEmpty(Request.Params["id"]) ? Request.Params["id"] : "0");
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
            return PartialView("~/Views/MarketJob/_GridPartial.cshtml", gridViewModel);
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

        public ActionResult MarketJobEdit(int? id)
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
            return View("~/Views/MarketJob/_CreateEdit.cshtml", model);
        }
        [HttpPost]
        public ActionResult SaveMarketJob(MarketJobViewModel model)
        {
            model.TenantId = CurrentTenantId;

            int status = (int)MarketJobStatusEnum.UnAllocated;

            if (model.ResourceID != null)
            {
                status = (int)MarketJobStatusEnum.Allocated;
            }

            _marketServices.SaveMarketJob(_mapper.Map(model, new MarketJob()), model.ResourceID, status, CurrentUserId, CurrentTenantId);
            return RedirectToAction("MarketJobs");
        }
        [HttpPost]
        public JsonResult DeleteMarketJob(int id)
        {
            _marketServices.CancelMarketJob(id, CurrentUserId, CurrentTenantId, "Cancelled by the administrator.");
            return Json(new { success = true });
        }

        public ActionResult _AllocateMarketJobPartial(int id)
        {
            var marketJob = _marketServices.GetMarketJobById(id);
           
            marketJob.AllResources = _employeeServices.GetAllEmployees(CurrentTenantId).Select(m => new SelectListItem() { Text = m.Name, Value = m.ResourceId.ToString() }).ToList();
            marketJob.AllResources.Insert(0, new SelectListItem() { Text = "Un-Allocated", Value = "0" });
            return PartialView("~/Views/MarketJob/_AllocateMarketJobPartial.cshtml", marketJob);
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

            return PartialView("~/Views/MarketJob/_MarketJobStatus.cshtml");
        }

        #endregion

        #region Market Stock Levels

        public ActionResult EditStockLevels(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var market = _marketServices.GetMarketById(id);
            ViewBag.MarketName = market.Name;
            ViewBag.MarketId = market.Id;
            return View();
        }

        public ActionResult _StockLevelsPartial(int id)
        {
            var model = _marketServices.GetAllStockLevelsForMarket(id);
            return PartialView("_StockLevelsPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult UpdateProductLevels(MVCxGridViewBatchUpdateValues<MarketProductLevelViewModel, int> updateValues)
        {
            int marketId = Convert.ToInt32(Request.Params["MarketId"]);
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    _marketServices.UpdateProductLevelsForMarkets(marketId, product.ProductID,
                       product.MinStockQuantity, CurrentUserId);
            }
            return _StockLevelsPartial(marketId);
        }

        #endregion

    }
}