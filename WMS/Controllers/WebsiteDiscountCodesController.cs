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
    public class WebsiteDiscountCodesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;

        public WebsiteDiscountCodesController(ICoreOrderService orderService, IPropertyService propertyService, ITenantWebsiteService tenantWebsiteService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: ProductsWebsitesMaps
        public ActionResult Index(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            return View();
        }

        public ActionResult _WebsiteDiscountCodesList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var model = _tenantWebsiteService.GetAllValidWebsiteDiscountCodes(CurrentTenantId, SiteId).ToList();
            return PartialView(model);
        }


        public ActionResult _WebsiteDiscountCodesProductList(int siteId, int? DiscountId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebsiteDiscountProductCodesGridView");
            ViewBag.Controller = "WebsiteDiscountCodes";
            if (DiscountId.HasValue)
            {
                var selectedIds = _tenantWebsiteService.GetAllValidWebsiteDiscountProductsMap(CurrentTenantId, (DiscountId ?? 0)).Select(u => u.ProductsWebsitesMapId).ToList();
                ViewData["DiscountProduct"] = selectedIds;
            }
            ViewBag.SiteId = siteId;
            if (viewModel == null)
                viewModel = WebsiteProductsCustomBinding.CreateWebsiteDiscountCodeProductsViewModel();

            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListPaging(GridViewPagerState pager, int siteId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebsiteDiscountProductCodesGridView");
            ViewBag.Controller = "WebsiteDiscountCodes";
            viewModel.Pager.Assign(pager);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListFiltering(GridViewFilteringState filteringState, int siteId)
        {
            ViewBag.Controller = "WebsiteDiscountCodes";
            var viewModel = GridViewExtension.GetViewModel("WebsiteDiscountProductCodesGridView");
            viewModel.ApplyFilteringState(filteringState);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }

        public ActionResult _WebsiteProductsListSorting(GridViewColumnState column, bool reset, int siteId)
        {
            ViewBag.Controller = "WebsiteDiscountCodes";
            var viewModel = GridViewExtension.GetViewModel("WebsiteDiscountProductCodesGridView");
            viewModel.ApplySortingState(column, reset);
            return WebsiteProductsListActionCore(viewModel, siteId);
        }


        public ActionResult WebsiteProductsListActionCore(GridViewModel gridViewModel, int siteId)
        {
            ViewBag.Controller = "WebsiteDiscountCodes";
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
            return PartialView("_WebsiteDiscountCodesProductList", gridViewModel);
        }
        // GET: WebsiteNavigations/Create
        public ActionResult Create(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.siteid = SiteId;
            SiteName(SiteId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteDiscountCodes websiteDiscountCodes, string ProductsWithIds)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (websiteDiscountCodes != null)
            {
                websiteDiscountCodes.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateWebsiteDiscountCodes(websiteDiscountCodes, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = websiteNav.SiteID });

            }
            SiteName(websiteDiscountCodes.SiteID);
            ViewBag.siteid = websiteDiscountCodes.SiteID;

            return View(websiteDiscountCodes);
        }
        // GET: WebsiteNavigations/Create
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var data = _tenantWebsiteService.GetWebsiteDiscountCodesById(id ?? 0);
            SiteName(data.SiteID);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WebsiteDiscountCodes websiteDiscountCodes, string ProductsWithIds)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (websiteDiscountCodes != null)
            {
                websiteDiscountCodes.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateWebsiteDiscountCodes(websiteDiscountCodes, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = websiteDiscountCodes.SiteID });

            }
            SiteName(websiteDiscountCodes.SiteID);
            return View(websiteDiscountCodes);
        }

        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var websiteNav = _tenantWebsiteService.RemoveWebsiteDiscountCodes(id ?? 0, CurrentUserId);
            return RedirectToAction("Index", new { SiteId = websiteNav.SiteID });

        }

    }
}
