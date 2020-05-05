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
    public class AuthUserGroupsController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        // GET: WebsiteNavigations

        public AuthUserGroupsController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
        }

        // GET: AuthUserGroups
        public ActionResult Index()
        {
            return View();
        }

        // GET: AuthUserGroups/List/5
        public ActionResult _authUsersGroupsList()
        {
            var data = _userService.GetAllAuthUserGroups(CurrentTenantId).ToList();
            return PartialView(data);
        }

        //// GET: AuthUserGroups/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            return View();
        }

        // POST: AuthUserGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuthUserGroups authUserGroups)
        {
            if (ModelState.IsValid)
            {
                _userService.CreateOrUpdateAuthUserGroup(authUserGroups, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            return View(authUserGroups);
        }

        //// GET: AuthUserGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuthUserGroups authUserGroups = _userService.GetUserGroupsById(id??0);
            if (authUserGroups == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", authUserGroups.CountryId);
            return View(authUserGroups);
        }

        //// POST: AuthUserGroups/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuthUserGroups authUserGroups)
        {
            if (ModelState.IsValid)
            {
                _userService.CreateOrUpdateAuthUserGroup(authUserGroups, CurrentUserId, CurrentTenantId);
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(_lookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", authUserGroups.CountryId);
            return View(authUserGroups);
        }

        //// GET: AuthUserGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            _userService.RemoveUserGroupsById(id ?? 0,CurrentUserId);
            return RedirectToAction("Index");
         
        }



    }
}
