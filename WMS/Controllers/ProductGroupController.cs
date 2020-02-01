using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductGroupController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;

        public ProductGroupController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ProductGroupList()
        { 
            var model = LookupServices.GetAllValidProductGroups(CurrentTenantId).ToList();
            return PartialView("_ProductGroupList", model);
        }

        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include= "ProductGroup,IconPath,DepartmentId")] ProductGroups productgroups)
        {
           
            if (ModelState.IsValid)
            {
                _productLookupService.CreateProductGroup(productgroups, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName",productgroups.DepartmentId);
            return View(productgroups);
        }

        // GET: /ProductGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroups productgroups  = _productLookupService.GetProductGroupById(id.Value);
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName", productgroups.DepartmentId);
            if (productgroups == null)
            {
                return HttpNotFound();
            }
            return View(productgroups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include= "ProductGroupId,ProductGroup,IconPath,DepartmentId")] ProductGroups productgroups)
        {
            if (ModelState.IsValid)
            {
                _productLookupService.UpdateProductGroup(productgroups, CurrentUserId);
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName", productgroups.DepartmentId);
            return View(productgroups);
        }

        // GET: /ProductGroup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroups productgroups = _productLookupService.GetProductGroupById(id.Value);
            if (productgroups == null)
            {
                return HttpNotFound();
            }
            return View(productgroups);
        }

        // POST: /ProductGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productLookupService.DeleteProductGroup(id, CurrentUserId);
            return RedirectToAction("Index");
        }
         
        public JsonResult IsProductGroupAvailable(string ProductGroup, int ProductGroupId = 0)
        {
            if (!String.IsNullOrEmpty(ProductGroup)) ProductGroup = ProductGroup.Trim();

            var productGroup = _productLookupService.GetProductGroupByName(ProductGroup);

            return Json((productGroup == null || productGroup.ProductGroupId == ProductGroupId), JsonRequestBehavior.AllowGet);
        }
         
      
    }
}
