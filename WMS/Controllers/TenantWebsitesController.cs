using DevExpress.Web;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS.Controllers
{
    public class TenantWebsitesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IProductLookupService _productLookupService;

        string UploadDirectory = "~/UploadedFiles/TenantWebSite/";
        string UploadTempDirectory = "~/UploadedFiles/TenantWebSite/TempFiles/";

        public TenantWebsitesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService,IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
            _productLookupService = productLookupService;
        }



        // GET: TenantWebsites
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Session["UploadTenantWebsiteLogo"] = null;
            Session["UploadTenantWebsiteSmallLogo"] = null;
            Session["UploadTenantWebsiteFavicon"] = null;
            return View();
        }

        [ValidateInput(false)]
        public ActionResult _TenantWebSiteList()
        {
            var model = _tenantWebsiteService.GetAllValidTenantWebSites(CurrentTenantId).ToList();
            return PartialView(model);
        }

        // GET: TenantWebsites/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.siteid = id;
            return View();
        }

        public ActionResult _FileUploader(string name, string filePath)
        {
            return PartialView(new FileUploaderViewModel {
                                    BindingName = name,
                                    DisplayName = name,
                                    UploadedFiles = !string.IsNullOrEmpty(filePath) ? new List<string> { new DirectoryInfo(filePath).Name } : null,
                                    AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png", ".ico" }
            });
        }


        // GET: TenantWebsites/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            ViewBag.ProductTags = new SelectList(_productLookupService.GetAllValidProductTag(CurrentTenantId),"Id","TagName");
            Session["UploadTenantWebsiteLogo"] = null;
            Session["UploadTenantWebsiteSmallLogo"] = null;
            Session["UploadTenantWebsiteFavicon"] = null;
            return View();
        }

        // POST: TenantWebsites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TenantWebsites tenantWebsites)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                tenantWebsites.DefaultWarehouseId = CurrentWarehouseId;
                var TenantWebsite = _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);

                var logoFilesName = Session["UploadTenantWebsiteLogo"] as string;
                TenantWebsite.Logo = logoFilesName != null ? MoveFile(logoFilesName, TenantWebsite.SiteID) : null;

                var smallLogoFilesName = Session["UploadTenantWebsiteSmallLogo"] as string;
                TenantWebsite.SmallLogo = smallLogoFilesName != null ? MoveFile(smallLogoFilesName, TenantWebsite.SiteID) : null;

                var faviconFilesName = Session["UploadTenantWebsiteFavicon"] as string;
                TenantWebsite.Favicon = faviconFilesName != null ? MoveFile(faviconFilesName, TenantWebsite.SiteID) : null;

                if (!string.IsNullOrEmpty(faviconFilesName) || !string.IsNullOrEmpty(logoFilesName))
                {
                    _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);
                }

                Session["UploadTenantWebsiteFavicon"] = null;
                Session["UploadTenantWebsiteLogo"] = null;
                Session["UploadTenantWebsiteSmallLogo"] = null;
                return RedirectToAction("Index");
            }


            return View(tenantWebsites);
        }

        // GET: TenantWebsites/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tenantWebsites = _tenantWebsiteService.GetTenantWebSiteBySiteId(id.Value);
            if (tenantWebsites == null)
            {
                return HttpNotFound();
            }

            Session["UploadTenantWebsiteLogo"] = !string.IsNullOrEmpty(tenantWebsites.Logo) ? new DirectoryInfo(tenantWebsites.Logo).Name : null;

            Session["UploadTenantWebsiteSmallLogo"] = !string.IsNullOrEmpty(tenantWebsites.SmallLogo) ? new DirectoryInfo(tenantWebsites.SmallLogo).Name : null;

            Session["UploadTenantWebsiteFavicon"] = !string.IsNullOrEmpty(tenantWebsites.Favicon) ? new DirectoryInfo(tenantWebsites.Favicon).Name : null;

            ViewBag.ProductTag = new SelectList(_productLookupService.GetAllValidProductTag(CurrentTenantId), "Id", "TagName");
            return View(tenantWebsites);
        }


        // POST: TenantWebsites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( TenantWebsites tenantWebsites,
                                  IEnumerable<UploadedFile> LogoUploadControl,
                                  IEnumerable<UploadedFile> SmallLogoUploadControl,
                                  IEnumerable<UploadedFile> FaviconUploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            tenantWebsites.DefaultWarehouseId = CurrentWarehouseId;
            if (ModelState.IsValid)
            {
                var logoFileName = Session["UploadTenantWebsiteLogo"] as string;
                if (string.IsNullOrEmpty(logoFileName)) { tenantWebsites.Logo = ""; }
                else
                {
                    if (LogoUploadControl != null && LogoUploadControl.Count() > 0)
                    {
                        tenantWebsites.Logo = MoveFile(logoFileName, tenantWebsites.SiteID);
                    }
                }

                var smallLogoFileName = Session["UploadTenantWebsiteSmallLogo"] as string;
                if (string.IsNullOrEmpty(smallLogoFileName)) { tenantWebsites.SmallLogo = ""; }
                else
                {
                    if (SmallLogoUploadControl != null && SmallLogoUploadControl.Count() > 0)
                    {
                        tenantWebsites.SmallLogo = MoveFile(smallLogoFileName, tenantWebsites.SiteID);
                    }
                }

                var faviconFileName = Session["UploadTenantWebsiteFavicon"] as string;
                if (string.IsNullOrEmpty(faviconFileName)) { tenantWebsites.Favicon = ""; }
                else
                {
                    if (FaviconUploadControl != null && FaviconUploadControl.Count() > 0)
                    {
                        tenantWebsites.Favicon = MoveFile(faviconFileName, tenantWebsites.SiteID);
                    }
                }

                _tenantWebsiteService.CreateOrUpdateTenantWebsite(tenantWebsites, CurrentUserId, CurrentTenantId);
                Session["UploadTenantWebsiteFavicon"] = null;
                Session["UploadTenantWebsiteLogo"] = null;
                Session["UploadTenantWebsiteSmallLogo"] = null;
                return RedirectToAction("Index");
            }

            return View(tenantWebsites);
        }

        // GET: TenantWebsites/EditSubscriptionPanel/5
        public ActionResult EditLayoutSettings(int siteId)
        {
            var layoutSettings = _tenantWebsiteService.GetWebsiteLayoutSettingsInfoBySiteId(siteId);

            var tenantWebsite = _tenantWebsiteService.GetTenantWebSiteBySiteId(siteId);

            ViewBag.SiteName = tenantWebsite.SiteName;

            Session["UploadTenantWebsiteSubscriptionPanelImage"] = !string.IsNullOrEmpty(layoutSettings?.SubscriptionPanelImageUrl) ? new DirectoryInfo(layoutSettings.SubscriptionPanelImageUrl).Name : null;

            return View(layoutSettings ?? new WebsiteLayoutSettings { SiteId = siteId, Id = 0});
        }

        // POST: TenantWebsites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLayoutSettings(WebsiteLayoutSettings layoutSettings, IEnumerable<UploadedFile> SubscriptionPanelImageUploadControl)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                var fileName = Session["UploadTenantWebsiteSubscriptionPanelImage"] as string;

                layoutSettings = _tenantWebsiteService.SaveWebsiteLayoutSettings(layoutSettings, CurrentUserId, CurrentTenantId);

                if (fileName != null && SubscriptionPanelImageUploadControl != null && SubscriptionPanelImageUploadControl.Count() > 0)
                {
                    layoutSettings.SubscriptionPanelImageUrl = MoveFile(fileName, layoutSettings.SiteId);
                    _tenantWebsiteService.SaveWebsiteLayoutSettings(layoutSettings, CurrentUserId, CurrentTenantId);
                    Session["UploadTenantWebsiteSubscriptionPanelImage"] = null;
                }

                return RedirectToAction("Index");
            }

            return View(layoutSettings);
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

        public ActionResult UploadSubscriptionPanelImageFile(IEnumerable<UploadedFile> SubscriptionPanelImageUploadControl)
        {
            return UploadFile(SubscriptionPanelImageUploadControl, "SubscriptionPanelImage");
        }

        public ActionResult UploadFaviconFile(IEnumerable<UploadedFile> FaviconUploadControl)
        {
            return UploadFile(FaviconUploadControl, "Favicon");
        }

        public ActionResult UploadLogoFile(IEnumerable<UploadedFile> LogoUploadControl)
        {
            return UploadFile(LogoUploadControl, "Logo");
        }

        public ActionResult UploadSmallLogoFile(IEnumerable<UploadedFile> SmallLogoUploadControl)
        {
            return UploadFile(SmallLogoUploadControl, "SmallLogo");
        }

        public ActionResult UploadFile(IEnumerable<UploadedFile> UploadControl, string bindingName)
        {
            foreach (var file in UploadControl)
            {
                SaveFile(file);
                Session[$"UploadTenantWebsite{bindingName}"] = file.FileName;
            }

            return Content("true");
        }

        private void SaveFile(UploadedFile file)
        {
            if (!Directory.Exists(Server.MapPath(UploadTempDirectory)))
                Directory.CreateDirectory(Server.MapPath(UploadTempDirectory));
            string resFileName = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            file.SaveAs(resFileName);
        }

        private string MoveFile(string FileName, int RelatedObjectId)
        {
            if (!Directory.Exists(Server.MapPath(UploadDirectory + RelatedObjectId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + RelatedObjectId.ToString()));

            string sourceFile = Server.MapPath(UploadTempDirectory + @"/" + FileName);
            string destFile = Server.MapPath(UploadDirectory + RelatedObjectId.ToString() + @"/" + FileName);
            if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(destFile))
            {
                System.IO.File.Move(sourceFile, destFile);
            }
            return (UploadDirectory.Replace("~", "") + RelatedObjectId.ToString() + @"/" + FileName);
        }

        public JsonResult RemoveFile(string fileName, string bindingName)
        {
            Session[$"UploadTenantWebsite{bindingName}"] = null;
            return null;
        }

        public JsonResult _RemoveLogoFile(string fileName, bool websiteSlider=false, bool navigationWebsite=false, bool websiteContent=false, string NavType="",bool productTag=false)
        {
            if (websiteSlider)
            {
                var files = Session["UploadWebsiteSlider"] as List<string>;
                var filetoremove = files.FirstOrDefault(a => a == fileName);
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
                var filetoremove = files.FirstOrDefault(a => a == fileName);
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
                var filetoremove = files.FirstOrDefault(a => a == fileName);
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

            binder.UploadControlBinderSettings.FileUploadCompleteHandler = (s, e) =>
            {
                e.CallbackData = e.UploadedFile.FileName;
            };

            base.Initialize(requestContext);
        }

    }
}
