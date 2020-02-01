using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class MarketRoutesController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IMarketServices _marketServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public MarketRoutesController(ICoreOrderService orderService,
            IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMarketServices marketServices, IEmployeeServices employeeServices, IMapper mapper) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _marketServices = marketServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
        }

        // GET: MarketRoutes
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var results = _marketServices.GetAllValidMarketRoutes(CurrentTenantId);
            return View(results);
        }


        public ActionResult MarketRoutesListPartial()
        {
            var results = _marketServices.GetAllValidMarketRoutes(CurrentTenantId);
            return PartialView("_GridPartial", results);
        }

        // GET: MarketRoutes/Create
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new MarketRouteViewModel();
            if (id.HasValue && id > 0)
            {
                model = _marketServices.GetMarketRouteById(id.Value);
            }
            return View("_CreateEdit", model);
        }

        [HttpPost]
        public ActionResult SaveMarketRoute(MarketRouteViewModel model)
        {
            model.TenantId = CurrentTenantId;
            var marketRoute = _mapper.Map(model, new MarketRoute());
            _marketServices.SaveMarketRoute(marketRoute, CurrentUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _marketServices.DeleteMarketRoute(id, CurrentUserId);

                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.InnerException });
            }
        }

        public ActionResult SearchAvailable(int id)
        {
            return PartialView("_SearchAvailable", id);
        }

        [HttpPost]
        public ActionResult SearchAvailable(int id, string query)
        {
            var model = new RouteMarketsViewModel() { RouteId = id };

            if (query != null)
            {
                model = _marketServices.GetRouteMarketsById(id, CurrentTenantId, query);
            }

            return PartialView("_SearchAvailableResult", model);
        }

        public ActionResult RouteMarkets(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var model = new RouteMarketsViewModel() { RouteId = id };


            if (id > 0)
            {
                ViewBag.RouteName = _marketServices.GetMarketRouteById(id).Name;
                model = _marketServices.GetRouteMarketsById(id, CurrentTenantId, null);
                model.RouteMarketsEntries = Newtonsoft.Json.JsonConvert.SerializeObject(model.SelectedMarkets);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveRouteMarkets(RouteMarketsViewModel model)
        {
            model.MarketRouteAccounts = (List<SelectedMarketViewModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(model.RouteMarketsEntries, typeof(List<SelectedMarketViewModel>));

            _marketServices.SaveRouteMarkets(model, CurrentUserId);

            return RedirectToAction("RouteMarkets", new { id = model.RouteId });
        }


        public ActionResult MarketRouteProgress()
        {
            var marketroutes = _marketServices.GetAllValidMarketRoutes(CurrentTenantId).MarketRouteViewModel;

            ViewBag.marketRoute = new SelectList(marketroutes, "Id", "Name");
            Guid g = new Guid();
            var test = g.ToString();

            return View();

        }
        [ValidateInput(false)]
        public ActionResult _MarketRouteProgressGridView()
        {
            var viewModel = GridViewExtension.GetViewModel("MarketRouteProgressGridView");

            if (viewModel == null)
                viewModel = MarketRouteProgressCustomBinding.MarketRouteProgressGridViewModel();

            return _MarketRouteProgressGridActionCore(viewModel);
        }
        public ActionResult _MarketRouteProgressGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    MarketRouteProgressCustomBinding.MarketRouteProgressGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        MarketRouteProgressCustomBinding.MarketRouteProgressGetData(args, CurrentTenantId);
                    })
            );
            return PartialView("_MarketRouteProgressGridPartial", gridViewModel);
        }

        public ActionResult _MarketRouteProgressPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("MarketRouteProgressGridView");
            viewModel.Pager.Assign(pager);
            return _MarketRouteProgressGridActionCore(viewModel);
        }

        public ActionResult _MarketRouteProgressFiltering(GridViewFilteringState filteringState)

        {
            var viewModel = GridViewExtension.GetViewModel("MarketRouteProgressGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _MarketRouteProgressGridActionCore(viewModel);
        }
        public ActionResult _MarketRouteProgressDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("MarketRouteProgressGridView");
            viewModel.ApplySortingState(column, reset);
            return _MarketRouteProgressGridActionCore(viewModel);
        }






    }
}
