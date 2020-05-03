using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class TenantLocationsController : BaseController
    {
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IMarketServices _marketServices;
        private readonly ITerminalServices _terminalServices;
        private readonly IUserService _userService;
        private readonly IEmployeeServices _employeeServices;

        public TenantLocationsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITenantLocationServices tenantLocationServices, IMarketServices marketServices, ITerminalServices terminalServices, IUserService userService, IEmployeeServices employeeServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tenantLocationServices = tenantLocationServices;
            _marketServices = marketServices;
            this._terminalServices = terminalServices;
            this._userService = userService;
            this._employeeServices = employeeServices;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }



        // GET: /Warehouse/Details/5
        public ActionResult Details(int? id)
        {

            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantLocations tenantwarehouse = _tenantLocationServices.GetTenantLocationById((int)id);
            if (tenantwarehouse == null)
            {
                return HttpNotFound();
            }
            return View(tenantwarehouse);
        }

        // GET: /Warehouse/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.TenantLocations = LookupServices.GetAllWarehousesForTenant(CurrentTenantId).Select(m => new SelectListItem() { Value = m.WarehouseId.ToString(), Text = m.WarehouseName });

            ViewBag.AllTerminals = _terminalServices.GetAllTerminalsWithoutMobileLocationLinks(CurrentTenantId).Select(m => new SelectListItem() { Value = m.TerminalId.ToString(), Text = m.TerminalName + " " + m.TermainlSerial });
            ViewBag.AllDrivers = _employeeServices.GetAllEmployeesWithoutResourceLinks(CurrentTenantId).Select(m => new SelectListItem() { Value = m.AuthUserId.ToString(), Text = m.SurName + " " + m.FirstName });

            ViewBag.CountryId = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryId", "CountryName");
            ViewBag.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TenantLocations tenantwarehouse)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {

                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();
                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _tenantLocationServices.SaveTenantLocation(tenantwarehouse, user.UserId, tenant.TenantId);

                return RedirectToAction("Index");
            }
            ViewBag.TenantLocations = LookupServices.GetAllWarehousesForTenant(CurrentTenantId).Select(m => new SelectListItem() { Value = m.WarehouseId.ToString(), Text = m.WarehouseName });
            ViewBag.CountryId = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryId", "CountryName");
            ViewBag.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });

            ViewBag.AllTerminals = _terminalServices.GetAllTerminalsWithoutMobileLocationLinks(CurrentTenantId).Select(m => new SelectListItem() { Value = m.TerminalId.ToString(), Text = m.TerminalName + " " + m.TermainlSerial });
            ViewBag.AllDrivers = _employeeServices.GetAllEmployeesWithoutResourceLinks(CurrentTenantId).Select(m => new SelectListItem() { Value = m.AuthUserId.ToString(), Text = m.SurName + " " + m.FirstName });

            return View(tenantwarehouse);
        }

        // GET: /Warehouse/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            TenantLocations tenantwarehouse = _tenantLocationServices.GetTenantLocationById((int)id);
            if (tenantwarehouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.TenantLocations = LookupServices.GetAllWarehousesForTenant(CurrentTenantId, (int)id).Select(m => new SelectListItem() { Value = m.WarehouseId.ToString(), Text = m.WarehouseName });
            ViewBag.AllVehicles = _marketServices.GetAllValidMarketVehicles(CurrentTenantId).MarketVehicles.Select(m => new SelectListItem() { Value = m.Id.ToString(), Text = m.Name });
            ViewBag.CountryId = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryId", "CountryName", tenantwarehouse.CountryID);

            ViewBag.AllTerminals = _terminalServices.GetAllTerminalsWithoutMobileLocationLinks(CurrentTenantId, tenantwarehouse.SalesTerminalId).Select(m => new SelectListItem() { Value = m.TerminalId.ToString(), Text = m.TerminalName + " " + m.TermainlSerial });
            ViewBag.AllDrivers = _employeeServices.GetAllEmployeesWithoutResourceLinks(CurrentTenantId, tenantwarehouse.SalesManUserId).Select(m => new SelectListItem() { Value = m.AuthUserId.ToString(), Text = m.SurName + " " + m.FirstName });

            return View(tenantwarehouse);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TenantLocations tenantwarehouse)
        {
            if (ModelState.IsValid)
            {
                // get properties of tenant
                caTenant tenant = caCurrent.CurrentTenant();
                // get properties of user
                caUser user = caCurrent.CurrentUser();

                _tenantLocationServices.UpdateTenantLocation(tenantwarehouse, user.UserId, tenant.TenantId);

                return RedirectToAction("Index");

            }
            ViewBag.TenantLocations = LookupServices.GetAllWarehousesForTenant(CurrentTenantId, (int)tenantwarehouse.TenantId);

            ViewBag.AllTerminals = _terminalServices.GetAllTerminalsWithoutMobileLocationLinks(CurrentTenantId, tenantwarehouse.SalesTerminalId).Select(m => new SelectListItem() { Value = m.TerminalId.ToString(), Text = m.TerminalName + " " + m.TermainlSerial });
            ViewBag.AllDrivers = _employeeServices.GetAllEmployeesWithoutResourceLinks(CurrentTenantId, tenantwarehouse.SalesManUserId).Select(m => new SelectListItem() { Value = m.AuthUserId.ToString(), Text = m.SurName + " " + m.FirstName });

            return View(tenantwarehouse);
        }

        // GET: /Warehouse/Delete/5
        public ActionResult Delete(int? id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TenantLocations tenantwarehouse = _tenantLocationServices.GetTenantLocationById((int)id);
            if (tenantwarehouse == null)
            {
                return HttpNotFound();
            }
            return View(tenantwarehouse);
        }

        // POST: /Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }


            TenantLocations tenantwarehouse = _tenantLocationServices.GetTenantLocationById((int)id);


            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();
            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _tenantLocationServices.DeleteTenantLocation(tenantwarehouse, user.UserId);
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _Whouse()
        {
            // get properties of tenant
            caTenant tenant = caCurrent.CurrentTenant();

            var model = _tenantLocationServices.GetAllTenantLocations(tenant.TenantId);
            return PartialView("__Whouse", model.ToList());
        }


        public ActionResult EditStockLevels(int? warehouseId = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (!warehouseId.HasValue)
            {
                warehouseId = CurrentWarehouseId;
                var warehouse = _tenantLocationServices.GetTenantLocationById(warehouseId.Value);
                ViewBag.WarehouseName = warehouse.WarehouseName;
            }

            return View();
        }


        [ValidateInput(false)]
        public ActionResult UpdateProductLevels(MVCxGridViewBatchUpdateValues<WarehouseProductLevelViewModel, int> updateValues)
        {
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    _tenantLocationServices.UpdateProductLevelsForTenantLocation(CurrentWarehouseId, product.ProductID,
                        product.MinStockQuantity, CurrentUserId);
            }
            return _StockLevelsPartial();
        }

        public ActionResult _StockLevelsPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("StockLevelsGridView");

            if (viewModel == null)
                viewModel = LocationListCustomBinding.CreateLocationGridViewModel();


            return LocationGridActionCore(viewModel);
        }
        public ActionResult _LocationListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("StockLevelsGridView");
            viewModel.Pager.Assign(pager);
            return LocationGridActionCore(viewModel);
        }

        public ActionResult _LocationFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("StockLevelsGridView");
            viewModel.ApplyFilteringState(filteringState);
            return LocationGridActionCore(viewModel);
        }


        public ActionResult _LocationGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("StockLevelsGridView");
            viewModel.ApplySortingState(column, reset);
            return LocationGridActionCore(viewModel);
        }

        public ActionResult LocationGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    LocationListCustomBinding.LocationGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        LocationListCustomBinding.LocationGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_StockLevelsPartial", gridViewModel);
        }

    }
}
