using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class TrucksController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        // GET: WebsiteNavigations

        public TrucksController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, IProductLookupService productLookupService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
        }

        // GET: WebsiteSliders
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult _TruckList()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var productTags = _productLookupService.GetAllTrucks(CurrentTenantId).ToList();
            return PartialView(productTags);
        }

        // GET: WebsiteSliders/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        // POST: WebsiteSliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MarketVehicle productTag)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                var slider = _productLookupService.CreateOrUpdateTruck(productTag, CurrentUserId, CurrentTenantId);

                return RedirectToAction("Index");
            }
            return View(productTag);
        }

        // GET: WebsiteSliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MarketVehicle productTag = _productLookupService.GetAllTrucks(CurrentTenantId).FirstOrDefault(u => u.Id == id);
            if (productTag == null)
            {
                return HttpNotFound();
            }
            ViewBag.Files = new List<string>();
          
            return View(productTag);
        }

        // POST: WebsiteSliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MarketVehicle productTag)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                var slider = _productLookupService.CreateOrUpdateTruck(productTag, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            return View(productTag);
        }

        // GET: WebsiteSliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = _productLookupService.RemoveTruck((id ?? 0), CurrentUserId);
            return RedirectToAction("Index");
        }
    }
}
