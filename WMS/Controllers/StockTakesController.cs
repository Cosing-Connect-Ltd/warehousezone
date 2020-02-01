using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using Ganedata.Core.Entities.Enums;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using WMS.CustomBindings;
using System.Threading.Tasks;

namespace WMS.Controllers
{
    public class StockTakesController : BaseController
    {
        public ITenantLocationServices TenantLocationServices { get; }
        private readonly IStockTakeApiService _stockTakeService;
        private readonly ITenantsServices _tenantServices;
        private readonly IProductServices _productServices;
        private readonly IGaneConfigurationsHelper _configHelper;

        public StockTakesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IStockTakeApiService stockTakeService, ITenantsServices tenantServices, ITenantLocationServices tenantLocationServices, IProductServices productServices, IGaneConfigurationsHelper configHelper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            TenantLocationServices = tenantLocationServices;
            _stockTakeService = stockTakeService;
            _tenantServices = tenantServices;
            _productServices = productServices;
            _configHelper = configHelper;
        }
        // GET: StockTakes
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            return View();
        }

        // GET: StockTakes/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StockTake stockTake = _stockTakeService.GetStockTakeById(id ?? 0);
            if (stockTake == null)
            {
                return HttpNotFound();
            }
            return View(stockTake);
        }

        // GET: StockTakes/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }


            ViewBag.TenantId = new SelectList(_tenantServices.GetAllTenants(), "TenantId", "TenantName");
            ViewBag.WarehouseId = new SelectList(TenantLocationServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName");
            ViewBag.CurrentStocktakeId = -1;
            ViewBag.CurrentStocktakeRef = "";
            ViewBag.CurrentStocktakeDesc = "";
            ViewBag.CurrentStocktakeDate = "";

            ViewBag.CurrentTenantId = CurrentTenantId;
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.WarehouseId = CurrentWarehouseId;
            ViewBag.WarehouseName = CurrentWarehouse.WarehouseName;

            ViewBag.ProductsList = _productServices.GetAllValidProductMasters(CurrentTenantId).Where(x => x.DontMonitorStock != true).Select(p => new StockTakeProductLookupRequest()
            {
                ProductCode = p.SKUCode,
                ProductName = p.Name,
                ProductDescription = p.Description
            }).ToList();

            var allProducts = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();



            var products = new SelectList(allProducts, "ProductId", "NameWithCode").ToList();
            products.Insert(0, new SelectListItem() { Value = "0", Text = "Select Product" });

            ViewBag.Products = new SelectList(products, "Value", "Text");

            // check if any stocktake running
            var model = _stockTakeService.GetStockTakeByStatus(CurrentWarehouseId, 0,CurrentTenantId);

            if (model != null)
            {
                ViewBag.CurrentStocktakeId = model.StockTakeId;
                ViewBag.CurrentStocktakeRef = model.StockTakeReference;
                ViewBag.CurrentStocktakeDesc = model.StockTakeDescription;
                ViewBag.CurrentStocktakeDate = model.StartDate;
            }

            var pendingStoppedStockTake = _stockTakeService.GetStockTakeByStatus(CurrentWarehouseId, 1, CurrentTenantId);
            if (pendingStoppedStockTake != null)
            {
                return RedirectToAction("Details", new { id = pendingStoppedStockTake.StockTakeId });
            }

            return View();

        }

        // POST: StockTakes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockTakeId,StockTakeReference,StockTakeDescription")] StockTake stockTake)
        {

            if (ModelState.IsValid)
            {
                _stockTakeService.CreateStockTake(stockTake, CurrentUserId, CurrentTenantId, CurrentWarehouseId);

                return RedirectToAction("Create");
            }

            ViewBag.TenantId = new SelectList(_tenantServices.GetAllTenants(), "TenantId", "TenantName", stockTake.TenantId);
            ViewBag.WarehouseId = new SelectList(TenantLocationServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName", stockTake.WarehouseId);
            return View(stockTake);
        }

        public ActionResult StockTakeDetail(int sid)
        {
            ViewBag.StockID = sid;
            return View();
        }


        public JsonResult delete(int id)
        {
            bool status = _stockTakeService.DeleteStockTakeDetial(id);


            return Json(status, JsonRequestBehavior.AllowGet);
        }

        // GET: StockTakes/Delete/5
        public ActionResult Stop(int Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            _stockTakeService.StopStockTake(Id);

            return RedirectToAction("Index");
        }


        public ActionResult FullReportGridPartial()
        {
            if (!string.IsNullOrEmpty(Request.Params["id"]))
            {
                var id = int.Parse(Request.Params["id"]);
                return PartialView("FullReportStocktakeDetails", _stockTakeService.GetStockTakeReportById(id, CurrentTenantId, CurrentWarehouseId, CurrentUserId));
            }
            else
            {
                return Json("Invalid Request without ID.", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult VarianceReportGridPartial()
        {
            if (!string.IsNullOrEmpty(Request.Params["id"]))
            {
                var id = int.Parse(Request.Params["id"]);
                return PartialView("VarianceReportStocktakeDetails", _stockTakeService.GetStockTakeReportById(id, CurrentTenantId, CurrentWarehouseId, CurrentUserId, true));
            }
            else
            {
                return Json("Invalid Request without ID.", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApplyStocktakeChanges(FormCollection form)
        {
            var request = StockTakeApplyChangeRequest.MapFromFormCollection(form);

            _stockTakeService.ApplyStockTakeChanges(request, CurrentUserId);

            return RedirectToAction("Details", new { id = request.StockTakeId });
        }

        public ActionResult FullReport(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var fullReportResponse = _stockTakeService.GetStockTakeReportById(id, CurrentTenantId, CurrentWarehouseId, CurrentUserId);
            return View(fullReportResponse);
        }

        public async Task<ActionResult> VarianceReport(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var varianceReportResponse = _stockTakeService.GetStockTakeReportById(id, CurrentTenantId, CurrentWarehouseId, CurrentUserId, true);
            var tenantconfig = _tenantServices.GetTenantConfigById(CurrentTenant.TenantId);

            var misMatchingItems = varianceReportResponse.StockTakeReportResponseItems.Where(m => m.CurrentQuantity != m.PreviousQuantity).ToList();
            if (tenantconfig.EnableStockVarianceAlerts && misMatchingItems.Any())
            {
                var emailHeader = "<h2>Variance Report - " + DateTime.Now.ToString("dd/MM/yyyy") + "</h2>";
                var tableBuilder = new TagBuilder("table");
                tableBuilder.AddCssClass("table table-bordered");
                var tableHeader = new TagBuilder("tr")
                {
                    InnerHtml = "<th>Product Name</th>Previous Quantity<th></th><th>Current Quantity</th>"
                };
                tableBuilder.InnerHtml = tableHeader.ToString();
                foreach (var item in misMatchingItems)
                {
                    var rowBuilder = new TagBuilder("tr") { InnerHtml = "<td>" + item.ProductName + "</td>" };
                    rowBuilder.InnerHtml += "<td>" + item.PreviousQuantity + "</td>";
                    rowBuilder.InnerHtml += "<td>" + item.CurrentQuantity + "</td>";
                    tableBuilder.InnerHtml += rowBuilder.ToString();
                }

                await _configHelper.SendStandardMailNotification(CurrentTenantId, "Variance report requires adjustments", emailHeader + tableBuilder.ToString(), null, tenantconfig.AuthorisationAdminEmail);
            }

            return View(varianceReportResponse);
        }

        [ValidateInput(false)]
        public ActionResult StocktakeGridPartial()
        {
            var model = _stockTakeService.GetAllStockTakes(CurrentTenantId,CurrentWarehouseId);

            return PartialView("_StocktakeGridPartial", model.ToList());
        }


        [ValidateInput(false)]
        public ActionResult StockTakeCurrentProductsPartial()
        {
            var currentStockTake = _stockTakeService.GetStockTakeByStatus(CurrentWarehouseId, 0, CurrentTenantId);
            if (currentStockTake != null)
            {
                var stockTakeDetails = _stockTakeService.GetStockTakeDetailsByStockTakeId(currentStockTake.StockTakeId).OrderByDescending(p => p.DateScanned).ToList();
                ViewBag.StockTakeCurrentProducts = stockTakeDetails;
            }

            return PartialView("_StocktakeCurrentProducts");
        }
        public ActionResult _StocktakeDetailGridPartial(int Id)
        {
            var viewModel = GridViewExtension.GetViewModel("StocktakeGridDetail");
            ViewBag.StockID = Id;
            if (viewModel == null)
                viewModel = StockTakeDetailsCustomBinding.CreateStockTakeDetailsGridViewModel();
            return StocktakeGridActionCore(viewModel, Id);

        }

        public ActionResult StocktakeGridActionCore(GridViewModel gridViewModel, int Id)
        {
            ViewBag.StockID = Id;
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    StockTakeDetailsCustomBinding.StockTakeDetailsDataRowCount(args, CurrentTenantId, CurrentWarehouseId, Id);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        StockTakeDetailsCustomBinding.StockTakeDetailsData(args, CurrentTenantId, CurrentWarehouseId, Id);
                    })
            );
            return PartialView("_StocktakeDetailGridPartial", gridViewModel);
        }

        public ActionResult _StockTakesGridViewsPaging(GridViewPagerState pager, int Id)
        {
            ViewBag.StockID = Id;
            var viewModel = GridViewExtension.GetViewModel("StocktakeGridDetail");
            viewModel.Pager.Assign(pager);
            return StocktakeGridActionCore(viewModel, Id);
        }

        public ActionResult _StockTakesGridViewFiltering(GridViewFilteringState filteringState, int Id)
        {
            ViewBag.StockID = Id;
            var viewModel = GridViewExtension.GetViewModel("StocktakeGridDetail");
            viewModel.ApplyFilteringState(filteringState);
            return StocktakeGridActionCore(viewModel, Id);
        }
        public ActionResult _StockTakesGridViewDataSorting(GridViewColumnState column, bool reset, int Id)
        {
            ViewBag.StockID = Id;
            var viewModel = GridViewExtension.GetViewModel("StocktakeGridDetail");
            viewModel.ApplySortingState(column, reset);
            return StocktakeGridActionCore(viewModel, Id);
        }







        public ActionResult StockTakeCurrentProductSerialsPartial(int id, int stockTakeId)
        {
            var stockTakeSerialDetails = _stockTakeService.GetProductStockTakeSerials(stockTakeId, id);
            ViewBag.StockTakeCurrentProductSerials = stockTakeSerialDetails.Select(m => new { m.StockTakeDetailsSerialId, m.SerialNumber, m.DateScanned }).ToList();
            return PartialView("_StocktakeCurrentProductSerials");
        }
        [ValidateInput(false)]
        public ActionResult StockTakeCurrentProductSerialsPopupPartial()
        {
            var productId = Request.Params["productId"];
            if (string.IsNullOrEmpty(productId)) throw new Exception("Product reference is missing in stock take.");

            var stockTakeDetailProductId = int.Parse(productId);
            var currentStockTakeProduct = _productServices.GetProductMasterById(stockTakeDetailProductId);

            if (currentStockTakeProduct == null)
            {
                throw new Exception("Product is missing from the catalog.");
            }

            var currentStockTake = _stockTakeService.GetStockTakeByStatus(CurrentWarehouseId, 0, CurrentTenantId);
            var stockTakeSerialDetails = _stockTakeService.GetProductStockTakeSerials(currentStockTake.StockTakeId, stockTakeDetailProductId);

            ViewBag.ProductName = currentStockTakeProduct.Name;
            ViewBag.StockTakeCurrentProductSerials = stockTakeSerialDetails.Select(m => new { m.StockTakeDetailsSerialId, m.SerialNumber, m.DateScanned }).ToList();
            return PartialView("_StocktakeCurrentProductSerialsPopup");
        }

        public ActionResult ProductLookupPartial()
        {
            var model = _productServices.GetAllValidProductMasters(CurrentTenantId).Where(x => x.DontMonitorStock != true).Select(p => new StockTakeProductLookupRequest()
            {
                ProductCode = p.SKUCode,
                ProductName = p.Name,
                ProductDescription = p.Description
            }).ToList();
            return PartialView("_StocktakeCurrentProductLookup", model);
        }

    }
}
