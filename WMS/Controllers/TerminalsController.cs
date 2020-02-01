using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class TerminalsController : BaseController
    {
        private readonly ITerminalServices _terminalServices;
        private readonly ITenantLocationServices _tenantLocationServices;

        public TerminalsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _terminalServices = terminalServices;
            _tenantLocationServices = tenantLocationServices;
        }
        // GET: Terminals
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }



        // GET: Terminals/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminals terminals = _terminalServices.GetTerminalById((int)id);
            if (terminals == null)
            {
                return HttpNotFound();
            }
            return View(terminals);
        }

        // GET: Terminals/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get current warehouse
            TenantLocations Warehouse = caCurrent.CurrentWarehouse();

            ViewBag.WarehouseId = new SelectList(_tenantLocationServices.GetTenantLocationListById(Warehouse.WarehouseId, tenant.TenantId), "WarehouseId", "WarehouseName", Warehouse.WarehouseId);
            return View();
        }

        // POST: Terminals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TerminalId,TerminalName,TermainlSerial,WarehouseId,IsActive")] Terminals terminals)
        {
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            // get current warehouse
            TenantLocations Warehouse = caCurrent.CurrentWarehouse();

            if (ModelState.IsValid)
            {
                _terminalServices.SaveTerminal(terminals, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }

            ViewBag.WarehouseId = new SelectList(_tenantLocationServices.GetTenantLocationListById(Warehouse.WarehouseId, tenant.TenantId), "WarehouseId", "WarehouseName", terminals.WarehouseId);
            return View(terminals);
        }

        // GET: Terminals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get current warehouse
            TenantLocations Warehouse = caCurrent.CurrentWarehouse();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminals terminals = _terminalServices.GetTerminalById((int)id);
            if (terminals == null)
            {
                return HttpNotFound();
            }

            ViewBag.WarehouseId = new SelectList(_tenantLocationServices.GetTenantLocationListById(Warehouse.WarehouseId, tenant.TenantId), "WarehouseId", "WarehouseName", terminals.WarehouseId);
            return View(terminals);

        }

        // POST: Terminals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TerminalId,TerminalName,TermainlSerial,WarehouseId,IsActive")] Terminals terminals)
        {
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            // get current warehouse
            TenantLocations Warehouse = caCurrent.CurrentWarehouse();

            if (ModelState.IsValid)
            {
                Terminals newTerminal = _terminalServices.GetTerminalById(terminals.TerminalId);

                newTerminal.TerminalName = terminals.TerminalName;
                newTerminal.TermainlSerial = terminals.TermainlSerial;
                newTerminal.WarehouseId = terminals.WarehouseId;
                newTerminal.IsActive = terminals.IsActive;

                _terminalServices.UpdateTerminal(newTerminal, user.UserId, tenant.TenantId);
                return RedirectToAction("Index");
            }
            ViewBag.WarehouseId = new SelectList(_tenantLocationServices.GetTenantLocationListById(Warehouse.WarehouseId, tenant.TenantId), "WarehouseId", "WarehouseName", terminals.WarehouseId);
            return View(terminals);
        }

        // GET: Terminals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Terminals terminals = _terminalServices.GetTerminalById((int)id);
            if (terminals == null)
            {
                return HttpNotFound();
            }
            return View(terminals);
        }

        // POST: Terminals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Terminals terminals = _terminalServices.GetTerminalById((int)id);

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _terminalServices.DeleteTerminal(terminals, user.UserId);
            return RedirectToAction("Index");
        }


        //check if Serial No already exist in database
        public JsonResult IsSerialAvailable(string TermainlSerial, int TerminalId = 0)
        {
            if (!String.IsNullOrEmpty(TermainlSerial)) TermainlSerial = TermainlSerial.Trim();

            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            int result = 0;

            if (TerminalId == 0)
            {
                result = _terminalServices.GetTerminalCountBySerial(TermainlSerial, tenant.TenantId);
            }
            else
            {
                result = _terminalServices.GetTerminalCountBySerialNotEqualId(TermainlSerial, TerminalId, tenant.TenantId);
            }



            if (result > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }

        //[ValidateInput(false)]
        //public ActionResult _TerminalList()
        //{
        //    // get properties of current tenant
        //    caTenant tenant = caCurrent.CurrentTenant();

        //    // get properties of user
        //    caUser user = caCurrent.CurrentUser();

        //    //get current warehouse
        //    TenantLocations warehouse = caCurrent.CurrentWarehouse();

        //    var model = _terminalServices.GetAllTerminalsForGrid(tenant.TenantId, warehouse.WarehouseId).ToList();
        //    return PartialView("__TerminalList", model);

        //}

        public ActionResult _TerminalList()
        {


            var viewModel = GridViewExtension.GetViewModel("Terminals");

            if (viewModel == null)
                viewModel = TerminalListCustomBinding.CreateTerminalListGridViewModel();

            return _TerminalListGridActionCore(viewModel);




        }
        public ActionResult _TerminalListPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("Terminals");
            viewModel.Pager.Assign(pager);
            return _TerminalListGridActionCore(viewModel);
        }


        public ActionResult _TerminalListFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("Terminals");

            viewModel.ApplyFilteringState(filteringState);
            return _TerminalListGridActionCore(viewModel);
        }

        public ActionResult _TerminalListSorting(GridViewColumnState column, bool reset)
        {

            var viewModel = GridViewExtension.GetViewModel("Terminals");
            viewModel.ApplySortingState(column, reset);
            return _TerminalListGridActionCore(viewModel);
        }


        public ActionResult _TerminalListGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(new GridViewCustomBindingGetDataRowCountHandler(args => { TerminalListCustomBinding.TerminalListGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId); }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        TerminalListCustomBinding.TerminalListGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_TerminalList", gridViewModel);
        }

        [ValidateInput(false)]
        public ActionResult _TermianlLog(int id)
        {

            ViewData["id"] = id;
            Session["terminalId"] = id;
            var viewModel = GridViewExtension.GetViewModel("TLog");

            if (viewModel == null)
                viewModel = TerminalLogListCustomBinding.CreateTerminalLogListGridViewModel();

            return _TerminalLogListGridActionCore(viewModel, id);




        }
        public ActionResult _TerminalLogListPaging(GridViewPagerState pager)
        {
            int terminalId = 0;
            if (Session["terminalId"] != null)
            {
                terminalId = Convert.ToInt32(Session["terminalId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("TLog");
            viewModel.Pager.Assign(pager);
            return _TerminalLogListGridActionCore(viewModel, terminalId);
        }


        public ActionResult _TerminalLogListFiltering(GridViewFilteringState filteringState)
        {
            int terminalId = 0;
            if (Session["terminalId"] != null)
            {
                terminalId = Convert.ToInt32(Session["terminalId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("TLog");

            viewModel.ApplyFilteringState(filteringState);
            return _TerminalLogListGridActionCore(viewModel, terminalId);
        }

        public ActionResult _TerminalLogListSorting(GridViewColumnState column, bool reset)
        {
            int terminalId = 0;
            if (Session["terminalId"] != null)
            {
                terminalId = Convert.ToInt32(Session["terminalId"]);
            }
            var viewModel = GridViewExtension.GetViewModel("TLog");
            viewModel.ApplySortingState(column, reset);
            return _TerminalLogListGridActionCore(viewModel, terminalId);
        }


        public ActionResult _TerminalLogListGridActionCore(GridViewModel gridViewModel, int terminalId)
        {
            gridViewModel.ProcessCustomBinding(new GridViewCustomBindingGetDataRowCountHandler(args => { TerminalLogListCustomBinding.TerminalLogListGetDataRowCount(args, terminalId); }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        TerminalLogListCustomBinding.TerminalLogListGetData(args, terminalId);
                    })
            );
            return PartialView("_TermianlLog", gridViewModel);
        }
    }
}
