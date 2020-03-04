using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductGroupController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;

        string UploadDirectory = "~/UploadedFiles/ProductGroups/";
        string UploadTempDirectory = "~/UploadedFiles/ProductGroups/TempFiles/";
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
            ViewBag.ControllerName = "ProductGroup";
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductGroup,IconPath,DepartmentId")] ProductGroups productgroups, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {

            if (ModelState.IsValid)
            {
                string filePath = "";
                var files = UploadControl;
                var filesName = Session["files"] as List<string>;
                var productGroup = _productLookupService.CreateProductGroup(productgroups, CurrentUserId, CurrentTenantId);
                foreach (var file in files)
                {
                    filePath = MoveFile(file,filesName.FirstOrDefault(), productGroup.ProductGroupId);
                    productGroup.IconPath = filePath;
                    _productLookupService.UpdateProductGroup(productGroup, CurrentUserId);
                    break;
                }


                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName", productgroups.DepartmentId);
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
            ViewBag.ControllerName = "ProductGroup";
            ProductGroups productgroups = _productLookupService.GetProductGroupById(id.Value);
            ViewBag.Departments = new SelectList(_LookupService.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName", productgroups.DepartmentId);
            if (productgroups == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrEmpty(productgroups.IconPath))
            {
                List<string> files = new List<string>();
                
                DirectoryInfo dInfo = new DirectoryInfo(productgroups.IconPath);
                files.Add(dInfo.Name);
                Session["files"] = files;
                ViewBag.Files = files;
            }

            return View(productgroups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductGroupId,ProductGroup,IconPath,DepartmentId")] ProductGroups productgroups,IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
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

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["files"] == null)
            {
                Session["files"] = new List<string>();
            }
            var files = Session["files"] as List<string>;

            foreach (var file in UploadControl)
            {
                var fileToken = Guid.NewGuid().ToString();
                var ext = new FileInfo(file.FileName).Extension;
                var fileName = fileToken + ext;
                files.Add(file.FileName);
            }
            Session["files"] = files;

            return null;
        }
        private string MoveFile(DevExpress.Web.UploadedFile file,string FileName, int ProductGroupId)
        {
            Session["files"] = null;
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductGroupId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductGroupId.ToString()));
            string resFileName = Server.MapPath(UploadDirectory + ProductGroupId.ToString() + @"/" + FileName);
            file.SaveAs(resFileName);
            return resFileName;
        }


    }
}
