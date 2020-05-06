using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data.Migrations;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductAllowanceGroupsController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;
        private readonly ITenantWebsiteService _tenantWebsiteService;

        public ProductAllowanceGroupsController(ICoreOrderService orderService, IPropertyService propertyService, ITenantWebsiteService tenantWebsiteService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
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
            return View();
        }

        public ActionResult _ProductAllowanceGroupList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var model = _tenantWebsiteService.GetAllValidProductAllowanceGroups(CurrentTenantId, SiteId).ToList();
            return PartialView(model);
        }


        public ActionResult _ProductAllowanceGroupProductList(int siteId, int? ProductAllownceGroupId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductAllowanceGroupsGridView");
            ViewBag.Controller = "ProductAllowanceGroups";
            if (ProductAllownceGroupId.HasValue)
            {
                //need to check
                var selectedIds = _tenantWebsiteService.GetAllValidProductAllowanceGroupMap(CurrentTenantId, (ProductAllownceGroupId ?? 0)).Select(u => u.ProductwebsiteMapId).ToList();
                ViewData["DiscountProduct"] = selectedIds;
            }
            ViewBag.SiteId = siteId;
            if (viewModel == null)
                viewModel = WebsiteProductsCustomBinding.CreateWebsiteDiscountCodeProductsViewModel();

            return WebsiteProductsListActionCore(viewModel,siteId);
        }

        public ActionResult _WebsiteProductsListPaging(GridViewPagerState pager, int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductAllowanceGroupsGridView");
            ViewBag.Controller = "ProductAllowanceGroups";
            viewModel.Pager.Assign(pager);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListFiltering(GridViewFilteringState filteringState, int siteId)
        {
            ViewBag.Controller = "ProductAllowanceGroups";
            var viewModel = GridViewExtension.GetViewModel("ProductAllowanceGroupsGridView");
            viewModel.ApplyFilteringState(filteringState);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListSorting(GridViewColumnState column, bool reset, int siteId)
        {
            ViewBag.Controller = "ProductAllowanceGroups";
            var viewModel = GridViewExtension.GetViewModel("ProductAllowanceGroupsGridView");
            viewModel.ApplySortingState(column, reset);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }


        public ActionResult WebsiteProductsListActionCore(GridViewModel gridViewModel, int siteId)
        {
            ViewBag.Controller = "ProductAllowanceGroups";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteProductsCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, siteId,true);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteProductsCustomBinding.ProductGetData(args, CurrentTenantId, siteId,true);
                    })
            );
            return PartialView("_ProductAllowanceGroupProductList", gridViewModel);
        }
        // GET: WebsiteNavigations/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.siteid = SiteId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductAllowanceGroup productAllowanceGroup, string ProductsWithIds)
        {
            if (productAllowanceGroup != null)
            {
                productAllowanceGroup.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateProductGroupAllowance(productAllowanceGroup, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = websiteNav.SiteID });

            }
            ViewBag.siteid = productAllowanceGroup.SiteID;

            return View(productAllowanceGroup);
        }
        // GET: WebsiteNavigations/Create
        public ActionResult Edit(int? id)
        {
            var data = _tenantWebsiteService.GetProductAllowanceGroupById(id ?? 0);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductAllowanceGroup productAllowanceGroup, string ProductsWithIds)
        {
            if (productAllowanceGroup != null)
            {
                productAllowanceGroup.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateProductGroupAllowance(productAllowanceGroup, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = websiteNav.SiteID });

            }

            return View(productAllowanceGroup);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var websiteNav = _tenantWebsiteService.RemoveProductAllowanceGroup(id ?? 0, CurrentUserId);
            return RedirectToAction("Index", new { SiteId = websiteNav.SiteID });

        }

    }
}
