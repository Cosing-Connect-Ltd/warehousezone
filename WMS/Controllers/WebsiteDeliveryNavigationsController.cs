using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class WebsiteDeliveryNavigationsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        // GET: WebsiteNavigations

        public WebsiteDeliveryNavigationsController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }
        // GET: WebsiteDeliveryNavigations
        public ActionResult Index(int siteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = siteId;
            SiteName(siteId);
            return View();
        }

        public ActionResult _WebSiteDeliveryNavigationList(int siteId)
        {
            ViewBag.SiteId = siteId;
            var model = _tenantWebsiteService.GetAllValidWebsiteDeliveryNavigations(CurrentTenantId, siteId);
            return PartialView(model.ToList());
        }

        [HttpGet]
        public ActionResult Create(int siteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = siteId;
            SiteName(siteId);
            return View();
        }

        [HttpPost]
        public ActionResult Create(WebsiteDeliveryNavigation websiteDeliveryNavigation)
        {
            if (ModelState.IsValid)
            {
                var result = _tenantWebsiteService.CreateOrUpdateWebsiteDeliveryNavigation(websiteDeliveryNavigation,
                    websiteDeliveryNavigation.SiteId, CurrentTenantId);
                return RedirectToAction("Index", new { siteId = websiteDeliveryNavigation.SiteId });
            }
            SiteName(websiteDeliveryNavigation.SiteId);
            return View(websiteDeliveryNavigation);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var websiteDeliveryNavigation= _tenantWebsiteService.GetWebsiteDeliveryNavigationById(id);
            SiteName(websiteDeliveryNavigation.SiteId);
            return View(websiteDeliveryNavigation);
        }

        [HttpPost]
        public ActionResult Edit(WebsiteDeliveryNavigation websiteDeliveryNavigation)
        {
            if (ModelState.IsValid)
            {
                var result = _tenantWebsiteService.CreateOrUpdateWebsiteDeliveryNavigation(websiteDeliveryNavigation,
                    websiteDeliveryNavigation.SiteId, CurrentTenantId);
                return RedirectToAction("Index", new { siteId = websiteDeliveryNavigation.SiteId });
            }
            SiteName(websiteDeliveryNavigation.SiteId);
            return View(websiteDeliveryNavigation);
        }
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var siteId = _tenantWebsiteService.RemoveWebsiteDeliveryNavigation((id ?? 0), CurrentUserId);
            return RedirectToAction("Index", new { siteId = siteId });
        }



    }
}