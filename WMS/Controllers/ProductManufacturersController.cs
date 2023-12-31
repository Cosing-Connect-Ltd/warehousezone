﻿using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS.Controllers
{
    public class ProductManufacturersController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;
        private string UploadDirectory = "~/UploadedFiles/ProductManufacturers/";
        private string UploadTempDirectory = "~/UploadedFiles/ProductManufacturers/TempFiles/";

        public ProductManufacturersController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }

        // GET: ProductManufacturers
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ProductManufacturersList()
        {
            var model = _LookupService.GetAllValidProductManufacturer(CurrentTenantId).ToList();
            return PartialView("_ProductManufacturerList", model);
        }

        // GET: ProductManufacturers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductManufacturer productManufacturer = _LookupService.GetAllValidProductManufacturer(CurrentTenantId).FirstOrDefault(u => u.Id == id);
            if (productManufacturer == null)
            {
                return HttpNotFound();
            }
            return View(productManufacturer);
        }

        // GET: ProductManufacturers/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "ProductManufacturers";
            return View();
        }

        // POST: ProductManufacturers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductManufacturer productManufacturer, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            ViewBag.ControllerName = "ProductManufacturers";
            var filesName = Session["UploadProductManufacturerImage"] as List<string>;
            string filePath = "";
            if (ModelState.IsValid)
            {
                productManufacturer.TenantId = CurrentTenantId;
                var manufacturer = _LookupService.SaveAndUpdateProductManufacturer(productManufacturer, CurrentUserId);
                if (filesName != null)
                {
                    filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), manufacturer.Id);
                    manufacturer.ImagePath = filePath;
                    manufacturer.TenantId = CurrentTenantId;
                    _LookupService.SaveAndUpdateProductManufacturer(manufacturer, CurrentUserId);
                }
                return RedirectToAction("Index");
            }

            return View(productManufacturer);
        }

        // GET: ProductManufacturers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "ProductManufacturers";
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductManufacturer productManufacturer = _LookupService.GetAllValidProductManufacturer(CurrentTenantId, id).FirstOrDefault();
            if (productManufacturer == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrEmpty(productManufacturer.ImagePath))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(productManufacturer.ImagePath);
                files.Add(dInfo.Name);
                Session["UploadProductManufacturerImage"] = files;
                ViewBag.Files = files;
            }
            return View(productManufacturer);
        }

        // POST: ProductManufacturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductManufacturer productManufacturer, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            ViewBag.ControllerName = "ProductManufacturers";
            string filePath = "";
            if (ModelState.IsValid)
            {
                var filesName = Session["UploadProductManufacturerImage"] as List<string>;
                if (filesName == null) { productManufacturer.ImagePath = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), productManufacturer.Id);
                        productManufacturer.ImagePath = filePath;
                    }
                }
                productManufacturer.TenantId = CurrentTenantId;
                _LookupService.SaveAndUpdateProductManufacturer(productManufacturer, CurrentUserId);
                return RedirectToAction("Index");
            }
            return View(productManufacturer);
        }

        // GET: ProductManufacturers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductManufacturer productManufacturer = _LookupService.GetAllValidProductManufacturer(CurrentTenantId, id).FirstOrDefault();
            if (productManufacturer == null)
            {
                return HttpNotFound();
            }
            return View(productManufacturer);
        }

        // POST: ProductManufacturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _LookupService.RemoveProductManufacturer(id);
            return RedirectToAction("Index");
        }

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadProductManufacturerImage"] == null)
            {
                Session["UploadProductManufacturerImage"] = new List<string>();
            }
            var files = Session["UploadProductManufacturerImage"] as List<string>;

            foreach (var file in UploadControl)
            {
                SaveFile(file);
                files.Add(file.FileName);
            }
            Session["UploadProductManufacturerImage"] = files;

            return Content("true");
        }

        private void SaveFile(DevExpress.Web.UploadedFile file)
        {
            if (!Directory.Exists(Server.MapPath(UploadTempDirectory)))
                Directory.CreateDirectory(Server.MapPath(UploadTempDirectory));
            string resFileName = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            file.SaveAs(resFileName);
        }

        private string MoveFile(DevExpress.Web.UploadedFile file, string FileName, int ProductmanuId)
        {
            Session["UploadProductManufacturerImage"] = null;
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductmanuId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductmanuId.ToString()));

            string sourceFile = Server.MapPath(UploadTempDirectory + @"/" + FileName);
            string destFile = Server.MapPath(UploadDirectory + ProductmanuId.ToString() + @"/" + FileName);
            if (!System.IO.File.Exists(destFile))
            {
                System.IO.File.Move(sourceFile, destFile);
            }
            return (UploadDirectory.Replace("~", "") + ProductmanuId.ToString() + @"/" + FileName);
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