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
    public class WebsiteVouchersController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        // GET: WebsiteNavigations

        public WebsiteVouchersController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
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
            SiteName(SiteId);
            return View();
        }

        public ActionResult _WebsiteVouchersList(int SiteId)
        {
            var viewModel = GridViewExtension.GetViewModel("WebsiteVouchersListGridView");
            ViewBag.SiteId = SiteId;

            if (viewModel == null)
                viewModel = WebsiteVouchersCustomBinding.CreateWebsiteVouchersViewModel();

            return _WebsiteVouchersListActionCore(viewModel, SiteId);
        }
        public ActionResult _WebsiteVouchersListPaging(GridViewPagerState pager, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("WebsiteVouchersListGridView");
            viewModel.Pager.Assign(pager);
            return _WebsiteVouchersListActionCore(viewModel, SiteId);
        }

        public ActionResult _WebsiteVouchersListFiltering(GridViewFilteringState filteringState, int SiteId)
        {
            ViewBag.SiteId = SiteId;;
            var viewModel = GridViewExtension.GetViewModel("WebsiteVouchersListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _WebsiteVouchersListActionCore(viewModel, SiteId);
        }


        public ActionResult _WebsiteVouchersListSorting(GridViewColumnState column, int SiteId, bool reset)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("WebsiteVouchersListGridView");
            viewModel.ApplySortingState(column, reset);
            return _WebsiteVouchersListActionCore(viewModel, SiteId);
        }


        public ActionResult _WebsiteVouchersListActionCore(GridViewModel gridViewModel, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteVouchersCustomBinding.WebsiteVouchersDataRowCount(args, CurrentTenantId, SiteId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteVouchersCustomBinding.WebsiteVouchersGetData(args, CurrentTenantId, SiteId);
                    })
            );
            return PartialView("_WebsiteVouchersList", gridViewModel);
        }
        // GET: WebsiteShippingRules/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.UserId = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName");
            WebsiteVouchers vouchers = new WebsiteVouchers();
            vouchers.Code = _tenantWebsiteService.GenerateVoucherCode();
            ViewBag.SiteID = SiteId;
            SiteName(SiteId);
            return View(vouchers);
        }

        // POST: WebsiteShippingRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( WebsiteVouchers websiteVouchers)
        {
            if (ModelState.IsValid)
            {
                var shippingRules=_tenantWebsiteService.CreateOrUpdateWebsiteVouchers(websiteVouchers, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId= shippingRules.SiteID });
            }
            SiteName(websiteVouchers.SiteID);
            ViewBag.UserId = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName", websiteVouchers.UserId);
            return View(websiteVouchers);
        }

        // GET: WebsiteShippingRules/Edit/5
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteVouchers websiteVouchers = _tenantWebsiteService.GetWebsiteVoucherById(id);
            if (websiteVouchers == null)
            {
                return HttpNotFound();
            }
            SiteName(websiteVouchers.SiteID);
            ViewBag.UserId = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName", websiteVouchers.UserId);
            return View(websiteVouchers);
        }

        // POST: WebsiteShippingRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( WebsiteVouchers websiteVouchers)
        {
            if (ModelState.IsValid)
            {
                var shippingRules = _tenantWebsiteService.CreateOrUpdateWebsiteVouchers(websiteVouchers, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = shippingRules.SiteID });
            }
            SiteName(websiteVouchers.SiteID);
            ViewBag.UserId = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName", websiteVouchers.UserId);
            return View(websiteVouchers);
        }

        // GET: WebsiteShippingRules/Delete/5
        public ActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            var shippingRules = _tenantWebsiteService.RemoveWebsiteVoucher(id, CurrentUserId);
            return RedirectToAction("Index", new { SiteId = shippingRules.SiteID });
        }

       
    }
}
