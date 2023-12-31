﻿using DevExpress.Web.Mvc;
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
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class WebsiteNavigationsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        string UploadDirectory = "~/UploadedFiles/TenantWebsiteNavigation/";
        string UploadTempDirectory = "~/UploadedFiles/TenantWebsiteNavigation/TempFiles/";
        // GET: WebsiteNavigations

        public WebsiteNavigationsController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantWebsiteService = tenantWebsiteService;
        }

        public ActionResult Index(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            Session["UploadTenantWebsiteNav"] = null;
            return View();
        }

        public ActionResult _NavigationList(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var model = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, SiteId).ToList();
            return PartialView(model);
        }

        // GET: WebsiteNavigations/Create
        public ActionResult Create(int SiteId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            Session["UploadTenantWebsiteNav"] = null;
            ViewBag.siteid = SiteId;
            SiteName(SiteId);
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, SiteId).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, SiteId), "Id", "Title");
            return View();
        }

        public ActionResult _Uploader(string Name)
        {
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ImageUploadControlName = Name;

            return PartialView("_Uploader");
        }
        public ActionResult _HoverUploader(string Name)
        {
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.HoverImageUploadControlName = Name;

            return PartialView("_HoverUploader");
        }

        public ActionResult CreateProductNavigation(int SiteId, int navigationId)
        {
            ViewBag.SiteId = SiteId;
            SiteName(SiteId);
            ViewBag.navigationId = navigationId;
            return View();
        }

        public ActionResult SaveNavigationProductList(int SiteId, int navigationId, MVCxGridViewBatchUpdateValues<NavigationProductsViewModel, int> updateValues)
            {
            List<bool> results = new List<bool>();
            foreach (var value in updateValues.Update)
            {
                value.NavigationId = navigationId;
                value.SiteID = SiteId;
                var res = _tenantWebsiteService.CreateOrUpdateWebsiteNavigationProducts(value, CurrentUserId, CurrentTenantId);
                results.Add(res);
            }

            return _NavigationProductList(SiteId, navigationId);
        }

        public ActionResult _NavigationProductList(int SiteId, int navigationId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            ViewBag.Controller = "WebsiteNavigations";
            ViewBag.SiteId = SiteId;
            ViewBag.NavigationId = navigationId;
            var data = _tenantWebsiteService.GetWebsiteNavigationId(navigationId).ProductsNavigationMap.Select(u => u.ProductWebsiteMapId).ToList();
            ViewData["NaviagtionProduct"] = data;

            if (viewModel == null)
                viewModel = WebsiteNavigationProductsCustomBinding.CreateProductNavigationViewModel();

            return NavigationProductListActionCore(viewModel, SiteId, navigationId);
        }

        public ActionResult _NavigationProductListPaging(GridViewPagerState pager, int SiteId, int navigationId)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            ViewBag.Controller = "WebsiteNavigations";
            viewModel.Pager.Assign(pager);
            return NavigationProductListActionCore(viewModel, SiteId, navigationId);
        }

        public ActionResult _NavigationProductListFiltering(GridViewFilteringState filteringState, int SiteId, int navigationId)
        {
            ViewBag.SiteId = SiteId;
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            viewModel.ApplyFilteringState(filteringState);
            return NavigationProductListActionCore(viewModel, SiteId, navigationId);
        }


        public ActionResult _NavigationProductListSorting(GridViewColumnState column, int SiteId, bool reset, int navigationId)
        {
            ViewBag.SiteId = SiteId;
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            viewModel.ApplySortingState(column, reset);
            return NavigationProductListActionCore(viewModel, SiteId, navigationId);
        }


        public ActionResult NavigationProductListActionCore(GridViewModel gridViewModel, int SiteId, int navigationId)
        {
            ViewBag.Controller = "WebsiteNavigations";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    WebsiteNavigationProductsCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, SiteId, navigationId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        WebsiteNavigationProductsCustomBinding.ProductGetData(args, CurrentTenantId, SiteId, navigationId);
                    })
            );
            return PartialView("_NavProductList", gridViewModel);
        }

        // POST: WebsiteNavigations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteNavigation websiteNavigation, string ProductsWithIds, IEnumerable<DevExpress.Web.UploadedFile> DefaultImage, IEnumerable<DevExpress.Web.UploadedFile> HoverImage)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (websiteNavigation != null)
            {
                websiteNavigation.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateWebsiteNavigation(websiteNavigation, CurrentUserId, CurrentTenantId);
                var Defaultfiles = DefaultImage;
                var hoverFile = HoverImage;
                var filesName = Session["UploadTenantWebsiteNav"] as Dictionary<string, string>;
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in Defaultfiles)
                    {
                        var defultFileName = filesName.FirstOrDefault(u => u.Key == "Default").Value;
                        if (!string.IsNullOrEmpty(defultFileName))
                        {
                            filePath = MoveFile(file, defultFileName, websiteNavigation.Id);
                            websiteNav.Image = filePath;
                        }
                        break;
                    }
                    foreach (var file in hoverFile)
                    {
                        var hoverFileName = filesName.FirstOrDefault(u => u.Key == "Hover").Value;
                        if (!string.IsNullOrEmpty(hoverFileName))
                        {
                            filePath = MoveFile(file, hoverFileName, websiteNavigation.Id);
                            websiteNav.HoverImage = filePath;
                        }
                        break;
                    }
                    if (!string.IsNullOrEmpty(websiteNav.Image) || !string.IsNullOrEmpty(websiteNav.HoverImage))
                    {
                        websiteNav.SelectedProductIds = string.Empty;
                        _tenantWebsiteService.CreateOrUpdateWebsiteNavigation(websiteNav, CurrentUserId, CurrentTenantId);
                    }


                }
                Session["UploadTenantWebsiteNav"] = null;
                return RedirectToAction("Index", new { SiteId = websiteNavigation.SiteID });

            }
            SiteName(websiteNavigation.SiteID);
            ViewBag.siteid = websiteNavigation.SiteID;
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = "HoverImage";

            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, websiteNavigation.SiteID).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, websiteNavigation.SiteID), "Id", "Title");
            return View(websiteNavigation);
        }

        //// GET: WebsiteNavigations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteNavigation websiteNavigation = _tenantWebsiteService.GetWebsiteNavigationId(id ?? 0);
            if (websiteNavigation == null)
            {
                return HttpNotFound();
            }
            Session["UploadTenantWebsiteNav"] = null;
            ViewBag.Files = new Dictionary<string, string>();
            Dictionary<string, string> files = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(websiteNavigation.Image) || !string.IsNullOrEmpty(websiteNavigation.HoverImage))
            {
                if (!string.IsNullOrEmpty(websiteNavigation.Image))
                {

                    ViewBag.FileLength = true;
                    DirectoryInfo dInfo = new DirectoryInfo(websiteNavigation.Image);
                    files.Add("Default", dInfo.Name);

                }
                if (!string.IsNullOrEmpty(websiteNavigation.HoverImage))
                {

                    ViewBag.FileLengthHover = true;
                    DirectoryInfo dInfo = new DirectoryInfo(websiteNavigation.HoverImage);
                    files.Add("Hover", dInfo.Name);

                }

                Session["UploadTenantWebsiteNav"] = files;
                ViewBag.Files = files;
            }


            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ParentIds = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, websiteNavigation.SiteID).ToList(), "Id", "Name", websiteNavigation.ParentId);
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, websiteNavigation.SiteID), "Id", "Title", websiteNavigation.ContentPageId);
            SiteName(websiteNavigation.SiteID);
            return View(websiteNavigation);
        }

        //// POST: WebsiteNavigations/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WebsiteNavigation websiteNavigation, string ProductsWithIds, IEnumerable<DevExpress.Web.UploadedFile> DefaultImage, IEnumerable<DevExpress.Web.UploadedFile> HoverImage, int SiteID)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (websiteNavigation != null)
            {
                websiteNavigation.SelectedProductIds = ProductsWithIds;
                string filePath = "";

                var filesName = Session["UploadTenantWebsiteNav"] as Dictionary<string, string>;

                if (filesName == null)
                {
                    websiteNavigation.Image = string.Empty;
                    websiteNavigation.HoverImage = string.Empty;

                }

                else
                {
                    if (DefaultImage != null)
                    {
                        var defaultImage = filesName?.FirstOrDefault(u => u.Key == "Default").Value;
                        if (defaultImage != null)
                        {
                            filePath = MoveFile(DefaultImage.FirstOrDefault(), defaultImage, websiteNavigation.Id);
                            websiteNavigation.Image = filePath;
                        }
                        else {
                            websiteNavigation.Image = string.Empty;
                        }
                    }
                    if (HoverImage != null)
                    {
                        var hoverImage = filesName?.FirstOrDefault(u => u.Key == "Hover").Value;
                        if (hoverImage != null)
                        {
                            filePath = MoveFile(HoverImage.FirstOrDefault(), hoverImage, websiteNavigation.Id);
                            websiteNavigation.HoverImage = filePath;
                        }
                        else
                        {
                            websiteNavigation.HoverImage = string.Empty;
                        }
                    }

                }
                _tenantWebsiteService.CreateOrUpdateWebsiteNavigation(websiteNavigation, CurrentUserId, CurrentTenantId);
                Session["UploadTenantWebsiteNav"] = null;
                return RedirectToAction("Index", new { SiteId = SiteID });
            }
            SiteName(websiteNavigation.SiteID);
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, websiteNavigation.SiteID).ToList(), "Id", "Name", websiteNavigation.ParentId);
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, websiteNavigation.SiteID), "Id", "Title", websiteNavigation.ContentPageId);
            ViewBag.SiteId = SiteID;
            return View(websiteNavigation);
        }

        //// GET: WebsiteNavigations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var navigation = _tenantWebsiteService.RemoveWebsiteNavigation((id ?? 0), CurrentUserId);
            return RedirectToAction("Index", new { SiteId = navigation.SiteID });
        }


        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> DefaultImage, IEnumerable<DevExpress.Web.UploadedFile> HoverImage)
        {
            if (Session["UploadTenantWebsiteNav"] == null)
            {
                Session["UploadTenantWebsiteNav"] = new Dictionary<string, string>();
            }
            var files = Session["UploadTenantWebsiteNav"] as Dictionary<string, string>;

            if (DefaultImage != null)
            {
                foreach (var file in DefaultImage)
                {
                    if (!string.IsNullOrEmpty(file.FileName))
                    {

                        SaveFile(file);
                        if (!files.ContainsKey("Default"))
                        {
                            files.Add("Default", file.FileName);
                        }
                    }

                }
            }

            if (HoverImage != null)
            {
                foreach (var file in HoverImage)
                {
                    if (!string.IsNullOrEmpty(file.FileName))
                    {

                        SaveFile(file);
                        if (!files.ContainsKey("Hover"))
                        {
                            files.Add("Hover", file.FileName);
                        }
                    }

                }
            }
            Session["UploadTenantWebsiteNav"] = files;

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
