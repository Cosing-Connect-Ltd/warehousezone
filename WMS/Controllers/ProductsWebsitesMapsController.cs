using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductsWebsitesMapsController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;
        private readonly ITenantWebsiteService _tenantWebsiteService;

        public ProductsWebsitesMapsController(ICoreOrderService orderService, IPropertyService propertyService, ITenantWebsiteService tenantWebsiteService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: ProductsWebsitesMaps
        public ActionResult Index(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            return PartialView();
        }

        public ActionResult SaveNavigationProductList(int SiteId, MVCxGridViewBatchUpdateValues<NavigationProductsViewModel, int> updateValues)
        {
            List<bool> results = new List<bool>();
            foreach (var value in updateValues.Update)
            {
                value.SiteID = SiteId;
                value.ProductId = value.Id;
                var res = _tenantWebsiteService.CreateOrUpdateWebsiteProducts(value, CurrentUserId, CurrentTenantId);
                results.Add(res);
            }

            return WebsiteProductsList(SiteId);
        }

        public ActionResult WebsiteProductsList(int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebsiteGridView");
            ViewBag.Controller = "ProductsWebsitesMaps";
            ViewBag.SiteId = siteId;
            if (viewModel == null)
                viewModel = WebsiteProductsCustomBinding.CreateWebsiteProductsViewModel();

            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListPaging(GridViewPagerState pager, int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebsiteGridView");
            ViewBag.Controller = "ProductsWebsitesMaps";
            viewModel.Pager.Assign(pager);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListFiltering(GridViewFilteringState filteringState, int siteId)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            var viewModel = GridViewExtension.GetViewModel("ProductWebsiteGridView");
            viewModel.ApplyFilteringState(filteringState);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListSorting(GridViewColumnState column, bool reset, int siteId)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            var viewModel = GridViewExtension.GetViewModel("ProductWebsiteGridView");
            viewModel.ApplySortingState(column, reset);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }


        public ActionResult WebsiteProductsListActionCore(GridViewModel gridViewModel, int siteId)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteProductsCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, siteId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteProductsCustomBinding.ProductGetData(args, CurrentTenantId, siteId);
                    })
            );
            return PartialView("_WebsiteProductsList", gridViewModel);
        }
    }
}
