using AutoMapper;
using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WMS.Controllers
{
    public class AdminUtilitiesController : BaseController
    {
        private readonly IAdminServices _adminServices;
        private readonly IProductServices _productServices;
        private readonly IApplicationContext _currentDbContext;
        private readonly IEmployeeShiftsServices _employShiftServices;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceServices;
        private readonly IDeliverectSyncService _deliverectSyncService;
        private readonly IGaneConfigurationsHelper emailServices;

        public AdminUtilitiesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAdminServices adminServices,
            IProductServices productServices, IApplicationContext currentDbContext, IEmployeeShiftsServices employeeShiftsServices, IMapper mapper, IInvoiceService invoiceServices, IDeliverectSyncService deliverectSyncService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _adminServices = adminServices;
            _productServices = productServices;
            _currentDbContext = currentDbContext;
            _employShiftServices = employeeShiftsServices;
            _mapper = mapper;
            _invoiceServices = invoiceServices;
            _deliverectSyncService = deliverectSyncService;
        }

        public ActionResult RecalculateStockAll()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            bool Result = false;
            ViewBag.Result = "Failed";

            var warehouses = CurrentTenant.TenantLocations.Where(x => x.IsActive != false && x.IsDeleted != true).ToList();

            foreach (var loc in warehouses)
            {
                Result = Inventory.StockRecalculateAll(loc.WarehouseId, CurrentTenantId, CurrentUserId);
            }

            if (Result)
            {
                ViewBag.Result = "Success";
            }

            return View();
        }

        public ActionResult RecalculateLocationsStockAll()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            bool Result = false;
            ViewBag.Result = "Failed";

            var warehouses = CurrentTenant.TenantLocations.Where(x => x.IsActive != false && x.IsDeleted != true).ToList();

            foreach (var loc in warehouses)
            {
                Result = Inventory.LocationStockRecalculateAll(loc.WarehouseId, CurrentTenantId, CurrentUserId);
            }

            if (Result)
            {
                ViewBag.Result = "Success";
            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Stock Recalculate was successful";
            ViewBag.Detail = "Stock Recalculate was successful";

            return View("AdminUtilities");
        }

        public ActionResult RecalculateStock(int id)
        {
            caUser user = caCurrent.CurrentUser();
            int warehouse = caCurrent.CurrentWarehouse().WarehouseId;
            Inventory.StockRecalculate(id, warehouse, user.TenantId, user.UserId);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Stock Recalculate was successful";
            ViewBag.Detail = "Stock Recalculate was successful";

            return View("AdminUtilities");

        }
        public ActionResult Departments()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [ValidateInput(false)]
        public ActionResult DepartmentsManagerPartial()
        {
            var model = LookupServices.GetAllValidTenantDepartments(CurrentTenantId);
            return PartialView("_DepartmentsManagerPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentsManagerPartialAddNew(TenantDepartments item)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                try
                {
                    _adminServices.SaveTenantDepartment(item, CurrentUserId);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = LookupServices.GetAllValidTenantDepartments(CurrentTenantId);
            return PartialView("_DepartmentsManagerPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentsManagerPartialUpdate(TenantDepartments item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _adminServices.SaveTenantDepartment(item, CurrentUserId);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = LookupServices.GetAllValidTenantDepartments(CurrentTenantId);
            return PartialView("_DepartmentsManagerPartial", model.ToList());
        }

        public ActionResult UpdateDontMonitorStockForLocationProducts()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var res = _productServices.UpdateDontMonitorStockFlagForLocationproducts(CurrentTenantId);

            return View("UpdateDontMonitorStock");

        }

        public ActionResult GoodsInAllCreatedPalletTrackings()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var res = _productServices.GetAllPalletTrackings(CurrentTenantId, CurrentWarehouseId).Where(x => x.Status == PalletTrackingStatusEnum.Created).ToList();

            foreach (var item in res)
            {
                var caseQuantity = item.ProductMaster.ProductsPerCase ?? 1;
                var trans = Inventory.StockTransactionApi(item.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, (item.RemainingCases * caseQuantity), null, CurrentTenantId, CurrentWarehouseId, CurrentUserId, null, item.PalletTrackingId);

                if (trans != null)
                {
                    PalletTrackingSync trackingItem = new PalletTrackingSync();
                    _mapper.Map(item, trackingItem);
                    trackingItem.DateUpdated = DateTime.UtcNow;
                    trackingItem.Status = PalletTrackingStatusEnum.Active;
                    trackingItem.RemainingCases = item.TotalCases;
                    _productServices.SavePalletTrackings(trackingItem, CurrentTenantId);
                }
            }

            return View("GoodsInAllCreatedPalletTrackings");

        }

        public ActionResult CompleteActivePallets(string code, DateTime date)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var pallets = _currentDbContext.PalletTracking.Where(x => x.ExpiryDate < date && x.ProductMaster.SKUCode == code && x.Status == PalletTrackingStatusEnum.Active).ToList();

            int counter = 0;
            int remaining = pallets.Count();

            foreach (var item in pallets)
            {
                counter++;
                remaining--;
                var caseQuantity = item.ProductMaster.ProductsPerCase ?? 1;
                var trans = Inventory.StockTransactionApi(item.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, (item.RemainingCases * caseQuantity), null, CurrentTenantId, CurrentWarehouseId, CurrentUserId, null, item.PalletTrackingId);
                if (trans != null)
                {
                    item.DateUpdated = DateTime.UtcNow;
                    item.Status = PalletTrackingStatusEnum.Completed;
                    item.RemainingCases = 0;

                    if (counter == 50 || remaining < 50)
                    {
                        _currentDbContext.SaveChanges();
                        counter = 0;
                    }
                }
            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Active Pallet was Completed Successfully";

            return View("AdminUtilities");

        }

        public async Task<ActionResult> SyncDeliverectDatas()
        {
            await _deliverectSyncService.SyncChannelLinks();
            await _deliverectSyncService.SyncProducts(CurrentTenantId, CurrentUserId);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Deliverect Data Sync Completed Successfully";

            return View("AdminUtilities");
        }


        public async Task<ActionResult> CorrectTnAData(int Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var shifts = _currentDbContext.ResourceShifts.Where(x => x.ResourceId == Id).ToList();

            foreach (var item in shifts)
            {
                DateTime stampDate = item.TimeStamp;

                var minDateSpan = stampDate.AddHours(-16);
                var lastStamp = shifts.Where(x => x.TimeStamp >= minDateSpan && x.TimeStamp < stampDate).LastOrDefault();

                if (lastStamp == null || lastStamp.StatusType == "Out")
                {
                    item.StatusType = "In";
                }
                else
                {
                    item.StatusType = "Out";
                }
            }

            await _currentDbContext.SaveChangesAsync();

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Data Correction was Completed Successfully";

            return View("AdminUtilities");

        }

        public async Task<ActionResult> CorrectTnADataAll()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var employees = _currentDbContext.ResourceShifts.Where(x => x.TenantId == CurrentTenantId).ToList();

            foreach (var emp in employees)
            {
                await CorrectTnAData(emp.ResourceId.Value);
            }

            _currentDbContext.SaveChanges();

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Data Correction was Completed Successfully";

            return View("AdminUtilities");
        }

        public ActionResult AdjustPalletCasesAll()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            InventoryPalletExtensions.AdjustPalletRemainingCasesAll(CurrentTenantId, CurrentWarehouseId, CurrentUserId);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Pallet adjust operation was Completed Successfully";

            return View("AdminUtilities");
        }

        public ActionResult AdjustPalletCasesByPalletId(int pallettrackingId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            //int[] palletIds = Enumerable.Range(10300, 21).ToArray();
            InventoryPalletExtensions.AdjustPalletRemainingCases(pallettrackingId, CurrentTenantId, CurrentWarehouseId, CurrentUserId);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Pallet adjust operation was Completed Successfully";

            return View("AdminUtilities");
        }

        public ActionResult CalculatePalletTrackingRemainingCases(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            List<PalletAuditIssueList> palletAuditIssueLists = new List<PalletAuditIssueList>();
            if (Id.HasValue)
            {
                var product = _productServices.GetProductMasterById(Id ?? 0);
                if (product != null && product.ProcessByPallet)
                {
                    var palletTracking = _adminServices.GetPalletTrackingsbyProductId(Id, CurrentTenantId, CurrentWarehouseId).ToList();
                    foreach (var item in palletTracking)
                    {
                        var Processedqunatity = InventoryPalletExtensions.CalculatePalletQuantity(item.PalletTrackingId);
                        var remianingquantity = product.ProductsPerCase * item.RemainingCases;

                        if (Processedqunatity != remianingquantity)
                        {
                            PalletAuditIssueList palletAuditIssueList = new PalletAuditIssueList();
                            palletAuditIssueList.PalletTrackingId = item.PalletTrackingId;
                            palletAuditIssueList.ProductId = item.ProductId;
                            palletAuditIssueList.ProductName = item.ProductMaster.Name;
                            palletAuditIssueLists.Add(palletAuditIssueList);
                        }

                    }
                }
                else
                {
                    ViewBag.Error = "Product is not Process by Pallet";
                }
            }
            else
            {
                var Products = _productServices.GetAllProductProcessByPallet(CurrentTenantId).ToList();
                foreach (var item in Products)
                {
                    var palletTrackings = _adminServices.GetPalletTrackingsbyProductId(item.ProductId, CurrentTenantId, CurrentWarehouseId).ToList();
                    foreach (var palletTracking in palletTrackings)
                    {
                        var Processedqunatity = InventoryPalletExtensions.CalculatePalletQuantity(palletTracking.PalletTrackingId);
                        var remianingquantity = (item.ProductsPerCase ?? 1) * palletTracking.RemainingCases;
                        if (Processedqunatity != remianingquantity)
                        {
                            PalletAuditIssueList palletAuditIssueList = new PalletAuditIssueList();
                            palletAuditIssueList.PalletTrackingId = palletTracking.PalletTrackingId;
                            palletAuditIssueList.ProductId = item.ProductId;
                            palletAuditIssueList.ProductName = item.Name;
                            palletAuditIssueLists.Add(palletAuditIssueList);
                        }
                    }
                    if (palletAuditIssueLists.Count >= 200)
                    {
                        break;
                    }
                }
            }

            if (palletAuditIssueLists.Count > 0)
            {
                ViewBag.PalletAuditList = palletAuditIssueLists.Take(200).ToList();
            }
            else
            {
                ViewBag.Error = "No Pallets Found";
            }

            return View();
        }

        public ActionResult ArchiveOrRemoveOldPallets()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var archivableItemsAgeDays = _currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == (CurrentTenantId == 0 ? 1 : CurrentTenantId)).ArchivableItemsAgeDays;

            if (archivableItemsAgeDays <= 0)
            {
                ViewBag.Title = "Operation was Not Successful!";
                ViewBag.Message = "Operation was Not Successful!";
                ViewBag.Detail = "ArchivableItemsAgeDays is not configured in TenantConfigs table";

                return View("AdminUtilities");
            }

            var archiveDate = DateTime.UtcNow.AddDays(-1 * (_currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == CurrentTenantId).ArchivableItemsAgeDays));
            var archivablePallets = _currentDbContext.PalletTracking.Where(x => x.TenantId == CurrentTenantId &&
                                                                                x.WarehouseId == CurrentWarehouseId &&
                                                                                x.RemainingCases == 0 &&
                                                                                (x.DateUpdated ?? x.DateCreated) < archiveDate && x.Status != PalletTrackingStatusEnum.Archived).ToList();

            int counter = 0;
            int remaining = archivablePallets.Count();

            foreach (var pallet in archivablePallets)
            {
                counter++;
                remaining--;
                pallet.Status = PalletTrackingStatusEnum.Archived;
                pallet.DateUpdated = DateTime.UtcNow;

                if (counter == 50 || remaining < 50)
                {
                    _currentDbContext.SaveChanges();
                    counter = 0;
                }
            }

            var unusedPallets = _currentDbContext.PalletTracking.Where(x => x.TenantId == CurrentTenantId &&
                                                                            !_currentDbContext.InventoryTransactions.Any(i => i.PalletTrackingId == x.PalletTrackingId) &&
                                                                            !_currentDbContext.StockTakePalletsSnapshot.Any(i => i.PalletTrackingId == x.PalletTrackingId) &&
                                                                            x.WarehouseId == CurrentWarehouseId &&
                                                                            x.Status == PalletTrackingStatusEnum.Created &&
                                                                            (x.DateUpdated ?? x.DateCreated) < archiveDate).ToList();
            counter = 0;
            remaining = unusedPallets.Count();

            foreach (var pallet in unusedPallets)
            {
                counter++;
                remaining--;
                _currentDbContext.PalletTracking.Remove(pallet);

                if (counter == 50 || remaining < 50)
                {
                    _currentDbContext.SaveChanges();
                    counter = 0;
                }
            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = $"Archiving {archivablePallets.Count()} and Removing {unusedPallets.Count()} old pallets operation was Completed Successfully; ";



            return View("AdminUtilities");

        }

        public ActionResult CorrectOrderDetailIdInOrderProcessDetail()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var items = _currentDbContext.OrderProcessDetail.Where(x => x.OrderDetailID == null).Take(100).ToList();

            int counter = 0;
            int remaining = items.Count();

            foreach (var item in items)
            {
                counter++;
                remaining--;

                var orderDetail = _currentDbContext.OrderDetail.Where(x => x.OrderID == item.OrderProcess.OrderID && x.ProductId == item.ProductId).FirstOrDefault();
                orderDetail.TaxID = orderDetail.ProductMaster.EnableTax == true ? orderDetail.ProductMaster.TaxID : (int?)null;
                if (orderDetail != null)
                {
                    item.DateUpdated = DateTime.UtcNow;
                    item.OrderDetailID = orderDetail.OrderDetailID;
                }


                if (counter == 50 || remaining < 50)
                {
                    _currentDbContext.SaveChanges();
                    counter = 0;
                }

            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";

            return View("AdminUtilities");

        }

        public ActionResult CreateInvoicesforDispatchedOrders(int tenantId = 0, int userId = 0, bool skipAuthorisation = false)
        {
            if (!skipAuthorisation)
            {
                if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
                tenantId = CurrentTenantId;
                userId = CurrentUserId;
            }

            if (tenantId == 0 || userId == 0) { return Redirect((string)Session["ErrorUrl"]); }

            var items = _currentDbContext.OrderProcess.AsNoTracking().Where(x => x.OrderProcessStatusId == OrderProcessStatusEnum.Dispatched && x.InvoiceNo == null && x.IsDeleted != true
                && (x.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales || x.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder))
                .Select(y => y.OrderProcessID).Take(100).ToList();

            foreach (var item in items)
            {
                var invoice = _invoiceServices.GetInvoicePreviewModelByOrderProcessId(item, CurrentTenant);
                var invoiceMaster = _invoiceServices.CreateInvoiceForSalesOrder(invoice, tenantId, userId, true);
                invoice.InvoiceMasterId = invoiceMaster.InvoiceMasterId;
            }


            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";

            return View("AdminUtilities");

        }

        public ActionResult MarkCompletedOrdersDispatched(int tenantId = 0, bool skipAuthorisation = false)
        {
            if (!skipAuthorisation)
            {
                if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
                tenantId = CurrentTenantId;
            }

            if (tenantId == 0) { return Redirect((string)Session["ErrorUrl"]); }

            //mark order processes as dispatched where status is complete
            _currentDbContext.Database.ExecuteSqlCommand("update OrderProcesses set OrderProcessStatusId = {0} where  OrderProcessStatusId = {1} ", OrderProcessStatusEnum.Dispatched, OrderProcessStatusEnum.Complete);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";

            return View("AdminUtilities");

        }

        public ActionResult CorrectAccountBalances(string accountCode = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var accounts = _currentDbContext.Account.Where(x => x.IsDeleted != true && (x.AccountCode.ToLower() == accountCode.ToLower() || accountCode == null)).ToList();


            int counter = 0;
            int remaining = accounts.Count();
            List<string> notMatched = new List<string>();

            foreach (var account in accounts)
            {
                counter++;
                remaining--;

                decimal? balance = Financials.CalcAccountBalance(account.AccountID);
                var lastTransaction = _currentDbContext.AccountTransactions.Where(x => x.AccountId == account.AccountID && x.IsDeleted != true).OrderByDescending(x => x.AccountTransactionId).FirstOrDefault();

                if (balance != account.FinalBalance || account.FinalBalance != (lastTransaction?.FinalBalance ?? 0))
                {
                    notMatched.Add(account.AccountCode);
                    account.FinalBalance = balance;
                    account.DateUpdated = DateTime.Now;
                    account.UpdatedBy = 2;

                }

                if (counter == 50 || remaining < 50)
                {
                    _currentDbContext.SaveChanges();
                    counter = 0;
                }

            }

            ViewBag.Data = notMatched;
            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";


            return View("AdminUtilities");

        }

        public ActionResult CorrectAccountTransactions(string accountCode = null, int numberOfLastTransactions = 12)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var accounts = _currentDbContext.Account.Where(x => x.IsDeleted != true && (x.AccountCode.ToLower() == accountCode.ToLower() || accountCode == null)).ToList();
            List<string> modified = new List<string>();
            int counter = 0;

            foreach (var account in accounts)
            {
                counter++;
                decimal? balance = Financials.CalcAccountBalance(account.AccountID);
                var transactions = _currentDbContext.AccountTransactions.Where(x => x.AccountId == account.AccountID && x.IsDeleted != true)
                    .OrderByDescending(x => x.AccountTransactionId).Take(numberOfLastTransactions).ToList();

                transactions = transactions.OrderBy(x => x.AccountTransactionId).ToList();

                for (int i = 1; i < transactions.Count(); i++)
                {
                    var expectedBalance = 0m;
                    if (transactions[i].AccountTransactionTypeId == AccountTransactionTypeEnum.InvoicedToAccount)
                    {
                        expectedBalance = transactions[i - 1].FinalBalance + transactions[i].Amount;
                    }
                    else
                    {
                        expectedBalance = transactions[i - 1].FinalBalance - transactions[i].Amount;
                    }

                    if (expectedBalance != transactions[i].FinalBalance)
                    {
                        transactions[i].FinalBalance = expectedBalance;
                        transactions[i].DateUpdated = DateTime.Now;
                        transactions[i].UpdatedBy = 2;
                        modified.Add(transactions[i].AccountTransactionId.ToString());
                        _currentDbContext.SaveChanges();
                    }
                }
            }

            ViewBag.Data = modified;
            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";

            return View("AdminUtilities");

        }

        public ActionResult CorrectStockLocationRecords()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            bool Result = false;
            ViewBag.Result = "Failed";

            var stockListfromLocation = _currentDbContext.InventoryTransactions.Where(u =>
                     (u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Samples
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentOut
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut
                     || u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.MovementOut)).ToList();



            foreach (var item in stockListfromLocation)
            {
                var locationId = Inventory.GetLocation(CurrentTenantId, CurrentWarehouseId, CurrentUserId, item.LocationId);

                Result = InventoryStockMoveExtensions.AdjustStockLocations(item.ProductId, 0, locationId,
                    item.Quantity, item.WarehouseId, item.TenentId,
                    CurrentUserId, null, null, true);
            }

            if (Result)
            {
                ViewBag.Result = "Success";
            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Stock Recalculate was successful";
            ViewBag.Detail = "Stock Recalculate was successful";

            return View("AdminUtilities");
        }

        // use this method to correct stock levels where pallet tracking is right but total stock count is wrong
        public ActionResult CorrectStockAccordingToPallets()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var product = _currentDbContext.ProductMaster.Where(x => x.TenantId == CurrentTenantId && x.IsActive == true && x.IsDeleted != true).ToList();

            foreach (var item in product)
            {
                var inStock = _currentDbContext.InventoryStocks.FirstOrDefault(x => x.ProductId == item.ProductId && x.TenantId == CurrentTenantId && x.WarehouseId == CurrentWarehouseId)?.InStock ?? 0;
                var totalPallets = _currentDbContext.PalletTracking.Where(x => x.ProductId == item.ProductId && x.Status != PalletTrackingStatusEnum.Created).Count();

                decimal palletStock = 0;
                if (item.ProcessByPallet && totalPallets > 0)
                {

                    palletStock = _currentDbContext.PalletTracking.Where(u => u.ProductId == item.ProductId && (u.RemainingCases > 0) && u.Status != PalletTrackingStatusEnum.Created).Select(u => u.RemainingCases).DefaultIfEmpty(0).Sum();

                    palletStock = palletStock * (item.ProductsPerCase ?? 1);

                    if (inStock != palletStock)
                    {

                        if (inStock > palletStock)
                        {
                            var quantity = inStock - palletStock;

                            //create adjustment out transaction
                            Inventory.StockTransaction(item.ProductId, InventoryTransactionTypeEnum.AdjustmentOut, quantity, null, null, "Auto generated correction transaction", null);

                        }
                        else
                        {
                            var quantity = palletStock - inStock;

                            //create adjustment in transaction
                            Inventory.StockTransaction(item.ProductId, InventoryTransactionTypeEnum.AdjustmentIn, quantity, null, null, "Auto generated correction transaction", null);
                        }
                    }
                }
            }

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Stock Recalculate was successful";
            ViewBag.Detail = "Stock Recalculate was successful";

            return View("AdminUtilities");
        }

        public ActionResult UpdateProductPriceInAllRelatedTables(int productId, decimal priceTobeChanged,decimal priceTobeUpdated)
        {

            foreach (var item in _productServices.GetOrderDetailsByProductId(productId))
            {
                item.Price = priceTobeUpdated;
                item.DateUpdated = DateTime.UtcNow;
                item.TotalAmount = Math.Round((item.Qty * item.Price), 2);
                item.TaxAmount = item.TaxAmount > 0 ? item.TaxName?.PercentageOfAmount == null ? 0 : Math.Round(((item.TotalAmount / 100) * item.TaxName.PercentageOfAmount), 2) : 0;
                item.TotalAmount = item.TotalAmount + item.TaxAmount;
                OrderService.SaveOrderDetailAdmin(item, CurrentTenantId, CurrentUserId, priceTobeChanged);
            }
            foreach (var item in _productServices.GetInvoiceDetailsByProductId(productId))
            {
                item.Price = priceTobeUpdated;
                item.Total = Math.Round((priceTobeUpdated * item.Quantity), 2);
                item.Tax = item.Tax > 0 ? item.GlobalTax?.PercentageOfAmount == null ? 0 : Math.Round(((item.Total / 100) * item.GlobalTax.PercentageOfAmount), 2) : 0;
                item.NetAmount = item.Total + item.Tax;
                _invoiceServices.SaveInvoiceDetail(item, CurrentTenantId, CurrentUserId,priceTobeChanged);
            }

            return View();
        }



        public ActionResult ExplicitGC()
        {
            GC.Collect();
            ViewBag.Title = "GC collection was Successful";
            return View("AdminUtilities");
        }

    }
    public class PalletAuditIssueList
    {
        public int PalletTrackingId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}