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
    public class ProductTagsController : BaseController
    {
        private readonly IProductLookupService _productLookupService;
        string UploadDirectory = "~/UploadedFiles/ProductTags/";
        string UploadTempDirectory = "~/UploadedFiles/ProductTags/TempFiles/";
        // GET: WebsiteNavigations

        public ProductTagsController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, IProductLookupService productLookupService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
        }

        // GET: WebsiteSliders
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            Session["UploadProductTag"] = null;
            return View();
        }

        public ActionResult _ProductTagList()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var productTags = _productLookupService.GetAllValidProductTag(CurrentTenantId).ToList();
            return PartialView(productTags);
        }

        // GET: WebsiteSliders/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "ProductTags";
            Session["UploadProductTag"] = null;
            return View();
        }

        // POST: WebsiteSliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductTag productTag, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                ViewBag.ControllerName = "ProductTags";
                var files = UploadControl;
                var filesName = Session["UploadProductTag"] as List<string>;
                var slider = _productLookupService.CreateOrUpdateProductTag(productTag, CurrentUserId, CurrentTenantId);
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in files)
                    {
                        filePath = MoveFile(file, filesName.FirstOrDefault(), slider.Id);
                        slider.IconImage = filePath;
                        _productLookupService.CreateOrUpdateProductTag(slider, CurrentUserId, CurrentTenantId);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(productTag);
        }

        // GET: WebsiteSliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            Session["UploadProductTag"] = null;
            ViewBag.ControllerName = "ProductTags";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductTag  productTag = _productLookupService.GetProductTagById(id??0);
            if (productTag == null)
            {
                return HttpNotFound();
            }
            ViewBag.Files = new List<string>();
            if (!string.IsNullOrEmpty(productTag.IconImage))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(productTag.IconImage);
                files.Add(dInfo.Name);
                Session["UploadProductTag"] = files;
                ViewBag.Files = files;
            }
            return View(productTag);
        }

        // POST: WebsiteSliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductTag productTag, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                string filePath = "";

                var filesName = Session["UploadProductTag"] as List<string>;
                if (filesName == null)
                { productTag.IconImage = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), productTag.Id);
                        productTag.IconImage = filePath;
                    }

                }
                var slider= _productLookupService.CreateOrUpdateProductTag(productTag, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            return View(productTag);
        }

        // GET: WebsiteSliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = _productLookupService.RemoveProductTag((id ?? 0), CurrentUserId);
            return RedirectToAction("Index");
        }



        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadProductTag"] == null)
            {
                Session["UploadProductTag"] = new List<string>();
            }
            var files = Session["UploadProductTag"] as List<string>;

            foreach (var file in UploadControl)
            {
                if (!string.IsNullOrEmpty(file.FileName))
                {

                    SaveFile(file);
                    files.Add(file.FileName);
                }

            }
            Session["UploadProductTag"] = files;

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
            Session["UploadProductTag"] = null;
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
