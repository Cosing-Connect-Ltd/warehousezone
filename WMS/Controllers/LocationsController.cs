using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using WMS.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using Ganedata.Core.Entities.Enums;

namespace WMS.Controllers
{
    public class LocationsController : BaseController
    {
        private readonly IProductServices _productServices;

        public LocationsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
        }
        // GET: /Location/
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        // GET: /Location/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Locations locations = LookupServices.GetLocationById(id.Value, CurrentTenantId);
            if (locations == null)
            {
                return HttpNotFound();
            }
            return View(locations);
        }

        // GET: /Location/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Locations location, string ProductKit, List<int> ProductKitIds)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            // get properties of tenant
            var clocation = LookupServices.GetLocationByCode(location.LocationCode, CurrentTenantId);
            var cname = LookupServices.GetLocationByCode(location.LocationName, CurrentTenantId);

            if (!string.IsNullOrEmpty(ProductKit))
            {
                ProductKitIds = new List<int>();
                ProductKitIds = ProductKit.Split(',').Select(Int32.Parse).ToList();
            }
            if (clocation != null || cname != null)
            {
                if (clocation != null)
                    ModelState.AddModelError("", "Location Code already exists");

                if (cname != null)
                    ModelState.AddModelError("", "Location Name already exists");

                SetDropdowns();
                return View(location);
            }

            if (ModelState.IsValid)
            {
                LookupServices.CreateLocation(location, ProductKitIds, CurrentUserId, CurrentTenantId, CurrentWarehouseId);

                return RedirectToAction("Index");
            }


            return View(location);
        }

        public ActionResult BulkCreate()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            SetDropdowns();
            return View(new Locations { });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkCreate(Locations model, List<int> ProductIds, int? StartValue, int? EndValue)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (!(StartValue != null && EndValue != null))
            {
                ModelState.AddModelError("", "Start and End value must be provided");
                SetDropdowns();
                return View(model);
            }
            var codes = new List<string>();
            var names = new List<string>();

            for (var ctr = StartValue; ctr <= EndValue; ctr++)
            {
                codes.Add(model.LocationCode + ctr);
                names.Add(model.LocationName + ctr);
            }

            var ccodes = LookupServices.GetAllLocations(CurrentTenantId).Where(a => codes.Contains(a.LocationCode)).Select(a => a.LocationCode).Distinct().ToList();
            var nnames = LookupServices.GetAllLocations(CurrentTenantId).Where(a => names.Contains(a.LocationName)).Select(a => a.LocationName).Distinct().ToList();

            string cmsg = null;
            string nmsg = null;
            if (ccodes.Count > 0 || nnames.Count > 0)
            {
                if (ccodes.Count > 0)
                {
                    cmsg = "Following codes already exists</br>" + Environment.NewLine;
                    foreach (var cc in ccodes)
                    {
                        cmsg += cc + "</br>";
                    }
                    ModelState.AddModelError("", cmsg);
                }
                if (nnames.Count > 0)
                {
                    nmsg = "Following names already exists</br>" + Environment.NewLine;
                    foreach (var nn in nnames)
                    {
                        nmsg += nn + "</br>";
                    }
                    ModelState.AddModelError("", nmsg);
                }

                SetDropdowns();
                return View(model);
            }

            LookupServices.BulkCreateProductsLocation(model, ProductIds, StartValue, EndValue, CurrentTenantId, CurrentWarehouseId, CurrentUserId);

            return RedirectToAction("Index");
        }
        // GET: /Location/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Locations locations, string ProductKit, List<int> ProductKitIds)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (!string.IsNullOrEmpty(ProductKit))
            {
                ProductKitIds = new List<int>();
                ProductKitIds = ProductKit.Split(',').Select(Int32.Parse).ToList();
            }
            if (ModelState.IsValid)
            {
                LookupServices.BulkEditProductsLocation(locations, ProductKitIds, CurrentTenantId, CurrentWarehouseId, CurrentUserId);

                return RedirectToAction("Index");
            }
            ViewBag.LocationGroupId = new SelectList(LookupServices.GetAllValidLocationGroups(CurrentTenantId), "LocationGroupId", "Locdescription", locations.LocationGroupId);
            ViewBag.WarehouseId = new SelectList(LookupServices.GetAllWarehousesForTenant(CurrentTenantId), "WarehouseId", "WarehouseName", locations.WarehouseId);
            return View(locations.LocationId);
        }

        // GET: /Location/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var locations = LookupServices.GetLocationById(id ?? 0, CurrentTenantId);
            if (locations == null)
            {
                return HttpNotFound();
            }
            ViewBag.Products = locations.ProductLocationsMap.Where(a => a.IsDeleted != true).Select(p => p.ProductMaster.Name).ToList();

            return View(locations);
        }

        // POST: /Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            LookupServices.DeleteLocationById(id, CurrentTenantId, CurrentUserId);
            return RedirectToAction("Index");
        }

        public JsonResult _GetLocationGroups()
        {
            var data = LookupServices.GetAllValidLocationGroups(CurrentTenantId)
                        .Select(m => new
                        {
                            Id = m.LocationGroupId,
                            Description = m.Locdescription
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _LocationsList()
        {
            ViewBag.settingsName = "Locations";
            ViewBag.routeValues = new { Controller = "Locations", Action = "_LocationsList" };
            var locations = LookupServices.GetAllLocations(CurrentTenantId).ToList();
            return PartialView("_Locations", locations);
        }
        public ActionResult _LocationCreate(int? Id)
        {
            SetDropdowns();
            return PartialView("_LocationCreate", new Locations());
        }

        private void SetDropdowns()
        {
            var tenant = caCurrent.CurrentTenant();
            ViewBag.LocationGroups = LookupServices.GetAllValidLocationGroups(CurrentTenantId)
                .Select(m => new
                {
                    Id = m.LocationGroupId,
                    Group = m.Locdescription

                }).ToList();
            ViewBag.DimensionUOMs = (from duom in LookupServices.GetAllValidGlobalUoms(EnumUomType.Dimensions)
                                     select new
                                     {
                                         Id = duom.UOMId,
                                         DUOM = duom.UOM

                                     }).ToList();

            ViewBag.UOMs = (from duom in LookupServices.GetAllValidGlobalUoms(EnumUomType.Weight)
                            select new
                            {
                                Id = duom.UOMId,
                                UOM = duom.UOM

                            }).ToList();

            ViewBag.LocationTypes = LookupServices.GetAllValidLocationTypes(CurrentTenantId);
        }

        public ActionResult _Products(int LocationId)
        {
            ViewBag.locationid = LocationId;
            var model = LookupServices.GetProductsByLocationId(LocationId, CurrentTenantId).ToList();
            return PartialView(model);
        }
        public JsonResult _SetValue(int Id)
        {
            Session["LocId"] = Id;
            return Json(string.Empty);
        }

        public JsonResult IsLocationCodeAvailable(string locationCode, int locationid = 0)
        {
            if (locationid == 0)
            {
                return Json(LookupServices.GetLocationByCode(locationCode, CurrentTenantId) == null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(LookupServices.IsLocationCodeAvailable(locationCode, locationid, CurrentTenantId), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult IsLocationTypeAvailable(string LocTypeName, int LocationTypeId = 0)
        {
            if (LocationTypeId == 0)
            {
                return Json(LookupServices.GetLocationTypeByName(LocTypeName, CurrentTenantId), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(LookupServices.GetLocationTypeByName(LocTypeName, CurrentTenantId, LocationTypeId), JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult _LocationEdit(int? id)
        {
            SetDropdowns();

            if (id.HasValue)
                ViewBag.ProductIds = _productServices.GetAllProductsInALocationFromMaps(id.Value).ToList();

            var model = LookupServices.GetLocationById(id.Value, CurrentTenantId);

            return PartialView("_LocationEdit", model);
        }
        public ActionResult _LocationTypeCreate(int? Id)
        {
            if (Id == null)
                return PartialView();
            else
            {
                var model = LookupServices.GetLocationTypeById(Id.Value);
                return PartialView(model);
            }

        }
        public ActionResult _LocationTypeSubmit(LocationTypes model)
        {
            if (model.LocationTypeId == 0)
            {
                var item = LookupServices.GetLocationTypeByName(model.LocTypeName, CurrentTenantId);
                if (item != null)
                    return Json(new { error = true, msg = "Location already exists" });

                var locationType = LookupServices.CreateLocationType(model.LocTypeName, model.LocTypeDescription, CurrentTenantId, CurrentUserId);
                if (locationType != null)
                {
                    model.LocationTypeId = locationType.LocationTypeId;
                }
            }
            return Json(new { error = false, id = model.LocationTypeId, type = model.LocTypeName });
        }
        public ActionResult _LocationGroupCreate(int? Id)
        {
            if (Id == null)
                return PartialView();
            else
            {
                var model = LookupServices.GetLocationGroupById(Id.Value);
                return PartialView(model);
            }

        }
        public JsonResult IsLocationGroupAvailable(string Locdescription, int LocationGroupId = 0)
        {
            return Json(LookupServices.GetLocationGroupByName(Locdescription, CurrentTenantId, LocationGroupId) == null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _LocationGroupSubmit(LocationGroup model)
        {
            LocationGroup addedLocationGroup = new LocationGroup();

            if (model.LocationGroupId == 0)
            {
                var item = LookupServices.GetLocationGroupByName(model.Locdescription, CurrentTenantId);
                if (item != null)
                    return Json(new { error = true, msg = "Location group already exists" });

                addedLocationGroup = LookupServices.CreateLocationGroup(model.Locdescription, CurrentTenantId, CurrentUserId);
            }
            return Json(new { error = false, id = addedLocationGroup.LocationGroupId, type = addedLocationGroup.Locdescription });
        }
    }
}
