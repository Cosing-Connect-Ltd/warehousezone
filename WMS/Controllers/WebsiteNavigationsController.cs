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

        public ActionResult Index()
        {
            
            return View();
        }

        // GET: WebsiteNavigations/Create
        public ActionResult Create(int SiteId)
        {
            ViewBag.siteid = SiteId;
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = "HoverImage";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId), "Id", "MetaTitle");
            return View();
        }

        public ActionResult _Uploader(string Name)
        {
            ViewBag.ControllerName = "WebsiteNavigations"; ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = Name;

            return PartialView("_Uploader");
        }
        public ActionResult _NavigationProductList()
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            ViewBag.Controller = "WebsiteNavigations";
            
            if (viewModel == null)
                viewModel = ProducctListCustomBinding.CreateProductGridViewModel();


            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            ViewBag.Controller = "WebsiteNavigations";
            viewModel.Pager.Assign(pager);
            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductsFiltering(GridViewFilteringState filteringState)
        {
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ProductGridActionCore(viewModel);
        }


        public ActionResult _ProductsGetDataSorting(GridViewColumnState column, bool reset)
        {
            ViewBag.Controller = "WebsiteNavigations";
            var viewModel = GridViewExtension.GetViewModel("ProductWebMapsGridView");
            viewModel.ApplySortingState(column, reset);
            return ProductGridActionCore(viewModel);
        }


        public ActionResult ProductGridActionCore(GridViewModel gridViewModel)
        {
            ViewBag.Controller = "WebsiteNavigations";
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProducctListCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId,true);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProducctListCustomBinding.ProductGetData(args, CurrentTenantId, CurrentWarehouseId,true);
                    })
            );
            return PartialView("_NavProductList", gridViewModel);
        }





        // POST: WebsiteNavigations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WebsiteNavigation websiteNavigation, IEnumerable<DevExpress.Web.UploadedFile> ImageDefault, IEnumerable<DevExpress.Web.UploadedFile> HoverImage)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            ViewBag.siteid = websiteNavigation.SiteID;
            ViewBag.ControllerName = "WebsiteNavigations";
            ViewBag.Name = "HoverImage";
            ViewBag.ParentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteNavigation(CurrentTenantId).ToList(), "Id", "Name");
            ViewBag.contentId = new SelectList(_tenantWebsiteService.GetAllValidWebsiteContentPages(CurrentTenantId), "Id", "MetaTitle");
            return View(websiteNavigation);
        }

        //// GET: WebsiteNavigations/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WebsiteNavigation websiteNavigation = db.WebsiteNavigations.Find(id);
        //    if (websiteNavigation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ParentId = new SelectList(db.WebsiteNavigations, "Id", "Image", websiteNavigation.ParentId);
        //    ViewBag.SiteID = new SelectList(db.TenantWebsites, "SiteID", "SiteName", websiteNavigation.SiteID);
        //    return View(websiteNavigation);
        //}

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
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WebsiteNavigation websiteNavigation = db.WebsiteNavigations.Find(id);
        //    if (websiteNavigation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(websiteNavigation);
        //}

        //// POST: WebsiteNavigations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    WebsiteNavigation websiteNavigation = db.WebsiteNavigations.Find(id);
        //    db.WebsiteNavigations.Remove(websiteNavigation);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> ImageDefault)
        {
            if (Session["UploadTenantWebsiteNav"] == null)
            {
                Session["UploadTenantWebsiteNav"] = new List<string>();
            }
            var files = Session["UploadTenantWebsiteNav"] as List<string>;

            foreach (var file in ImageDefault)
            {

                SaveFile(file);
                files.Add(file.FileName);

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
