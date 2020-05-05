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
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductAllowancesController : BaseController
    {
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;
        private readonly IRolesServices _RolesServices;
        // GET: WebsiteNavigations

        public ProductAllowancesController(ICoreOrderService orderService, IRolesServices RolesServices, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, ITenantWebsiteService tenantWebsiteService)
        : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _tenantWebsiteService = tenantWebsiteService;
            _RolesServices = RolesServices;
        }

        // GET: ProductAllowances
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _ProductAllowanceList()
        {
            var data = _tenantWebsiteService.GetAllValidProductAllowance(CurrentTenantId).ToList();
            return PartialView(data);
        }


        // GET: ProductAllowances/Create
        public ActionResult Create()
        {
            ViewBag.RolesId = new SelectList(_RolesServices.GetAllRoles(CurrentTenantId), "Id", "RoleName");
            ViewBag.AllowanceGroupId = new SelectList(_tenantWebsiteService.GetAllValidProductAllowanceGroups(CurrentTenantId), "Id", "Name");
            return View();
        }

        // POST: ProductAllowances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductAllowance productAllowance)
        {
            if (ModelState.IsValid)
            {
                _tenantWebsiteService.CreateOrUpdateProductAllowance(productAllowance,CurrentUserId,CurrentTenantId);
                return RedirectToAction("Index");
            }
            ViewBag.RolesId = new SelectList(_RolesServices.GetAllRoles(CurrentTenantId), "Id", "RoleName", productAllowance.RolesId);
            ViewBag.AllowanceGroupId = new SelectList(_tenantWebsiteService.GetAllValidProductAllowanceGroups(CurrentTenantId), "Id", "Name", productAllowance.AllowanceGroupId);
         
           
            return View(productAllowance);
        }

        // GET: ProductAllowances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductAllowance productAllowance = _tenantWebsiteService.GetProductAllowanceById(id ?? 0);
            if (productAllowance == null)
            {
                return HttpNotFound();
            }
            ViewBag.RolesId = new SelectList(_RolesServices.GetAllRoles(CurrentTenantId), "Id", "RoleName", productAllowance.RolesId);
            ViewBag.AllowanceGroupId = new SelectList(_tenantWebsiteService.GetAllValidProductAllowanceGroups(CurrentTenantId), "Id", "Name", productAllowance.AllowanceGroupId);

            return View(productAllowance);
        }

        // POST: ProductAllowances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductAllowance productAllowance)
        {
            if (ModelState.IsValid)
            {
                _tenantWebsiteService.CreateOrUpdateProductAllowance(productAllowance, CurrentUserId, CurrentTenantId);
                 return RedirectToAction("Index");
            }
            ViewBag.RolesId = new SelectList(_RolesServices.GetAllRoles(CurrentTenantId), "Id", "RoleName", productAllowance.RolesId);
            ViewBag.AllowanceGroupId = new SelectList(_tenantWebsiteService.GetAllValidProductAllowanceGroups(CurrentTenantId), "Id", "Name", productAllowance.AllowanceGroupId);

            return View(productAllowance);
        }

        // GET: ProductAllowances/Delete/5
        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var websiteNav = _tenantWebsiteService.RemoveWebsiteDiscountCodes(id ?? 0, CurrentUserId);
            return RedirectToAction("Index");
        }

    }
}
