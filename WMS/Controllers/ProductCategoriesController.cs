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
    public class ProductCategoriesController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;

        public ProductCategoriesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }
        // GET: ProductCategories
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public PartialViewResult _ProductCategoriesList()
        {
            var model = LookupServices.GetAllValidProductCategories(CurrentTenantId).ToList();
            return PartialView(model);
        }

        // GET: ProductCategories/Create
        public ActionResult Create()
        {
            ViewBag.ControllerName = "ProductCategories";
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ProductGroupId = new SelectList(_LookupService.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            return View();
        }

        // POST: ProductCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductCategoryName,SortOrder,ProductGroupId")] ProductCategory productCategory)
        {
            ViewBag.ControllerName = "ProductCategories";
            
            if (ModelState.IsValid)
            {
                
                var productCategories = _productLookupService.CreateProductCategory(productCategory, CurrentUserId, CurrentTenantId);
                
                return RedirectToAction("Index");
            }

            ViewBag.ProductGroupId = new SelectList(_LookupService.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup", productCategory.ProductGroupId);
            return View(productCategory);
        }

        // GET: ProductCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductCategory productCategory = _productLookupService.GetProductCategoryById(id.Value);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductGroupId = new SelectList(_LookupService.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            return View(productCategory);
        }

        // POST: ProductCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductCategoryId,ProductCategoryName,SortOrder,ProductGroupId,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] ProductCategory productCategory)
        {
            ViewBag.ControllerName = "ProductCategory";
            if (ModelState.IsValid)
            {
                _productLookupService.UpdateProductCategory(productCategory, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            ViewBag.ProductGroupId = new SelectList(_LookupService.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            return View(productCategory);
        }

        // GET: ProductCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory productCategory = _productLookupService.GetProductCategoryById(id.Value);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            return View(productCategory);
        }

        // POST: ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productLookupService.DeleteProductCategory(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        public JsonResult IsProductCategoryAvailable(string ProductCategoryName, int ProductCategoryId = 0)
        {
            if (!String.IsNullOrEmpty(ProductCategoryName)) ProductCategoryName = ProductCategoryName.Trim();

            var productCategory = _productLookupService.GetProductCategoryByName(ProductCategoryName);

            return Json((productCategory == null || productCategory.ProductCategoryId == ProductCategoryId), JsonRequestBehavior.AllowGet);
        }
       
    }
}
