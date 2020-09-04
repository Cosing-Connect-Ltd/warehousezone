using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class ProductKitTypesController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;

        public ProductKitTypesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }

        // GET: ProductKitTypes
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public PartialViewResult _ProductKitTypesList()
        {
            var model = LookupServices.GetAllValidProductKitType(CurrentTenantId).ToList();
            return PartialView(model);
        }

        // GET: ProductKitTypes/Create
        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            ViewBag.ControllerName = "ProductKitType";
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ProductGroupId = new SelectList(_LookupService.GetAllValidProductKitType(CurrentTenantId));
            return View();
        }

        // POST: ProductKitTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,SortOrder")] ProductKitType productKitType)
        {
            ViewBag.ControllerName = "ProductKitTypes";

            if (ModelState.IsValid)
            {
                var productKitTypes = _productLookupService.CreateProductKitType(productKitType, CurrentUserId, CurrentTenantId);

                return RedirectToAction("Index");
            }

            ViewBag.ProductKitTypeId = new SelectList(_LookupService.GetAllValidProductKitType(CurrentTenantId));
            return View(productKitType);
        }

        // GET: ProductKitTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductKitType productKitType = _productLookupService.GetProductKitTypeById(id.Value);
            if (productKitType == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductKitTypeId = new SelectList(_LookupService.GetAllValidProductKitType(CurrentTenantId));
            return View(productKitType);

        }

        // POST: ProductKitTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,SortOrder,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsActive,IsDeleted,TenentId")] ProductKitType productKitType)
        {
            ViewBag.ControllerName = "ProductKitTypes";
            if (ModelState.IsValid)
            {
                _productLookupService.UpdateProductKitType(productKitType, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            ViewBag.ProductKitTypeId = new SelectList(_LookupService.GetAllValidProductKitType(CurrentTenantId));
            return View(productKitType);
        }

        // GET: ProductKitTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductKitType productKitType = _productLookupService.GetProductKitTypeById(id.Value);
            if (productKitType == null)
            {
                return HttpNotFound();
            }
            return View(productKitType);
        }

        // POST: ProductKitTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productLookupService.DeleteProductKitType(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        public JsonResult IsKitTypeAvailable(string ProductKitTypeName, int ProductKitTypeId = 0)
        {
            if (!String.IsNullOrEmpty(ProductKitTypeName)) ProductKitTypeName = ProductKitTypeName.Trim();

            var productKitType = _productLookupService.GetProductKitTypeByName(ProductKitTypeName);

            return Json((productKitType == null || productKitType.Id == ProductKitTypeId), JsonRequestBehavior.AllowGet);
        }
    }
}
