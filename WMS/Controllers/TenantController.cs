using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class TenantController : BaseController
    {
        private readonly ITenantsServices _tenantServices;

        public TenantController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantsServices tenantServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantServices = tenantServices;
        }


        // GET: /Tenant/
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        // GET: /Tenant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var tenant = _tenantServices.GetByClientId((int)id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }



        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
           
            var cntries = (from cntry in LookupServices.GetAllGlobalCountries()
                           select new
                           {
                               CountryId = cntry.CountryID,
                               CountryName = cntry.CountryName + "-" + cntry.CountryCode

                           }).ToList();
            ViewBag.Countries = new SelectList(cntries.OrderBy(o => o.CountryId), "CountryID", "CountryName");
            ViewBag.Currencies = new MultiSelectList(LookupServices.GetAllGlobalCurrencies().OrderBy(o => o.CurrencyID), "CurrencyId", "CurrencyName");
            return View();
        }

        // POST: /Tenant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenantId,TenantName,CurrencyID,CountryID,AccountNumber,ProductCodePrefix,TenantCulture,TenantTimeZoneId,TenantNo,TenantVatNo,TenantAccountReference,TenantWebsite,TenantDayPhone,TenantEveningPhone,TenantMobilePhone,TenantFax,TenantEmail,TenantAddress1,TenantAddress2,TenantAddress3,TenantAddress4,TenantStateCounty,TenantPostalCode,TenantCity,TenantSubDmoain,IsActive")] Tenant tenants)
        {
            

            tenants.CreatedBy = caCurrent.CurrentUser().UserId;
            tenants.DateCreated = DateTime.UtcNow;
            _tenantServices.Add(tenants);

            return RedirectToAction("Index");
        

            
           
        }




        // GET: /Tenant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var tenant = _tenantServices.GetByClientId((int)id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            var cntries = (from cntry in LookupServices.GetAllGlobalCountries()
                           select new
                           {
                               CountryId = cntry.CountryID,
                               CountryName = cntry.CountryName + "-" + cntry.CountryCode

                           }).ToList();
            ViewBag.Countries = new SelectList(cntries.OrderBy(o => o.CountryId), "CountryID", "CountryName");
            ViewBag.Currencies = new MultiSelectList(LookupServices.GetAllGlobalCurrencies().OrderBy(o => o.CurrencyID), "CurrencyId", "CurrencyName");
            return View(tenant);
        }

        // POST: /Tenant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TenantId,TenantName,CurrencyID,CountryID,AccountNumber,ProductCodePrefix,TenantCulture,TenantTimeZoneId,TenantNo,TenantVatNo,TenantAccountReference,TenantWebsite,TenantDayPhone,TenantEveningPhone,TenantMobilePhone,TenantFax,TenantEmail,TenantAddress1,TenantAddress2,TenantAddress3,TenantAddress4,TenantStateCounty,TenantPostalCode,TenantCity,TenantSubDmoain,IsActive")] Tenant tenants)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();

                // get properties of user
                caUser user = caCurrent.CurrentUser();

                tenants.DateUpdated = DateTime.UtcNow;
                tenants.UpdatedBy = user.UserId;

                _tenantServices.Update(tenants);

                return RedirectToAction("Index");
            }
            return View(tenants);
        }
         

       
        [ValidateInput(false)]
        public ActionResult _TenantList()
        {

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            var model = _tenantServices.GetAllClients(tenant.TenantId).ToList(); ;
            return PartialView("__TenantList", model);
        }
    }

}