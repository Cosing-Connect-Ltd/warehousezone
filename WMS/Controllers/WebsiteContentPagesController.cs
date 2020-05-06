using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class WebsiteContentPagesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        // GET: WebsiteNavigations

        public WebsiteContentPagesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: WebsiteContentPages
        public ActionResult Index(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            return View();
        }

        public ActionResult _WebsiteContentPagesList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var contentPages = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, SiteId).ToList();
            return PartialView(contentPages);
        }


        // GET: WebsiteContentPages/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var websitecontentPages = new WebsiteContentPages();
            return View(websitecontentPages);
        }

        // POST: WebsiteContentPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteContentPages websiteContentPages)
        {
            if (ModelState.IsValid)
            {
                _tenantWebsiteService.CreateOrUpdateWebsiteContentPages(websiteContentPages, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index", new { SiteId = websiteContentPages.SiteID });
            }

            return View(websiteContentPages);
        }

        // GET: WebsiteContentPages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteContentPages websiteContentPages = _tenantWebsiteService.GetWebsiteContentById(id??0);
            if (websiteContentPages == null)
            {
                return HttpNotFound();
            }
            
            return View(websiteContentPages);
        }

        // POST: WebsiteContentPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( WebsiteContentPages websiteContentPages)
        {
            if (ModelState.IsValid)
            {
                var contentPages=_tenantWebsiteService.CreateOrUpdateWebsiteContentPages(websiteContentPages, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index",new { SiteId= contentPages.SiteID });
            }

            ViewBag.SiteID = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteID", "SiteName", websiteContentPages.SiteID);
            return View(websiteContentPages);
        }

        //GET: WebsiteContentPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contentPages=_tenantWebsiteService.RemoveWebsiteContentPages((id ?? 0), CurrentUserId);

            return RedirectToAction("Index", new { SiteId = contentPages.SiteID });
        }


    }
}
