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
    public class WebsiteContentPagesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        string UploadDirectory = "~/UploadedFiles/WebsiteContentPage/";
        string UploadTempDirectory = "~/UploadedFiles/WebsiteContentPage/TempFiles/";
        // GET: WebsiteNavigations

        public WebsiteContentPagesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: WebsiteContentPages
        public ActionResult Index(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            Session["UploadWebsiteContentPage"] = null;
            return View();
        }

        public ActionResult _WebsiteContentPagesList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var contentPages = _tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, SiteId).ToList();
            return PartialView(contentPages);
        }


        // GET: WebsiteContentPages/Create
        public ActionResult Create(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            Session["UploadWebsiteContentPage"] = null;
            ViewBag.ControllerName = "WebsiteContentPages";
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            var websitecontentPages = new WebsiteContentPages();
            return View(websitecontentPages);
        }

        // POST: WebsiteContentPages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteContentPages websiteContentPages, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "WebsiteContentPages";
            SiteName(websiteContentPages.SiteID);
            if (ModelState.IsValid)
            {
                var files = UploadControl;
                var filesName = Session["UploadWebsiteContentPage"] as List<string>;
                var websitecontent=_tenantWebsiteService.CreateOrUpdateWebsiteContentPages(websiteContentPages, CurrentUserId, CurrentTenantId);
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in files)
                    {
                        filePath = MoveFile(file, filesName.FirstOrDefault(), websitecontent.Id);
                        websitecontent.Image = filePath;
                        _tenantWebsiteService.CreateOrUpdateWebsiteContentPages(websitecontent, CurrentUserId, CurrentTenantId);
                        break;
                    }
                }
                return RedirectToAction("Index", new { SiteId = websiteContentPages.SiteID });
            }

            return View(websiteContentPages);
        }

        // GET: WebsiteContentPages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteContentPages websiteContentPages = _tenantWebsiteService.GetWebsiteContentById(id??0);
            if (websiteContentPages == null)
            {
                return HttpNotFound();
            }
            ViewBag.ControllerName = "WebsiteContentPages";
            Session["UploadWebsiteContentPage"] = null;
            ViewBag.Files = new List<string>();
            if (!string.IsNullOrEmpty(websiteContentPages.Image))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(websiteContentPages.Image);
                files.Add(dInfo.Name);
                Session["UploadWebsiteContentPage"] = files;
                ViewBag.Files = files;
            }
            SiteName(websiteContentPages.SiteID);
            return View(websiteContentPages);
        }

        // POST: WebsiteContentPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( WebsiteContentPages websiteContentPages, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "WebsiteContentPages";
            if (ModelState.IsValid)
            {
                string filePath = "";
                var filesName = Session["UploadWebsiteContentPage"] as List<string>;
                if (filesName == null)
                { websiteContentPages.Image = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), websiteContentPages.Id);
                        websiteContentPages.Image = filePath;
                    }

                }
                var contentPages=_tenantWebsiteService.CreateOrUpdateWebsiteContentPages(websiteContentPages, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index",new { SiteId= contentPages.SiteID });
            }
            SiteName(websiteContentPages.SiteID);
            ViewBag.SiteID = new SelectList(_tenantWebsiteService.GetAllActiveTenantWebSites(CurrentTenantId), "SiteID", "SiteName", websiteContentPages.SiteID);
            return View(websiteContentPages);
        }

        //GET: WebsiteContentPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contentPages=_tenantWebsiteService.RemoveWebsiteContentPages((id ?? 0), CurrentUserId);

            return RedirectToAction("Index", new { SiteId = contentPages.SiteID });
        }
        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadWebsiteContentPage"] == null)
            {
                Session["UploadWebsiteContentPage"] = new List<string>();
            }
            var files = Session["UploadWebsiteContentPage"] as List<string>;

            foreach (var file in UploadControl)
            {
                if (!string.IsNullOrEmpty(file.FileName))
                {

                    SaveFile(file);
                    files.Add(file.FileName);
                }

            }
            Session["UploadWebsiteContentPage"]  = files;

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
            Session["UploadWebsiteContentPage"] = null;
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
