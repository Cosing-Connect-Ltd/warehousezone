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
using Ganedata.Core.Entities.Enums;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class ProductCodesController : BaseController
    {
        public IProductServices ProductService { get; }

        public ProductCodesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            ProductService = productService;
        }

        // GET: /SupplierCodes/Create
        public ActionResult Create(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productmaster = ProductService.GetProductMasterById(id);
            if (productmaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = id;
            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "SupplierID", "CompanyName");
            ViewBag.SupplierCodes = productmaster.ProductAccountCodes.Where(x => x.TenantId == CurrentTenantId && x.IsDeleted != true);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "AccountID,ProductId,ProdAccCode")] ProductAccountCodes ProductAccountCodes)
        {

            ProductMaster productmaster = ProductService.GetProductMasterById(ProductAccountCodes.ProductId ?? 0);
            if (productmaster == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {

                if (productmaster.ProductAccountCodes.All(s => s.AccountID != ProductAccountCodes.AccountID))
                {
                    ProductService.CreateProductAccountCodes(ProductAccountCodes, CurrentTenantId, CurrentUserId);

                    ViewBag.Msg = false;
                    return RedirectToAction("Create", new { id = ProductAccountCodes.ProductId });

                }
                else
                    ViewBag.Msg = true;
            }

            ViewBag.ProductId = ProductAccountCodes.ProductId;
            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "SupplierID", "CompanyName");
            ViewBag.SupplierCodes = productmaster.ProductAccountCodes.Where(x => x.TenantId == CurrentTenantId && x.IsDeleted != true);
            return View(ProductAccountCodes);

        }

        public ActionResult RemoveSC(int Sid, int Pid)
        {
            var isDeleted = ProductService.RemoveProductAccountCodes(Sid, Pid, CurrentTenantId, CurrentUserId);
            if (!isDeleted)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Create", new { id = Pid });

        }

    }
}

