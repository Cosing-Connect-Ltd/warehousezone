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
    public class TenantEmailConfigController : BaseController
    {
        private readonly ITenantEmailConfigServices _tenantEmailConfigService;

        public TenantEmailConfigController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantEmailConfigServices tenantEmailConfigService) 
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantEmailConfigService = tenantEmailConfigService;
        }
        // GET: /TenantEmailConfig/
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            return View(_tenantEmailConfigService.GetAllEmialConfigByTenant(tenant.TenantId));
        }

        // GET: /TenantEmailConfig/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TenantEmailConfig tenantemailconfig = _tenantEmailConfigService.GetEmailConfigById((int)id);

            if (tenantemailconfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantemailconfig);
        }

        // GET: /TenantEmailConfig/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        // POST: /TenantEmailConfig/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="TenantEmailConfigId,TenantId,SmtpHost,SmtpPort,UserEmail,Password,EnableSsl,DateUpdated,UpdatedBy,IsActive")] TenantEmailConfig tenantemailconfig)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _tenantEmailConfigService.SaveEmailConfig(tenantemailconfig, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }

            return View(tenantemailconfig);
        }

        // GET: /TenantEmailConfig/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantEmailConfig tenantemailconfig = _tenantEmailConfigService.GetEmailConfigById((int)id);
            if (tenantemailconfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantemailconfig);
        }

        // POST: /TenantEmailConfig/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="TenantEmailConfigId,TenantId,SmtpHost,SmtpPort,UserEmail,Password,EnableSsl,DateUpdated,UpdatedBy,IsActive")] TenantEmailConfig tenantemailconfig)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _tenantEmailConfigService.UpdateEmailConfig(tenantemailconfig, user.UserId, tenant.TenantId);
     
                return RedirectToAction("Index");
            }
            return View(tenantemailconfig);
        }

        // GET: /TenantEmailConfig/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantEmailConfig tenantemailconfig = _tenantEmailConfigService.GetEmailConfigById((int)id);
            if (tenantemailconfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantemailconfig);
        }

        // POST: /TenantEmailConfig/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // get properties of user
            caUser user = caCurrent.CurrentUser();

            TenantEmailConfig tenantemailconfig = _tenantEmailConfigService.GetEmailConfigById((int)id);
            _tenantEmailConfigService.DeleteEmailConfig(tenantemailconfig, user.UserId);
           
            return RedirectToAction("Index");
        }
         
    }
}
