using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class WebsiteWarehousesController : BaseController
    {
        private readonly ILookupServices _LookupService;
        private readonly ITenantWebsiteService _tenantWebsiteService;

        public WebsiteWarehousesController(ICoreOrderService orderService, IPropertyService propertyService,  IAccountServices accountServices, ILookupServices lookupServices, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _LookupService = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }


        public ActionResult Index(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            return View();
        }

        public ActionResult SaveWebsiteWarehouseList(int SiteId, MVCxGridViewBatchUpdateValues<WebsiteWarehousesViewModel, int> updateValues)
        {
            List<bool> results = new List<bool>();
            foreach (var value in updateValues.Update)
            {
                value.SiteID = SiteId;
                value.WarehouseId = value.Id;
                var res = _tenantWebsiteService.CreateOrUpdateWebsiteWarehouse(value, CurrentUserId, CurrentTenantId);
                results.Add(res);
            }

            return WebsiteWarehousesList(SiteId);
        }

        public ActionResult WebsiteWarehousesList(int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebsiteWarehousesGridView");
            ViewBag.Controller = "WebsiteWarehouses";
            ViewBag.SiteId = siteId;
            if (viewModel == null)
                viewModel = WebsiteWarehousesCustomBinding.CreateWebsiteWarehousesViewModel();

            return WebsiteWarehousesListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteWarehousesListPaging(GridViewPagerState pager, int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebsiteWarehousesGridView");
            ViewBag.Controller = "WebsiteWarehouses";
            viewModel.Pager.Assign(pager);
            return WebsiteWarehousesListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteWarehousesListFiltering(GridViewFilteringState filteringState, int siteId)
        {
            ViewBag.Controller = "WebsiteWarehouses";
            var viewModel = GridViewExtension.GetViewModel("WebsiteWarehousesGridView");
            viewModel.ApplyFilteringState(filteringState);
            return WebsiteWarehousesListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteWarehousesListSorting(GridViewColumnState column, bool reset, int siteId)
        {
            ViewBag.Controller = "WebsiteWarehouses";
            var viewModel = GridViewExtension.GetViewModel("WebsiteWarehousesGridView");
            viewModel.ApplySortingState(column, reset);
            return WebsiteWarehousesListActionCore(viewModel, siteId);
        }


        public ActionResult WebsiteWarehousesListActionCore(GridViewModel gridViewModel, int siteId)
        {
            ViewBag.Controller = "WebsiteWarehouses";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteWarehousesCustomBinding.WarehouseGetDataRowCount(args, CurrentTenantId, siteId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteWarehousesCustomBinding.WarehouseGetData(args, CurrentTenantId, siteId);
                    })
            );
            return PartialView("_WebsiteWarehousesList", gridViewModel);
        }
    }
}
