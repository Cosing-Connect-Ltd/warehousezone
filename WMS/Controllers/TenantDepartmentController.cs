using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
                var departments = _LookupService.SaveTenantDepartment(tenantDepartments.DepartmentName,tenantDepartments.AccountID, CurrentUserId, CurrentTenantId);
                string filePath = "";
                foreach (var file in files)
                {
                    filePath = MoveFile(file, filesName.FirstOrDefault(), departments.DepartmentId);
                    departments.ImagePath = filePath;
                    _LookupService.UpdateTenantDepartment(departments);
                    break;
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
            TenantDepartments tenantDepartments = _LookupService.GetTenantDepartmentById(id??0);
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountIDs = new SelectList(accounts, "AccountID", "AccountNameCode",tenantDepartments?.AccountID);
            if (tenantDepartments == null)
            {
                return HttpNotFound();
            }
            //ViewBag.TenantId = new SelectList(db.Tenants, "TenantId", "TenantName", tenantDepartments.TenantId);
            return View(tenantDepartments);
        }

        // POST: TenantDepartment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName,TenantId,AccountID,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantDepartments tenantDepartments, IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (ModelState.IsValid)
            {
                tenantDepartments.DepartmentId = tenantDepartments.DepartmentId;
                tenantDepartments.CreatedBy = CurrentUserId;
                tenantDepartments.TenantId = CurrentTenantId;

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
            _LookupService.RemoveTenantDepartment(id,CurrentUserId);
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
                var fileToken = Guid.NewGuid().ToString();
                var ext = new FileInfo(file.FileName).Extension;
                var fileName = fileToken + ext;
                files.Add(file.FileName);
            }
            Session["UploadTenantDepartmentImage"] = files;

            return Content("true");
        }
        private string MoveFile(DevExpress.Web.UploadedFile file, string FileName, int ProductGroupId)
        {
            Session["UploadTenantDepartmentImage"] = null;
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductGroupId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductGroupId.ToString()));
            string resFileName = Server.MapPath(UploadDirectory + ProductGroupId.ToString() + @"/" + FileName);
            file.SaveAs(resFileName);
            return resFileName;
        }


    }
}
