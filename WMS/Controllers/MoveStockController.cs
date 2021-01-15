using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using WMS.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class MoveStockController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly ICommonDbServices _commonServices;

        public MoveStockController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, ICommonDbServices commonServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _commonServices = commonServices;
        }
        // GET: MoveStock
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        public ActionResult _StockListByLocation()
        {
            return PartialView(_commonServices.ProductsByLocations(CurrentTenantId, CurrentWarehouseId));
        }

        public ActionResult _StockListByLocationDetails(int locationId)
        {
            ViewBag.LocationId = locationId;
            return PartialView(_commonServices.ProductsByLocationDetails(locationId));
        }

        public ActionResult _MoveStock()
        {
            int? id = int.Parse(Request.Params["Id"]);
            var transaction = _productServices.GetInventoryTransactionById(id.Value);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.ProductMaster.Serialisable)
                transaction.Quantity = (int)_commonServices.GetQuantityInLocation(transaction);
            ViewBag.Locations = new SelectList(LookupServices.GetAllLocations(CurrentTenantId)
                .Where(a => a.IsDeleted != true &&
                a.WarehouseId == transaction.WarehouseId &&
                a.TenantId == transaction.TenentId && a.LocationId != transaction.LocationId), "LocationId", "LocationCode");
            return PartialView(transaction);
        }
        public ActionResult _MoveStockSubmit(InventoryTransaction model)
        {
            _productServices.MoveStockBetweenLocations(model.InventoryTransactionId, model.LocationId, model.Quantity,
                CurrentTenantId, CurrentWarehouseId, CurrentUserId);
            return Json(new { error = false });
        }
    }
}