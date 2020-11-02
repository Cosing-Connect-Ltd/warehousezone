﻿using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using ClosedXML.Report.Utils;

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

        public AdminUtilitiesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAdminServices adminServices,
            IProductServices productServices, IApplicationContext currentDbContext, IEmployeeShiftsServices employeeShiftsServices, IMapper mapper, IInvoiceService invoiceServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _adminServices = adminServices;
            _productServices = productServices;
            _currentDbContext = currentDbContext;
            _employShiftServices = employeeShiftsServices;
            _mapper = mapper;
            _invoiceServices = invoiceServices;
        }
        // GET: AdminUtilities/RecalculateStockAll
        /// <summary>
        /// This is used to recalculate all stock in case if needs to be removed some enteried manually from inventory transactions
        /// </summary>
        /// <returns></returns>
        public ActionResult RecalculateStockAll()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Boolean Result = false;
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

            Inventory.AdjustPalletRemainingCasesAll(CurrentTenantId, CurrentWarehouseId, CurrentUserId);

            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Pallet adjust operation was Completed Successfully";

            return View("AdminUtilities");
        }

        public ActionResult AdjustPalletCasesByPalletId(int pallettrackingId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            //int[] palletIds = Enumerable.Range(10300, 21).ToArray();
            Inventory.AdjustPalletRemainingCases(pallettrackingId, CurrentTenantId, CurrentWarehouseId, CurrentUserId);

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
                        var Processedqunatity = Inventory.CalculatePalletQuantity(item.PalletTrackingId);
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


                        var Processedqunatity = Inventory.CalculatePalletQuantity(palletTracking.PalletTrackingId);
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

            var archivableItemsAgeDays = _currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == CurrentTenantId).ArchivableItemsAgeDays;

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
                                                                                (x.DateUpdated ?? x.DateCreated) < archiveDate).ToList();

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

        public ActionResult CreateInvoicesforDispatchedOrders()
        {
            //if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var items = _currentDbContext.OrderProcess.AsNoTracking().Where(x => x.OrderProcessStatusId == OrderProcessStatusEnum.Dispatched &&
                                                                                 x.InvoiceNo == null &&
                                                                                 x.IsDeleted != true &&
                                                                                (x.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.DirectSales ||
                                                                                 x.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder))
                .Select(y => y.OrderProcessID).Take(100).ToList();

            foreach (var item in items)
            {
                var invoice = _invoiceServices.GetInvoicePreviewModelByOrderProcessId(item, CurrentTenant);
                var invoiceMaster = _invoiceServices.CreateInvoiceForSalesOrder(invoice, CurrentTenantId, CurrentUserId, true);
                invoice.InvoiceMasterId = invoiceMaster.InvoiceMasterId;
            }


            ViewBag.Title = "Operation was Successful";
            ViewBag.Message = "Operation was Successful";
            ViewBag.Detail = "Operation was Completed Successfully";

            return View("AdminUtilities");

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