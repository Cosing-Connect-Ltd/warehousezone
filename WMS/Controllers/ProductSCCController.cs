using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using WMS;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class ProductSCCController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLocationService;

        public ProductSCCController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLocationService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _productLocationService = productLocationService;
        }

        public ActionResult Create(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMaster productmaster = _productServices.GetProductMasterById(id);

            if (productmaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = id;
            ViewBag.SCC = from c in productmaster.ProductSCCCodes where (c.IsDeleted!=true) select c   ;
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,SCC,Quantity")] ProductSCCCodes productscccodes)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(productscccodes.SCC)) productscccodes.SCC = productscccodes.SCC.Trim();

                _productServices.AddProductSccCodes(productscccodes, CurrentTenantId, CurrentUserId);
             
                return RedirectToAction("Create", new { id = productscccodes.ProductId });
            }

            ProductMaster productmaster = _productServices.GetProductMasterById(productscccodes.ProductId);
            if (productmaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.SCC = productmaster.ProductSCCCodes.Where(m=> m.IsDeleted!=true);
            
           return View(productscccodes);
        }


        public ActionResult RemoveSCC(int id = 0, int ProductId=0)
        {
            if (id == 0 || ProductId==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _productServices.RemoveProductSccCodes(ProductId, id, CurrentTenantId, CurrentUserId);

            return RedirectToAction("Create", new { id = ProductId });
        }
         
    }
}