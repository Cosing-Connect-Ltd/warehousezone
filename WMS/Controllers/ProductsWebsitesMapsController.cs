using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductsWebsitesMapsController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        string UploadDirectory = "~/UploadedFiles/ProductsWebsitesMaps/";
        string UploadTempDirectory = "~/UploadedFiles/ProductsWebsitesMaps/TempFiles/";

        public ProductsWebsitesMapsController(ICoreOrderService orderService, IPropertyService propertyService,ITenantWebsiteService tenantWebsiteService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }


        // GET: ProductsWebsitesMaps
        public ActionResult Index(int SiteId)
        {
            ViewBag.siteid = SiteId;
            return PartialView();
        }
        public ActionResult ProductList()
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            ViewBag.Controller = "ProductsWebsitesMaps";
            if (viewModel == null)
                viewModel = ProducctListCustomBinding.CreateProductGridViewModel();

           
            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            ViewBag.Controller = "ProductsWebsitesMaps";
            viewModel.Pager.Assign(pager);
            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductsFiltering(GridViewFilteringState filteringState)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ProductGridActionCore(viewModel);
        }


        public ActionResult _ProductsGetDataSorting(GridViewColumnState column, bool reset)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            viewModel.ApplySortingState(column, reset);
            return ProductGridActionCore(viewModel);
        }


        public ActionResult ProductGridActionCore(GridViewModel gridViewModel)
        {
            ViewBag.Controller = "ProductsWebsitesMaps";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProducctListCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProducctListCustomBinding.ProductGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_AllProductList", gridViewModel);
        }

        public JsonResult Create(int siteId,string ProductIds)
        {
            if (!string.IsNullOrEmpty(ProductIds))
            {
                List<int> ProductList = ProductIds.Split(',').Select(Int32.Parse).ToList();
                var productsWebsitesMaps = _tenantWebsiteService.CreateOrUpdateProductsWebsitesMap(ProductList,siteId, CurrentUserId, CurrentTenantId);
                return Json(productsWebsitesMaps);
            }
            return Json(false);
            
          
            
        }

      
    }
}
