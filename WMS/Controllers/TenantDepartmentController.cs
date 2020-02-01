using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public ActionResult Create([Bind(Include = "DepartmentId,DepartmentName,TenantId,AccountID,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantDepartments tenantDepartments)
        {
            if (ModelState.IsValid)
            {
                _LookupService.SaveTenantDepartment(tenantDepartments.DepartmentName,tenantDepartments.AccountID, CurrentUserId, CurrentTenantId);
                    
                return RedirectToAction("Index");
            }

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
        public ActionResult Edit([Bind(Include = "DepartmentId,DepartmentName,TenantId,AccountID,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantDepartments tenantDepartments)
        {
            if (ModelState.IsValid)
            {
                tenantDepartments.DepartmentId = tenantDepartments.DepartmentId;
                tenantDepartments.CreatedBy = CurrentUserId;
                tenantDepartments.TenantId = CurrentTenantId;

                _LookupService.UpdateTenantDepartment(tenantDepartments);
                return RedirectToAction("Index");
            }
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

       
    }
}
