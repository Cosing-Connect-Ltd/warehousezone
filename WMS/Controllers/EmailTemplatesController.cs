using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class EmailTemplatesController : BaseController
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly EmailServices _emailServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;


        public EmailTemplatesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices,
            IEmailNotificationService emailNotificationService, EmailServices emailServices,ITenantWebsiteService tenantWebsiteService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _emailNotificationService = emailNotificationService;
            _emailServices = emailServices;
            _tenantWebsiteService = tenantWebsiteService;
        }
        // GET: /EmailTemplates/
        public ActionResult Index()
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }


        // GET: /EmailTemplates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var emailtemplates = _emailNotificationService.GetTenantEmailTemplateById(id ?? 0, CurrentTenantId);
            if (emailtemplates == null)
            {
                return HttpNotFound();
            }
            return View(emailtemplates);
        }

        // POST: /EmailTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _emailNotificationService.RemoveTenantEmailTemplateById(id, CurrentTenantId);
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult EmailTemplatesPartial()
        {
            // get properties of tenant
            var tenant = caCurrent.CurrentTenant();
            var model = _emailNotificationService.GetAllTenantEmailTemplates(tenant.TenantId);
            return PartialView("_EmailTemplates", model.ToList());
        }

        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession())
            {
                return Redirect((string)Session["ErrorUrl"]);


            }

            ViewBag.Sites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteID",
                "SiteName");
            return View("Create", new TenantEmailTemplates());
        }

        [HttpPost]
        public ActionResult Create(TenantEmailTemplates template)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                template.TenantId = CurrentTenantId;
                template.CreatedBy = CurrentUserId;
                template.DateCreated = DateTime.UtcNow;

                _emailNotificationService.CreateEmailEmailTemplate(template);
                return RedirectToAction("Index");

            }
            ViewBag.Sites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteID",
                "SiteName");
            return View(template);



        }

        // GET: /EmailTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var template = _emailNotificationService.GetTenantEmailTemplateById(id ?? 0, CurrentTenantId);
            if (template == null)
            {
                return HttpNotFound();
            }

            ViewBag.Sites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteID",
                "SiteName", template.SiteId);
            return View(template);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TenantEmailTemplates emailtemp)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                emailtemp.TenantId = CurrentTenantId;
                emailtemp.UpdatedBy = CurrentUserId;
                emailtemp.DateUpdated = DateTime.UtcNow;
                _emailNotificationService.SaveEmailEmailTemplate(emailtemp);

                return RedirectToAction("Index");
            }
            ViewBag.Sites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteID",
                "SiteName");
            return View(emailtemp);
        }

    }
}
