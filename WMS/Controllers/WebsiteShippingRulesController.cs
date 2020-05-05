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
    public class WebsiteShippingRulesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        // GET: WebsiteNavigations

        public WebsiteShippingRulesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: WebsiteShippingRules
        public ActionResult Index(int SiteId)
        {
            ViewBag.SiteId = SiteId;
           
            return View();
        }

        public ActionResult _WebsiteShippingRulesList(int SiteId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebShippingRuleGridView");
            ViewBag.SiteId = SiteId;

            if (viewModel == null)
                viewModel = WebsiteShippingRuleCustomBinding.CreateWebsiteShippingRulesViewModel();

            return _WebsiteShippingRulesListActionCore(viewModel, SiteId);
        }
        public ActionResult _WebsiteShippingRuleProductListPaging(GridViewPagerState pager, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("WebShippingRuleGridView");
            viewModel.Pager.Assign(pager);
            return _WebsiteShippingRulesListActionCore(viewModel, SiteId);
        }

        public ActionResult _WebsiteShippingRuleProductListFiltering(GridViewFilteringState filteringState, int SiteId)
        {
            ViewBag.SiteId = SiteId;;
            var viewModel = GridViewExtension.GetViewModel("WebShippingRuleGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _WebsiteShippingRulesListActionCore(viewModel, SiteId);
        }


        public ActionResult _WebsiteShippingRuleProductListSorting(GridViewColumnState column, int SiteId, bool reset)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("WebShippingRuleGridView");
            viewModel.ApplySortingState(column, reset);
            return _WebsiteShippingRulesListActionCore(viewModel, SiteId);
        }


        public ActionResult _WebsiteShippingRulesListActionCore(GridViewModel gridViewModel, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteShippingRuleCustomBinding.WebsiteShippingRulesDataRowCount(args, CurrentTenantId, SiteId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteShippingRuleCustomBinding.WebsiteShippingRulesGetData(args, CurrentTenantId, SiteId);
                    })
            );
            return PartialView("_WebsiteShippingRulesList", gridViewModel);
        }
        // GET: WebsiteShippingRules/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            ViewBag.SiteID = SiteId;
            return View();
        }

        // POST: WebsiteShippingRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( WebsiteShippingRules websiteShippingRules)
        {
            if (ModelState.IsValid)
            {
                var shippingRules=_tenantWebsiteService.CreateOrUpdateWebsiteShippingRules(websiteShippingRules, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId= shippingRules.SiteID });
            }

            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", websiteShippingRules.CountryId);
            return View(websiteShippingRules);
        }

        // GET: WebsiteShippingRules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteShippingRules websiteShippingRules = _tenantWebsiteService.GetWebsiteShippingRulesById(id??0);
            if (websiteShippingRules == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", websiteShippingRules.CountryId);
            return View(websiteShippingRules);
        }

        // POST: WebsiteShippingRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( WebsiteShippingRules websiteShippingRules)
        {
            if (ModelState.IsValid)
            {
                var shippingRules = _tenantWebsiteService.CreateOrUpdateWebsiteShippingRules(websiteShippingRules, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = shippingRules.SiteID });
            }
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", websiteShippingRules.CountryId);
            return View(websiteShippingRules);
        }

        // GET: WebsiteShippingRules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            var shippingRules = _tenantWebsiteService.RemoveWebsiteShippingRules(id??0, CurrentUserId);
            return RedirectToAction("Index", new { SiteId = shippingRules.SiteID });
        }

       
    }
}
