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
using DevExpress.Web;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;

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
            var model = _stockTakeService.GetStockTakeByStatus(CurrentWarehouseId, 0, CurrentTenantId);

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
            
            return View(new StockTake());

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
            var model = _stockTakeService.GetAllStockTakes(CurrentTenantId, CurrentWarehouseId);

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







        public ActionResult ImportData(int Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.StocktakeId = Id;
            return View();
        }



        public ActionResult UploadStocktakeDataFile(int? StocktakeId)
        {

            ViewBag.StocktakeId = StocktakeId;
            UploadControlExtension.GetUploadedFiles("UploadControl", ValidationSettings, uc_FileUploadComplete);

            return null;
        }

        public readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".csv", },
            MaxFileSize = 100000000,
        };
        public void uc_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {

                try
                {
                    int stocktakeId = ViewBag.StocktakeId;
                    string ImportType = "Stocktake";
                    var importsDirectory = Server.MapPath("~/UploadedFiles/imports/");
                    if (!Directory.Exists(importsDirectory))
                    {
                        Directory.CreateDirectory(importsDirectory);
                    }
                    var importFileName = Server.MapPath("~/UploadedFiles/imports/" + "/Import_" + ImportType + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHMMss") + "_" + e.UploadedFile.FileName);
                    e.UploadedFile.SaveAs(importFileName, true);
                    string importResponse = "";

                    ImportStocktakeData(importFileName, stocktakeId, CurrentTenantId, CurrentWarehouseId, CurrentUserId).ForEach(m => importResponse += "<li>" + m + "</li>");

                    e.CallbackData = importResponse;
                    e.ErrorText = importResponse;

                }
                catch (Exception ex)
                {

                    e.CallbackData = "<li>File not imported: " + ex.Message + "<li>";
                }
            }

        }

        public List<string> ImportStocktakeData(string importPath, int stockTakeId, int tenantId, int warehouseId, int? userId = null)
        {

            var stocktake = _stockTakeService.GetStockTakeById(stockTakeId);

            if (stocktake == null)
            {
                return new List<string> { $"No Valid Stocktake found!" };
            }


            var headers = new List<string>();
            List<StockTakeImportViewModel> importData = null;

            try
            {
                using (var csv = new CsvReader(System.IO.File.OpenText(importPath), CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());

                    if (!headers.Contains("sku") || !headers.Contains("name") || !headers.Contains("quantity"))
                    {
                        return new List<string> { $"Incorrect file headers!" };
                    }

                    importData = csv.GetRecords<StockTakeImportViewModel>().ToList();
                }
            }
            catch (Exception)
            {
                return new List<string> { $"Incorrect file content!" };
            }

            if (importData == null || importData.Count() <= 0)
            {
                return new List<string> { $"Empty file, no values to import!" };
            }

            var exceptionsList = new List<string>();
            var successCount = 0;

            try
            {
                ImportDataIntoStocktake(tenantId,
                                        warehouseId,
                                        userId,
                                        stockTakeId,
                                        importData,
                                        ref exceptionsList,
                                        ref successCount);
            }
            catch (Exception ex)
            {
                exceptionsList.Add($"Unable to import data. Error message: {ex.Message}");
            }

            var result = new List<string>();

            result.Add($"{successCount} Items imported for \"{stocktake.StockTakeDescription}\" ");

            if (exceptionsList.Count > 0)
            {
                result.Add($"{exceptionsList.Count} Failures in import process :");
                result.AddRange(exceptionsList);
            }

            return result;
        }

        private void ImportDataIntoStocktake(int tenantId,
                                                        int warehouseId,
                                                        int? userId,
                                                        int stockTakeId,
                                                        IEnumerable<StockTakeImportViewModel> importData,
                                                        ref List<string> exceptionsList,
                                                        ref int successCount)
        {

            var stocktake = _stockTakeService.GetStockTakeById(stockTakeId);

            foreach (var stocktakeItem in importData)
            {
                var product = _productServices.GetProductMasterBySKU(stocktakeItem.SKU, tenantId);


                if (product == null)
                {
                    exceptionsList.Add($"Unable to find \"{stocktakeItem.SKU} - {stocktakeItem.Name}\"");
                    continue;
                }

                if (product.Serialisable == true)
                {
                    exceptionsList.Add($"Serialised SKU \"{stocktakeItem.SKU} - {stocktakeItem.Name}\" not supported");
                    continue;
                }

            }

            if (exceptionsList.Count > 0)
            {
                return;
            }

            foreach (var stocktakeItem in importData)
            {
                try
                {
                    var product = _productServices.GetProductMasterBySKU(stocktakeItem.SKU, tenantId);

                    var stockTakeDetail = new StockTakeDetail();
                    stockTakeDetail.StockTakeId = stocktake.StockTakeId;
                    stockTakeDetail.ProductId = product.ProductId;
                    stockTakeDetail.ReceivedSku = product.SKUCode;
                    stockTakeDetail.Quantity = stocktakeItem.Quantity;
                    stockTakeDetail.DateScanned = DateTime.Now;
                    stockTakeDetail.TenentId = tenantId;
                    stockTakeDetail.WarehouseId = warehouseId;
                    stockTakeDetail = _stockTakeService.CreateStockTakeDetail(stockTakeDetail);

                    if (product.ProcessByPallet)
                    {

                        PalletTracking palletTracking = new PalletTracking();
                        palletTracking.ProductId = product.ProductId;

                        palletTracking.BatchNo = stocktake.StockTakeId + "-csv-upload";
                        palletTracking.TotalCases = Math.Ceiling(stocktakeItem.Quantity / product.ProductsPerCase ?? 1);
                        palletTracking.RemainingCases = Math.Ceiling(stocktakeItem.Quantity / product.ProductsPerCase ?? 1);
                        palletTracking.Comments = stocktake.StockTakeDescription + " csv upload";
                        palletTracking.Status = Ganedata.Core.Entities.Enums.PalletTrackingStatusEnum.Created;
                        palletTracking.DateCreated = DateTime.UtcNow;
                        palletTracking.DateUpdated = DateTime.UtcNow;
                        palletTracking.TenantId = CurrentTenantId;
                        palletTracking.WarehouseId = CurrentWarehouseId;
                        string palletTrackingIds = _productServices.CreatePalletTracking(palletTracking, 1);
                        var palletTrackingId = Convert.ToInt32(palletTrackingIds.Split(',').ToArray()[0]);

                        palletTracking = _productServices.GetPalletbyPalletId(palletTrackingId);

                        StockTakeDetailsPallets detailPallets = new StockTakeDetailsPallets();
                        detailPallets.ProductPalletId = palletTrackingId;
                        detailPallets.ProductId = product.ProductId;
                        detailPallets.StockTakeDetailId = stockTakeDetail.StockTakeDetailId;
                        detailPallets.PalletSerial = palletTracking.PalletSerial;
                        detailPallets.CreatedBy = userId;

                        _stockTakeService.CreateStockTakeDetailsPallets(detailPallets);

                    }

                    successCount++;

                }
                catch (Exception ex)
                {
                    exceptionsList.Add($"Unable to import items for \"{stocktakeItem.SKU} - {stocktakeItem.Name}\". Error message: {ex.Message}");
                    continue;
                }
            }
        }

    }
}
