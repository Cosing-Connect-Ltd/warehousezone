using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web.Mvc;
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
            ViewBag.ControllerName = "ProductGroup";
            if (ModelState.IsValid)
            {
                string filePath = "";
                var files = UploadControl;
                var filesName = Session["uploadProductGroup"] as List<string>;
                var productGroup = _productLookupService.CreateProductGroup(productgroups, CurrentUserId, CurrentTenantId);
                if (filesName != null)
                {
                    
                    filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), productGroup.ProductGroupId);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        productGroup.IconPath = filePath;
                        _productLookupService.UpdateProductGroup(productGroup, CurrentUserId);
                    }
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
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(productgroups.IconPath);
                files.Add(dInfo.Name);
                Session["uploadProductGroup"] = files;
                ViewBag.Files = files;
            }


            return View(productgroups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductGroupId,ProductGroup,IconPath,DepartmentId")] ProductGroups productgroups, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            ViewBag.ControllerName = "ProductGroup";
            string filePath = "";
            if (ModelState.IsValid)
            {
                var filesName = Session["uploadProductGroup"] as List<string>;
                if (filesName == null) { productgroups.IconPath = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), productgroups.ProductGroupId);
                        productgroups.IconPath = filePath;
                    }

                }
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
            if (Session["uploadProductGroup"] == null)
            {
                Session["uploadProductGroup"] = new List<string>();
            }
            var files = Session["uploadProductGroup"] as List<string>;

            foreach (var file in UploadControl)
            {
                SaveFile(file);
                files.Add(file.FileName);
            }
            Session["uploadProductGroup"] = files;

            return null;
        }
        private string MoveFile(DevExpress.Web.UploadedFile file, string FileName, int ProductGroupId)
        {
            Session["uploadProductGroup"] = null;
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductGroupId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductGroupId.ToString()));

            string sourceFile = Server.MapPath(UploadTempDirectory + @"/" + FileName);
            string destFile = Server.MapPath(UploadDirectory + ProductGroupId.ToString() + @"/" + FileName);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Move(sourceFile, destFile);
            }
            return (UploadDirectory.Replace("~", "") + ProductGroupId.ToString() + @"/" + FileName);
        }
        private void SaveFile(DevExpress.Web.UploadedFile file)
        {
            if (!Directory.Exists(Server.MapPath(UploadTempDirectory)))
                Directory.CreateDirectory(Server.MapPath(UploadTempDirectory));
            string resFileName = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            file.SaveAs(resFileName);
        }
        protected override void Initialize(RequestContext requestContext)
        {
            var binder = (DevExpressEditorsBinder)ModelBinders.Binders.DefaultBinder;
            var actionName = (string)requestContext.RouteData.Values["Action"];
            switch (actionName)
            {
                case "UploadFile":
                    binder.UploadControlBinderSettings.FileUploadCompleteHandler = (s, e) =>
                    {
                        e.CallbackData = e.UploadedFile.FileName;
                    };
                    break;
            }
            base.Initialize(requestContext);
        }

    }
}
