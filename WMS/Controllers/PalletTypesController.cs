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
    public class PalletTypesController : BaseController
    {

        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;

        public PalletTypesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }

        // GET: PalletTypes
        public ActionResult Index()
        {
            if(!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        [ValidateInput(false)]
        public ActionResult PalletTypeList()
        {
            var model = LookupServices.GetAllValidPalletTypes(CurrentTenantId).ToList();
            return PartialView("_PalletTypesList", model);
        }
        

        // GET: PalletTypes/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description")] PalletType palletType)
        {
            if (ModelState.IsValid)
            {
                _productLookupService.CreatePalletType(palletType, CurrentTenantId, CurrentUserId);
                return RedirectToAction("Index");
            }

            return View(palletType);
        }

        // GET: PalletTypes/Edit/5
        public ActionResult Edit(int? id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PalletType palletType = _productLookupService.GetPalletTypeById(id??0);
            if (palletType == null)
            {
                return HttpNotFound();
            }
            return View(palletType);
        }

        // POST: PalletTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PalletTypeId,Description")] PalletType palletType)
        {
            if (ModelState.IsValid)
            {
                _productLookupService.UpdatePalletType(palletType,CurrentUserId);
                return RedirectToAction("Index");
            }
            return View(palletType);
        }

        // GET: PalletTypes/Delete/5
        public ActionResult Delete(int? id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PalletType palletType = _productLookupService.GetPalletTypeById(id??0);
            if (palletType == null)
            {
                return HttpNotFound();
            }
            return View(palletType);
        }

        // POST: PalletTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productLookupService.DeletePalletType(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        public JsonResult IsPalletTypeAvailable(string Description, int PalletTypeId = 0)
        {
            if (!String.IsNullOrEmpty(Description)) Description = Description.Trim();

            var description = _productLookupService.GetPalletTypeByName(Description);

            return Json((description == null || description.PalletTypeId == PalletTypeId), JsonRequestBehavior.AllowGet);
        }

    }
}
