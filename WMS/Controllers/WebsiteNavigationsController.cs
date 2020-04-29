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
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class WebsiteNavigationsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        string UploadDirectory = "~/UploadedFiles/TenantWebsiteNavigation/";
        string UploadTempDirectory = "~/UploadedFiles/TenantWebsiteNavigation/TempFiles/";
        // GET: WebsiteNavigations

        public WebsiteNavigationsController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        public ActionResult Index(int SiteId)
        {
            ViewBag.SiteId = SiteId;
            return View();
        }

        public ActionResult _NavigationList(int SiteId)
        {
            var model = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, SiteId).ToList();
            return PartialView(model);
        }

        // GET: WebsiteNavigations/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.siteid = SiteId;
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, SiteId).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, SiteId), "Id", "MetaTitle");
            return View();
        }

        public ActionResult _Uploader(string Name)
        {
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = Name;

            return PartialView("_Uploader");
        }
        public ActionResult _HoverUploader(string Name)
        {
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = Name;

            return PartialView("_HoverUploader");
        }
        public ActionResult _NavigationProductList(int SiteId)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            ViewBag.Controller = "WebsiteNavigations";
            ViewBag.SiteId = SiteId;

            if (viewModel == null)
                viewModel = ProducctListCustomBinding.CreateProductGridViewModel();


            return ProductGridActionCore(viewModel, SiteId);
        }

        public ActionResult _ProductListPaging(GridViewPagerState pager, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            ViewBag.Controller = "WebsiteNavigations";
            viewModel.Pager.Assign(pager);
            return ProductGridActionCore(viewModel, SiteId);
        }

        public ActionResult _ProductsFiltering(GridViewFilteringState filteringState, int SiteId)
        {
            ViewBag.SiteId = SiteId;
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ProductGridActionCore(viewModel, SiteId);
        }


        public ActionResult _ProductsGetDataSorting(GridViewColumnState column, int SiteId, bool reset)
        {
            ViewBag.SiteId = SiteId;
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebNavGridView");
            viewModel.ApplySortingState(column, reset);
            return ProductGridActionCore(viewModel, SiteId);
        }


        public ActionResult ProductGridActionCore(GridViewModel gridViewModel, int SiteId)
        {
            ViewBag.Controller = "WebsiteNavigations";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProducctListCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId, true, SiteId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProducctListCustomBinding.ProductGetData(args, CurrentTenantId, CurrentWarehouseId, true, SiteId);
                    })
            );
            return PartialView("_NavProductList", gridViewModel);
        }

        // POST: WebsiteNavigations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteNavigation websiteNavigation, string ProductsWithIds, IEnumerable<DevExpress.Web.UploadedFile> ImageDefault, IEnumerable<DevExpress.Web.UploadedFile> HoverImage)
        {
            if (websiteNavigation != null)
            {
                websiteNavigation.SelectedProductIds = ProductsWithIds;
                var websiteNav = _tenantWebsiteService.CreateOrUpdateWebsiteNavigation(websiteNavigation, CurrentUserId, CurrentTenantId);
                var Defaultfiles = ImageDefault;
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
                            filePath = MoveFile(file, defultFileName, websiteNavigation.SiteID);
                            websiteNav.Image = filePath;
                        }
                        break;
                    }
                    foreach (var file in hoverFile)
                    {
                        var hoverFileName = filesName.FirstOrDefault(u => u.Key == "Hover").Value;
                        if (!string.IsNullOrEmpty(hoverFileName))
                        {
                            filePath = MoveFile(file, hoverFileName, websiteNavigation.SiteID);
                            websiteNav.Image = filePath;
                        }
                        break;
                    }
                    if (!string.IsNullOrEmpty(websiteNav.Image) || !string.IsNullOrEmpty(websiteNav.HoverImage))
                    {
                         websiteNav.SelectedProductIds = string.Empty;
                        _tenantWebsiteService.CreateOrUpdateWebsiteNavigation(websiteNav, CurrentUserId, CurrentTenantId);
                    }


                }
                return RedirectToAction("Index",new { SiteId= websiteNavigation .SiteID});

            }
            ViewBag.siteid = websiteNavigation.SiteID;
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = "HoverImage";

            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, websiteNavigation.SiteID).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, websiteNavigation.SiteID), "Id", "MetaTitle");
            return View(websiteNavigation);
        }

        //// GET: WebsiteNavigations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WebsiteNavigation websiteNavigation = _tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId,null).FirstOrDefault(u=>u.Id==id);
            if (websiteNavigation == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId, websiteNavigation.SiteID).ToList(), "Id", "Name", websiteNavigation.ParentId);
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId, websiteNavigation.SiteID), "Id", "MetaTitle", websiteNavigation.ContentPageId);
            return View(websiteNavigation);
        }

        //// POST: WebsiteNavigations/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,SiteID,Image,IamgeAltTag,HoverImage,HoverIamgeAltTag,Name,SortOrder,IsActive,ParentId,Type,ContentPageId,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] WebsiteNavigation websiteNavigation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //       //db.Entry(websiteNavigation).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ParentId = new SelectList(db.WebsiteNavigations, "Id", "Image", websiteNavigation.ParentId);
        //    ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteNavigation.SiteID);
        //    return View(websiteNavigation);
        //}

        //// GET: WebsiteNavigations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var navigation = _tenantWebsiteService.RemoveWebsiteNavigation((id??0), CurrentUserId);
            return RedirectToAction("Index", new { SiteId = navigation.SiteID });
        }

        
        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> ImageDefault, IEnumerable<DevExpress.Web.UploadedFile> HoverImage)
        {
            if (Session["UploadTenantWebsiteNav"] == null)
            {
                Session["UploadTenantWebsiteNav"] = new Dictionary<string, string>();
            }
            var files = Session["UploadTenantWebsiteNav"] as Dictionary<string, string>;

            foreach (var file in ImageDefault)
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
            Session["UploadTenantWebsiteNav"] = null;
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
