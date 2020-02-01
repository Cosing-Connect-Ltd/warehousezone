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
    public class TenantEmailConfigsController:Controller
    {
        private ITenantEmailConfigServices _tenantEmailConfigServices;

        public TenantEmailConfigsController(ITenantEmailConfigServices tenantEmailConfigServices,ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices)
        {
            //OrderService = orderService;
            //PropertyService = propertyService;
            //AccountServices = accountServices;
            //LookupServices = lookupServices;
            _tenantEmailConfigServices = tenantEmailConfigServices;

        }

        // GET: TenantEmailConfigs
        public ActionResult Index()
        {
            var tenantEmailConfigs = _tenantEmailConfigServices.GetAllEmialConfigByTenant(caCurrent.CurrentTenant().TenantId);
            return View(tenantEmailConfigs);
        }

        // GET: TenantEmailConfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantEmailConfig tenantEmailConfig = _tenantEmailConfigServices.GetEmailConfigById(id??0);
            if (tenantEmailConfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantEmailConfig);
        }

        // GET: TenantEmailConfigs/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: TenantEmailConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenantEmailConfigId,SmtpHost,SmtpPort,UserEmail,Password,EnableSsl,IsActive,DailyEmailDispatchTime,EnableRelayEmailServer,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantEmailConfig tenantEmailConfig)
        {
            if (ModelState.IsValid)
            {
                _tenantEmailConfigServices.SaveEmailConfig(tenantEmailConfig,caCurrent.CurrentUser().UserId, caCurrent.CurrentTenant().TenantId);
                
                return RedirectToAction("Index");
            }

          
            return View(tenantEmailConfig);
        }

        // GET: TenantEmailConfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantEmailConfig tenantEmailConfig = _tenantEmailConfigServices.GetEmailConfigById(id??0);
            if (tenantEmailConfig == null)
            {
                return HttpNotFound();
            }
          
            return View(tenantEmailConfig);
        }

        // POST: TenantEmailConfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TenantEmailConfigId,SmtpHost,SmtpPort,UserEmail,Password,EnableSsl,IsActive,DailyEmailDispatchTime,EnableRelayEmailServer,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantEmailConfig tenantEmailConfig)
        {
            if (ModelState.IsValid)
            {
                _tenantEmailConfigServices.UpdateEmailConfig(tenantEmailConfig, caCurrent.CurrentUser().UserId, caCurrent.CurrentTenant().TenantId);
                return RedirectToAction("Index");
            }
           
            return View(tenantEmailConfig);
        }

        // GET: TenantEmailConfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantEmailConfig tenantEmailConfig = _tenantEmailConfigServices.GetEmailConfigById(id??0);
            if (tenantEmailConfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantEmailConfig);
        }

        // POST: TenantEmailConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TenantEmailConfig tenantEmailConfig = _tenantEmailConfigServices.GetEmailConfigById(id);
            _tenantEmailConfigServices.DeleteEmailConfig(tenantEmailConfig, caCurrent.CurrentUser().UserId);
             return RedirectToAction("Index");
        }
        public ActionResult TenantEmailConfigGridViewPartial()
        {
            var tennatConfig = _tenantEmailConfigServices.GetAllEmialConfigByTenant(caCurrent.CurrentTenant().TenantId);
            return PartialView("_TenantEmailConfigGridViewPartial", tennatConfig.ToList());
        }
    }
}
