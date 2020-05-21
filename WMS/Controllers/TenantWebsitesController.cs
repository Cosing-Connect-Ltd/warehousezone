using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System.Web.Routing;
using Ganedata.Core.Services;
using DevExpress.CodeParser;

namespace WMS.Controllers
{
    public class TenantWebsitesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
      
        string UploadDirectory = "~/UploadedFiles/TenantWebSite/";
        string UploadTempDirectory = "~/UploadedFiles/TenantWebSite/TempFiles/";

        public TenantWebsitesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }



        // GET: TenantWebsites
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public ActionResult _TenantWebSiteList()
        {
            var model = _tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId).ToList();
            return PartialView(model);
        }

        // GET: TenantWebsites/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.siteid = id;
            return View();
        }


        // GET: TenantWebsites/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "TenantWebsites";
            return View();
        }

        // POST: TenantWebsites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TenantWebsites tenantWebsites, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                tenantWebsites.DefaultWarehouseId = CurrentWarehouseId;
                ViewBag.ControllerName = "TenantWebsites";
                var files = UploadControl;
                var filesName = Session["UploadTenantWebsiteLogo"] as List<string>;
                var TenantWebsite = _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in files)
                    {
                        filePath = MoveFile(file, filesName.FirstOrDefault(), TenantWebsite.SiteID);
                        TenantWebsite.Logo = filePath;
                        _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }


            return View(tenantWebsites);
        }

        // GET: TenantWebsites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "TenantWebsites";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantWebsites tenantWebsites = _tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId).FirstOrDefault(u => u.SiteID == id);
            if (tenantWebsites == null)
            {
                return HttpNotFound();
            }
            ViewBag.Files = new List<string>();
            if (!string.IsNullOrEmpty(tenantWebsites.Logo))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(tenantWebsites.Logo);
                files.Add(dInfo.Name);
                Session["UploadTenantWebsiteLogo"] = files;
                ViewBag.Files = files;
            }
            return View(tenantWebsites);
        }

        // POST: TenantWebsites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( TenantWebsites tenantWebsites, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "TenantWebsites";
            tenantWebsites.DefaultWarehouseId = CurrentWarehouseId;
            if (ModelState.IsValid)
            {
                string filePath = "";

                var filesName = Session["UploadTenantWebsiteLogo"] as List<string>;
                if (filesName == null) { tenantWebsites.Logo = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), tenantWebsites.SiteID);
                        tenantWebsites.Logo = filePath;
                    }

                }
                _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }

            return View(tenantWebsites);
        }

        // GET: TenantWebsites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _tenantWebsiteService.RemoveTenantWebsite((id ?? 0), CurrentUserId);
            return RedirectToAction("Index");
           
        }

        // POST: TenantWebsites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _tenantWebsiteService.RemoveTenantWebsite(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadTenantWebsiteLogo"] == null)
            {
                Session["UploadTenantWebsiteLogo"] = new List<string>();
            }
            var files = Session["UploadTenantWebsiteLogo"] as List<string>;

            foreach (var file in UploadControl)
            {

                SaveFile(file);
                files.Add(file.FileName);

            }
            Session["UploadTenantWebsiteLogo"] = files;

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
            Session["UploadTenantWebsiteLogo"] = null;
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


        public JsonResult _RemoveLogoFile(string filename, bool websiteSlider=false,bool tenantWebsite=false, bool navigationWebsite=false, bool websiteContent=false, string NavType="",bool productTag=false)
        {
            if (tenantWebsite)
            {
                var files = Session["UploadTenantWebsiteLogo"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (files.Count <= 0)
                {
                    Session["UploadTenantWebsiteLogo"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            else if (websiteSlider)
            {
                var files = Session["UploadWebsiteSlider"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (files.Count <= 0)
                {
                    Session["UploadWebsiteSlider"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            else if (navigationWebsite)
            {
                if (!string.IsNullOrEmpty(NavType))
                {
                    var files = Session["UploadTenantWebsiteNav"] as Dictionary<string, string>;
                    files.Remove(NavType);
                    if (files.Count <= 0)
                    {
                        Session["UploadTenantWebsiteNav"] = null;
                    }
                    var cfiles = files.Select(a => a.Key == NavType).ToList();
                    return Json(new { files = cfiles.Count == 0 ? null : cfiles });

                }

            }
            else if (websiteContent)
            {
                var files = Session["UploadWebsiteContentPage"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (files.Count <= 0)
                {
                    Session["UploadWebsiteContentPage"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            else if (productTag)
            {
                var files = Session["UploadProductTag"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == filename);
                files.Remove(filetoremove);
                if (files.Count <= 0)
                {
                    Session["UploadProductTag"] = null;
                }
                var cfiles = files.Select(a => a).ToList();
                return Json(new { files = cfiles.Count == 0 ? null : cfiles });
            }
            return default;
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
