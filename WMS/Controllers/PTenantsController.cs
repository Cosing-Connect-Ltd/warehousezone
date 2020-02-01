using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class PTenantsController : BaseController
    {
        public IWarehouseSyncService SyncService { get; }

        public PTenantsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IWarehouseSyncService syncService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            SyncService = syncService;
        }

        // GET: PTenants
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            
            return View();
        }
        
        public ActionResult PTenantsGridview(int? id, bool fragment=false)
        {
            ViewBag.PropertyId = id;
            if (id.HasValue && !fragment)
            {

                ViewBag.DisableCallBacks = true;
            }


            var viewModel = GridViewExtension.GetViewModel("_PTenantsListGridview");

            if (viewModel == null)
                viewModel = PTenantCustomBinding.CreatePTenantGridViewModel();

            return PTenantsGridActionCore(viewModel,id);

        }
        public ActionResult PTenantsGridActionCore(GridViewModel gridViewModel, int? id = null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PTenantCustomBinding.PTenantGetDataRowCount(args, id);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PTenantCustomBinding.PTenantGetData(args, id);
                    })
            );
            return PartialView("_PTenantsListGridview", gridViewModel);
        }

        public ActionResult _PTenantsGridViewsPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview" + ViewBag.PropertyId ?? "");
            viewModel.Pager.Assign(pager);
            return PTenantsGridActionCore(viewModel);
        }

        public ActionResult _PTenantsGridViewFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview" + ViewBag.PropertyId ?? "");
            viewModel.ApplyFilteringState(filteringState);
            return PTenantsGridActionCore(viewModel);
        }
        public ActionResult _PTenantsGridViewDataSorting(GridViewColumnState column, bool reset)
        {

            var viewModel = GridViewExtension.GetViewModel("PPropertiesGridview" + ViewBag.PropertyId ?? "");
            viewModel.ApplySortingState(column, reset);
            return PTenantsGridActionCore(viewModel);
        }



        // GET: PTenants/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PTenant pTenant = PropertyService.GetPropertyTenantById(id ?? 0);
            if (pTenant == null)
            {
                return HttpNotFound();
            }
            return View(pTenant);
        }

        // GET: PTenants/Create
        public ActionResult Create(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var properties = PropertyService.GetAllValidProperties().ToList().Select(m => new SelectListItem() { Text = m.FullAddress + "(" + m.PropertyCode + ")", Value = m.PPropertyId.ToString() }).ToList();
            ViewBag.CurrentPropertyId = new SelectList(properties, "Value", "Text", id);
            ViewBag.ReturnUrl = Url.Action("Index", "PTenants", new { id = id });
            return View();
        }

        // POST: PTenants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PTenantId,TenantCode,TenantYCode,TenantFullName,TenantSalutation,TenancyStatus,TenancyCategory,TenancyAdded,TenancyStarted,TenancyRenewDate,TenancyPeriodMonths,SiteId,SyncRequiredFlag,CurrentPropertyCode,AddressLine1,AddressLine2,AddressLine3,AddressLine4,AddressPostcode,HomeTelephone,WorkTelephone1,WorkTelephone2,WorkTelephoneFax,MobileNumber,Email,CurrentPropertyId")] PTenant pTenant, FormCollection form)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                PropertyService.CreatePropertyTenant(pTenant, CurrentUserId);
                if (form["ReturnUrl"] != null)
                {
                    return Redirect(form["ReturnUrl"]);
                }
                return RedirectToAction("Index");
            }

            var properties = PropertyService.GetAllValidProperties().Select(m => new SelectListItem() { Text = m.FullAddress + "(" + m.PropertyCode + ")", Value = m.PPropertyId.ToString() }).ToList();
            ViewBag.CurrentPropertyId = new SelectList(properties, "Value", "Text", pTenant.CurrentPropertyId);

            return View(pTenant);
        }

        // GET: PTenants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PTenant pTenant = PropertyService.GetPropertyTenantById(id ?? 0);
            if (pTenant == null)
            {
                return HttpNotFound();
            }
            var properties = PropertyService.GetAllValidProperties().Select(m => new SelectListItem() { Text = m.FullAddress + "(" + m.PropertyCode + ")", Value = m.PPropertyId.ToString() }).ToList();
            ViewBag.CurrentPropertyId = new SelectList(properties, "Value", "Text", pTenant.CurrentPropertyId);

            return View(pTenant);
        }

        // POST: PTenants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PTenant pTenant)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (ModelState.IsValid)
            {
                PropertyService.UpdatePropertyTenant(pTenant, CurrentUserId);
                return RedirectToAction("Index");
            }
            var properties = PropertyService.GetAllValidProperties().Select(m => new SelectListItem() { Text = m.FullAddress + "(" + m.PropertyCode + ")", Value = m.PPropertyId.ToString() }).ToList();
            ViewBag.CurrentPropertyId = new SelectList(properties, "Value", "Text", pTenant.CurrentPropertyId);

            return View(pTenant);
        }

        // GET: PTenants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var pTenant = PropertyService.GetPropertyTenantById(id ?? 0);
            if (pTenant == null)
            {
                return HttpNotFound();
            }
            return View(pTenant);
        }

        // POST: PTenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            PropertyService.DeletePropertyTenant(id);
            return RedirectToAction("Index");
        }

        public ActionResult UpdateTenantCurrentProperties()
        {
            SyncService.UpdateTenantCurrentProperties();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
