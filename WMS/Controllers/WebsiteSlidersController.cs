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
    public class WebsiteSlidersController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        string UploadDirectory = "~/UploadedFiles/WebsiteSliders/";
        string UploadTempDirectory = "~/UploadedFiles/WebsiteSliders/TempFiles/";
        // GET: WebsiteNavigations

        public WebsiteSlidersController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: WebsiteSliders
        public ActionResult Index(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            Session["UploadWebsiteSlider"] = null;
            return View();
        }

        public ActionResult _SliderList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var slider = _tenantWebsiteService.GetAllValidWebsiteSlider(CurrentTenantId, SiteId).ToList();
            return PartialView(slider);
        }

        // GET: WebsiteSliders/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            ViewBag.ControllerName = "WebsiteSliders";
            Session["UploadWebsiteSlider"] = null;
            return View();
        }

        // POST: WebsiteSliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( WebsiteSlider websiteSlider, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ControllerName = "WebsiteSliders";
                var files = UploadControl;
                var filesName = Session["UploadWebsiteSlider"] as List<string>;
                var slider = _tenantWebsiteService.CreateOrUpdateProductswebsiteSlider(websiteSlider, CurrentUserId, CurrentTenantId);
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in files)
                    {
                        filePath = MoveFile(file, filesName.FirstOrDefault(), slider.Id);
                        websiteSlider.Image = filePath;
                        _tenantWebsiteService.CreateOrUpdateProductswebsiteSlider(websiteSlider, CurrentUserId, CurrentTenantId);
                        break;
                    }
                }
                return RedirectToAction("Index",new { SiteId= websiteSlider.SiteID });
            }

            return View(websiteSlider);
        }

        // GET: WebsiteSliders/Edit/5
        public ActionResult Edit(int? id)
        {
            Session["UploadWebsiteSlider"] = null;
            ViewBag.ControllerName = "WebsiteSliders";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteSlider websiteSlider = _tenantWebsiteService.GetWebsiteSliderById(id??0);
            if (websiteSlider == null)
            {
                return HttpNotFound();
            }
            ViewBag.Files = new List<string>();
            if (!string.IsNullOrEmpty(websiteSlider.Image))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(websiteSlider.Image);
                files.Add(dInfo.Name);
                Session["UploadWebsiteSlider"] = files;
                ViewBag.Files = files;
            }
            return View(websiteSlider);
        }

        // POST: WebsiteSliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( WebsiteSlider websiteSlider, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";

                var filesName = Session["UploadWebsiteSlider"] as List<string>;
                if (filesName == null) 
                { websiteSlider.Image = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), websiteSlider.Id);
                        websiteSlider.Image = filePath;
                    }

                }
                var slider=_tenantWebsiteService.CreateOrUpdateProductswebsiteSlider(websiteSlider, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index",new { SiteId=slider.SiteID});
            }
            return View(websiteSlider);
        }

        // GET: WebsiteSliders/Delete/5
        public ActionResult Delete(int? id)
        {
            var result = _tenantWebsiteService.RemoveWebsiteSlider((id ?? 0), CurrentUserId);
            return RedirectToAction("Index", new { SiteId = result.SiteID });
        }

     

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadWebsiteSlider"] == null)
            {
                Session["UploadWebsiteSlider"] = new List<string>();
            }
            var files = Session["UploadWebsiteSlider"] as List<string>;

            foreach (var file in UploadControl)
            {
                if (!string.IsNullOrEmpty(file.FileName))
                {

                    SaveFile(file);
                    files.Add(file.FileName);
                }

            }
            Session["UploadWebsiteSlider"] = files;

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
            Session["UploadWebsiteSlider"] = null;
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
