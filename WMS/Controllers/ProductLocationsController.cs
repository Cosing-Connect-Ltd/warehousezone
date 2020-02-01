using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductLocationsController : BaseController
    {
        private readonly IProductServices _productServices;

        public ProductLocationsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLocationService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
        }
        public ActionResult Index(int id = 0)
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

            var warehouses = LookupServices.GetAllWarehousesForTenant(CurrentTenantId);

            ViewBag.Warehouse = new SelectList(warehouses, "WarehouseId", "WarehouseName");

            try
            {
                ViewBag.Locations = new SelectList(warehouses.FirstOrDefault().Locations, "LocationId", "LocationCode");
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while getting Product Locations - " + ex.Message.ToString(), ex.InnerException);
            }

            return View(productmaster);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLocation(int ProductId = 0, int Locations = 0)
        {
            if (_productServices.AddProductLocationMap(ProductId, Locations) == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Index", new { id = ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLocationlist(int?[] locations, int ProductId = 0)
        {
            if (_productServices.AddProductLocationMaps(ProductId, locations)==null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", new { id = ProductId });
        }
    
        public ActionResult DeleteLocation(int LocationId, int ProductId)
        {
            if (!_productServices.RemoveProductLocationMap(ProductId, LocationId))
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", new { id = ProductId });
        }


        public ActionResult JsongetLocationlist(long ProductId, int id, string q)
        {
            try
            {
                var result = _productServices.GetProductLocationList(ProductId, id, q);

                if (result == null)
                {
                    return HttpNotFound();
                }

                return Json(result.Select(m=> new { LocationId= m.Key, LocationCode = m.Value }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, isValid = false, isException = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult JsongetLocations(int id)
        {
            try
            {
                var warehouseLocationsList = _productServices.GetWarehouseLocationList(id);

                return Json(warehouseLocationsList.Select(m => new { LocationId = m.Key, LocationCode = m.Value }).ToList(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, isValid = false, isException = true }, JsonRequestBehavior.AllowGet);
            }
             
        }
         
    }
}