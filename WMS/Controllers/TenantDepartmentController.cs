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
    public class TenantDepartmentController : BaseController
    {

        private readonly IProductLookupService _productLookupService;
        private readonly ILookupServices _LookupService;
        string UploadDirectory = "~/UploadedFiles/TenantDepartment/";
        string UploadTempDirectory = "~/UploadedFiles/TenantDepartment/TempFiles/";

        public TenantDepartmentController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;
            _LookupService = lookupServices;
        }

        // GET: TenantDepartment
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        [ValidateInput(false)]
        public ActionResult TenantDepartmentList()
        {
            var model = _LookupService.GetAllValidTenantDepartments(CurrentTenantId).ToList();
            return PartialView("_TenantDepartmentList", model);
        }

        // GET: TenantDepartment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantDepartments tenantDepartments = _LookupService.GetTenantDepartmentById(id ?? 0);

            if (tenantDepartments == null)
            {
                return HttpNotFound();
            }
            return View(tenantDepartments);
        }

        // GET: TenantDepartment/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.ControllerName = "TenantDepartment";
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountIDs = new SelectList(accounts, "AccountID", "AccountNameCode");
            return View();

        }

        // POST: TenantDepartment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,DepartmentName,TenantId,AccountID,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantDepartments tenantDepartments, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (ModelState.IsValid)
            {
                var files = UploadControl;
                var filesName = Session["UploadTenantDepartmentImage"] as List<string>;
                var departments = _LookupService.SaveTenantDepartment(tenantDepartments.DepartmentName, tenantDepartments.AccountID, CurrentUserId, CurrentTenantId);
                string filePath = "";
                if (filesName != null)
                {
                    foreach (var file in files)
                    {
                        filePath = MoveFile(file, filesName.FirstOrDefault(), departments.DepartmentId);
                        departments.ImagePath = filePath;
                        _LookupService.UpdateTenantDepartment(departments);
                        break;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.ControllerName = "TenantDepartment";
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountIDs = new SelectList(accounts, "AccountID", "AccountNameCode");
            return View(tenantDepartments);
        }

        // GET: TenantDepartment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          
            ViewBag.ControllerName = "TenantDepartment";
            TenantDepartments tenantDepartments = _LookupService.GetTenantDepartmentById(id ?? 0);
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountIDs = new SelectList(accounts, "AccountID", "AccountNameCode", tenantDepartments?.AccountID);
            if (tenantDepartments == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrEmpty(tenantDepartments.ImagePath))
            {
                List<string> files = new List<string>();
                ViewBag.FileLength = true;
                DirectoryInfo dInfo = new DirectoryInfo(tenantDepartments.ImagePath);
                files.Add(dInfo.Name);
                Session["UploadTenantDepartmentImage"] = files;
                ViewBag.Files = files;
            }
            //ViewBag.TenantId = new SelectList(db.Tenants, "TenantId", "TenantName", tenantDepartments.TenantId);
            return View(tenantDepartments);
        }

        // POST: TenantDepartment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName,TenantId,AccountID,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantDepartments tenantDepartments, IEnumerable <DevExpress.Web.UploadedFile> UploadControl)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                tenantDepartments.DepartmentId = tenantDepartments.DepartmentId;
                tenantDepartments.CreatedBy = CurrentUserId;
                tenantDepartments.TenantId = CurrentTenantId;
                var filesName = Session["UploadTenantDepartmentImage"] as List<string>;
                if (filesName == null) { tenantDepartments.ImagePath = ""; }
                else
                {
                    if (UploadControl != null)
                    {
                        filePath = MoveFile(UploadControl.FirstOrDefault(), filesName.FirstOrDefault(), tenantDepartments.DepartmentId);
                        tenantDepartments.ImagePath = filePath;
                    }

                }

                _LookupService.UpdateTenantDepartment(tenantDepartments);
                return RedirectToAction("Index");
            }
            ViewBag.ControllerName = "TenantDepartment";
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountIDs = new SelectList(accounts, "AccountID", "AccountNameCode", tenantDepartments.AccountID);
            //ViewBag.TenantId = new SelectList(db.Tenants, "TenantId", "TenantName", tenantDepartments.TenantId);
            return View(tenantDepartments);
        }

        // GET: TenantDepartment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantDepartments tenantDepartments = _LookupService.GetTenantDepartmentById(id ?? 0);
            if (tenantDepartments == null)
            {
                return HttpNotFound();
            }
            return View(tenantDepartments);
        }

        // POST: TenantDepartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _LookupService.RemoveTenantDepartment(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["UploadTenantDepartmentImage"] == null)
            {
                Session["UploadTenantDepartmentImage"] = new List<string>();
            }
            var files = Session["UploadTenantDepartmentImage"] as List<string>;

            foreach (var file in UploadControl)
            {
               
                    SaveFile(file);
                    files.Add(file.FileName);
                
            }
            Session["UploadTenantDepartmentImage"] = files;

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
            Session["UploadTenantDepartmentImage"] = null;
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
