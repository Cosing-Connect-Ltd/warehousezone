using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WMS.Helpers;
using Ganedata.Core.Entities.Helpers;
using System.Globalization;
using Ganedata.Core.Models;
using System.Threading.Tasks;

namespace WMS.Controllers
{
    public class PurchaseOrdersController : BaseReportsController
    {
        public IStockTakeApiService StockTakeApiService;
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLookupService;
        private readonly ICommonDbServices _commonDbServices;
        private readonly ICoreOrderService _orderService;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly ISalesOrderService _salesServices;
        private readonly IAccountServices _accountServices;
        private readonly IMapper _mapper;

        public PurchaseOrdersController(ITenantLocationServices tenantLocationServices, ICoreOrderService orderService, IStockTakeApiService stockTakeApiService, IPropertyService propertyService, IAccountServices accountServices,
            ILookupServices lookupServices, IAppointmentsService appointmentsService, IProductServices productServices, IProductLookupService productLookupService, IGaneConfigurationsHelper ganeConfigurationHelper,
            IEmailServices emailServices, ICommonDbServices commonDbServices, ITenantLocationServices tenantLocationservices, ISalesOrderService salesOrderService, ITenantsServices tenantsServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, ganeConfigurationHelper, emailServices, tenantLocationservices, tenantsServices)
        {
            StockTakeApiService = stockTakeApiService;
            _productServices = productServices;
            _productLookupService = productLookupService;
            _orderService = orderService;
            _tenantLocationServices = tenantLocationServices;
            _salesServices = salesOrderService;
            _accountServices = accountServices;
            _commonDbServices = commonDbServices;
            _mapper = mapper;
        }
        // GET: PurchaseOrders  
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Error = TempData["Error"];
            return View();
        }

        public string GeneratePO(int inventoryTranstionType)
        {
            return GenerateNextOrderNumber((InventoryTransactionTypeEnum)inventoryTranstionType);
        }


        public ActionResult Create(int? Id, string pageToken = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (Id == null)
            {
                int id = 0;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(id, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                var directsales = _salesServices.GetDirectSaleOrders(null);
                ViewBag.DirectOrderList = new SelectList(directsales, "OrderID", "OrderNumber");
                Order NewOrder = new Order();
                NewOrder.OrderNumber = GeneratePO((int)InventoryTransactionTypeEnum.PurchaseOrder);
                NewOrder.IssueDate = DateTime.Today;
                SetViewBagItems(null, EnumAccountType.Supplier);
                ViewBag.OrderDetails = new List<OrderDetail>();
                ViewBag.OrderProcesses = new List<OrderProcess>();
                //ViewBag.IsCollectionFromCustomerSide = true;
                ViewBag.AllowAccountAddress = caCurrent.CurrentWarehouse().AllowShipToAccountAddress;
                if (string.IsNullOrEmpty(pageToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }
                NewOrder.InventoryTransactionTypeId =(int)InventoryTransactionTypeEnum.PurchaseOrder;
                GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderDetailSessionViewModel>());
                GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderNotes>());
                return View(NewOrder);
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Order Order, OrderRecipientInfo shipmentAndRecipientInfo, int EmailTemplate)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (shipmentAndRecipientInfo.ShipmentDestination == null ||
                        shipmentAndRecipientInfo.TenantAddressID == null)
                    {
                        ViewBag.Error = "Please choose a delivery address";
                    }
                    else
                    {
                        var orderDetails = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                        var orderNotes = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);




                        OrderService.CreatePurchaseOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(orderDetails, new List<OrderDetail>()), orderNotes);

                        if (shipmentAndRecipientInfo.SendEmailWithAttachment)
                        {

                            var report = CreatePurchaseOrderPrint(Order.OrderID);
                            PrepareDirectory("~/UploadedFiles/reports/po/");
                            var reportPath = "~/UploadedFiles/reports/po/" + Order.OrderNumber + ".pdf";
                            report.ExportToPdf(Server.MapPath(reportPath));
                            var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Purchase order", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                                worksOrderNotificationType: (WorksOrderNotificationTypeEnum)EmailTemplate);

                            if (result != "Success")
                            {
                                TempData["Error"] = result;
                            }
                        }

                        GaneOrderDetailsSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                        GaneOrderNotesSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                        return AnchoredOrderIndex("PurchaseOrders", "Index", "PO");
                    }
                }

                SetViewBagItems(Order, EnumAccountType.Supplier);
                int id = 0;
                ViewBag.AllowAccountAddress = caCurrent.CurrentWarehouse().AllowShipToAccountAddress;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(id, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                var directsales = _salesServices.GetDirectSaleOrders(null);
                ViewBag.DirectOrderList = new SelectList(directsales, "OrderID", "OrderNumber");

                if (string.IsNullOrEmpty(shipmentAndRecipientInfo.PageSessionToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }
                return View(Order);
            }
            catch (Exception Exp)
            {
                if (Exp.InnerException != null && Exp.InnerException.Message == "Duplicate Order Number")
                {
                    ModelState.AddModelError("OrderNumber", Exp.Message);
                }
                else
                {
                    ModelState.AddModelError("", Exp.Message);
                }
                SetViewBagItems(Order);
                int id = 0;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(id, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
                var directsales = _salesServices.GetDirectSaleOrders(null);
                ViewBag.DirectOrderList = new SelectList(directsales, "OrderID", "OrderNumber");
                return View(Order);
            }
        }

        public ActionResult GetAccountRecipient(int? id)
        {
            return Json(GaneConfigurationsHelper.GetRecipientEmailForAccount(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckProductExist(string id)
        {
            var product = _productServices.GetProductMasterByProductCode(id, CurrentTenantId);
            return Json(product != null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountContactRecipient(int id)
        {
            return Json(GaneConfigurationsHelper.GetRecipientEmailForAccount(0, id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearPageSessionData(string id = null)
        {
            GaneOrderDetailsSessionHelper.ClearSessionTokenData(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id, string pageSessionToken = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order Order = OrderService.GetOrderById(id.Value);
            var accountaddressId = OrderService.GetAccountContactId(id.Value).ToList();
            if (Order == null)
            {
                return HttpNotFound();
            }
            var list = OrderService.GetAccountContactId(id ?? 0).ToList();
            var EmailAutoCheckedOnEdit = _tenantServices.GetTenantConfigById(CurrentTenantId).EmailAutoCheckedOnEdit;
            if (EmailAutoCheckedOnEdit)
            {
                if (list.Any())
                {
                    ViewBag.accountShip = true;

                }
            }

            int ids = 0;
            var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(ids, CurrentTenantId);
            ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail");
            ViewBag.AllowAccountAddress = caCurrent.CurrentWarehouse().AllowShipToAccountAddress;

            SetViewBagItems(Order, EnumAccountType.Supplier);
            if (string.IsNullOrEmpty(pageSessionToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }

            var odList = OrderService.GetAllValidOrderDetailsByOrderId(id.Value).ToList();
            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(odList, new List<OrderDetailSessionViewModel>()));
            var odNotes = Order.OrderNotes.Where(a => a.IsDeleted != true).ToList();
            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, odNotes);

            if (Order.ShipmentPropertyId.HasValue && Order.ShipmentPropertyId > 0)
            {
                ViewBag.IsShipmentToProperty = true;
                ViewBag.ShipmentPropertyId = Order.ShipmentPropertyId;
            }
            else if (Order.AccountAddressId.HasValue && Order.AccountAddressId > 0)
            {
                ViewBag.IsAccountAddressId = true;
                ViewBag.AccountAddressId = Order.AccountAddressId;
            }

            else if (Order.IsShippedToTenantMainLocation || Order.ShipmentWarehouseId != null)
            {

                ViewBag.IsShipmentToWarehouse = true;
            }
            else if (Order.IsCollectionFromCustomerSide)
            {
                ViewBag.IsCollectionFromCustomerSide = true;
            }
            else
            {
                ViewBag.IsShipmentToCustomAddress = true;
            }
            return View(Order);
        }

        #region Pallet Tracking
        public JsonResult _VerifyPallete(string serial, int? pid)
        {
            bool status = false;
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            var recored = OrderService.GetVerifedPallet(serial, pid ?? 0, CurrentTenantId, warehouseId);
            if (recored != null)
            {
                List<string> values = new List<string>();
                values.Add(recored.PalletTrackingId.ToString());
                values.Add(recored.PalletSerial);
                values.Add(recored.RemainingCases.ToString());
                return Json(values, JsonRequestBehavior.AllowGet);
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult _AddPalletes()
        {
            int? receivedQty = 0;
            if (!string.IsNullOrEmpty(Request.Params["rec_qty"]))
            {
                receivedQty = int.Parse(Request.Params["rec_qty"]);
            }

            ViewBag.QuantityEnabled = true;
            int qty = int.Parse(Request.Params["qty"]);
            ViewBag.ProcessedQuantity = receivedQty;
            int prodId = int.Parse(Request.Params["pid"]);
            ViewBag.RequiredQuantity = qty - receivedQty;
            ViewBag.PalletPerCase = _productServices.GetProductMasterById(prodId).ProductsPerCase ?? 1;
            decimal caseperpallet = ((decimal)ViewBag.RequiredQuantity / (decimal?)ViewBag.PalletPerCase ?? 1);
            if (ViewBag.RequiredQuantity < 0) { ViewBag.RequiredQuantity = 0; }
            if (caseperpallet < 0) { caseperpallet = 0; }
            ViewBag.cases = caseperpallet;
            if (Request.UrlReferrer.AbsolutePath.Contains("PurchaseOrders"))
            {
                ViewBag.Locations = new SelectList(_productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId), "LocationId", "LocationCode");
                ViewBag.QuantityEnabled = false;
            }

            if (Request.UrlReferrer.AbsolutePath.Contains("SalesOrders") || Request.UrlReferrer.AbsolutePath.Contains("WorksOrders"))
            {
                ViewBag.QuantityEnabled = false;
                var palletTracking = _tenantLocationServices.GetTenantLocationById(CurrentWarehouseId);
                if (palletTracking != null)
                {
                    ViewBag.palletTracking = palletTracking.PalletTrackingScheme;
                    ViewBag.palletTrackingId = (int)palletTracking.PalletTrackingScheme;
                    int palletTrackingIds = (int)palletTracking.PalletTrackingScheme;
                    if (palletTrackingIds == 3)
                    {
                        var date = _salesServices.GetSerialByPalletTrackingScheme(prodId, palletTrackingIds, CurrentTenantId, CurrentWarehouseId)?.ExpiryDate;
                        if (date != null)
                        {
                            ViewBag.MoYy = date.Value.Day + "/" + date.Value.Month + "/" + date.Value.Year;
                            ViewBag.expDate = date;
                        }

                    }
                    if (palletTrackingIds == 4)
                    {
                        var date = _salesServices.GetSerialByPalletTrackingScheme(prodId, palletTrackingIds, CurrentTenantId, CurrentWarehouseId)?.ExpiryDate;
                        if (date != null)
                        {
                            ViewBag.MoYy = date.Value.ToString("MMMM", CultureInfo.InvariantCulture) + "-" + date.Value.Year;
                            ViewBag.expDate = date;
                        }
                    }
                    else
                    {
                        ViewBag.Serial = _salesServices.GetSerialByPalletTrackingScheme(prodId, palletTrackingIds, CurrentTenantId, CurrentWarehouseId)?.PalletSerial;
                    }

                }

            }


            ViewBag.product = _productServices.GetProductMasterById(prodId);
            return PartialView();

        }
        public JsonResult _SubmitPalleteSerials(List<string> serialList, int? pid, int? orderId, string DeliveryNo, int OrderDetailID)
        {
            GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync();
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            List<PalleTrackingProcess> palleTrackingProcessList = new List<PalleTrackingProcess>();
            foreach (var pallet in serialList)
            {
                PalleTrackingProcess palleTrackingProcess = new PalleTrackingProcess();
                decimal quantity = 0;
                var palletdData = pallet.Split(new string[] { "#+#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                if (palletdData.Length >= 2)
                {
                    int PalletTrackingId = 0;

                    if (!string.IsNullOrEmpty(palletdData[0]))
                    {
                        PalletTrackingId = int.Parse(palletdData[0]);
                        palleTrackingProcess.PalletTrackingId = PalletTrackingId;
                    }
                    if (!string.IsNullOrEmpty(palletdData[1]))
                    {
                        quantity = decimal.Parse(palletdData[1]);
                        palleTrackingProcess.ProcessedQuantity = quantity;
                    }
                }

                palleTrackingProcessList.Add(palleTrackingProcess);
            }

            goodsReturnRequestSync.PalleTrackingProcess = palleTrackingProcessList;
            goodsReturnRequestSync.ProductId = pid ?? 0;
            goodsReturnRequestSync.OrderId = orderId ?? 0;
            goodsReturnRequestSync.deliveryNumber = DeliveryNo;
            goodsReturnRequestSync.OrderDetailID = OrderDetailID;
            goodsReturnRequestSync.tenantId = CurrentTenantId;
            goodsReturnRequestSync.warehouseId = warehouseId;
            goodsReturnRequestSync.userId = CurrentUserId;

            int result = OrderService.ProcessPalletTrackingSerial(goodsReturnRequestSync);
            return Json(result < 0 ? false : true, JsonRequestBehavior.AllowGet);
        }

        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderID,OrderNumber,DirectShip,ExpectedDate,Note,AccountID,OrderTypeID,LoanID,OrderStatusID,AccountContactId,Posted,PPropertyId,DepartmentId,InvoiceNo,InvoiceDetails,OrderCost,IsCollectionFromCustomerSide,ShipmentAccountAddressId")] Order Order, OrderRecipientInfo shipmentAndRecipientInfo, string orderSaveAndProcess, int EmailTemplate)
        {

            if (ModelState.IsValid)
            {
                var items = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                var orderNotes = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);
                OrderService.SavePurchaseOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(items, new List<OrderDetail>()), orderNotes);

                if (shipmentAndRecipientInfo.SendEmailWithAttachment)
                {
                    var report = CreatePurchaseOrderPrint(Order.OrderID);
                    PrepareDirectory("~/UploadedFiles/reports/po/");
                    var reportPath = "~/UploadedFiles/reports/po/" + Order.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Purchase order has been updated", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: (WorksOrderNotificationTypeEnum)EmailTemplate);

                    if (result != "Success")
                    {
                        TempData["Error"] = result;
                    }

                }

                GaneOrderDetailsSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                GaneOrderNotesSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                ViewBag.Fragment = Request.Form["fragment"];
                if (orderSaveAndProcess == "1")
                {
                    return Redirect(Url.RouteUrl(new { controller = "PurchaseOrders", action = "ReceivePO", id = Order.OrderID }) + "?fragment=" + ViewBag.Fragment as string);
                }

                return AnchoredOrderIndex("PurchaseOrders", "Index", ViewBag.Fragment as string);
            }
            ViewBag.AllowAccountAddress = caCurrent.CurrentWarehouse()?.AllowShipToAccountAddress;
            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();

            SetViewBagItems(null, EnumAccountType.Supplier);
            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "SupplierID", "CompanyName", Order.AccountID);
            return View(Order);
        }
        public ActionResult _PurchaseOrders()
        {
            var model = OrderService.GetAllPurchaseOrders(CurrentTenantId);
            return PartialView("_PurchaseOrders", model.ToList());

        }
        public ActionResult _OrderDetails(string pageSessionToken)
        {
            ViewBag.setName = "gridViewOrdDet";
            ViewBag.routeValues = new { Controller = "_OrderDetails", Action = "Order" };

            var model = GaneOrderDetailsSessionHelper.GetOrderDetailSession(pageSessionToken);
            return PartialView("_OrderDetails", model);
        }

        public ActionResult ReceivePO(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = OrderService.GetOrderById(id.Value);

            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDto = _mapper.Map(order, new ReceivePOVM());
            VerifyOrderStatus(order.OrderID);
            orderDto.DeliveryNumber = GaneStaticAppExtensions.GenerateDateRandomNo();
            orderDto.InventoryTransactionTypeId = (int)InventoryTransactionTypeEnum.PurchaseOrder;
            if (TempData["AutoCompleteError"] != null)
            {
                ViewBag.Warning = TempData["AutoCompleteError"].ToString();
            }
            return View(orderDto);
        }

        public ActionResult _PODetails(int Id)
        {
            ViewBag.orderid = Id;
            VerifyOrderStatus(Id);
            ViewBag.setname = "gvPODetail";
            ViewBag.route = new { Controller = "PurchaseOrders", Action = "_PODetails", Id = ViewBag.orderid };
            return PartialView("_PODetails", OrderService.GetPurchaseOrderDetailsById(Id, CurrentTenantId));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReceivePO(Order model)
        {

            return View();
        }

        public PartialViewResult _AddSerial()
        {
            ViewBag.Locations = new SelectList(_productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId), "LocationId", "LocationCode");
            return PartialView();

        }
        /// <summary>
        /// This is for non serialized products.
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _RecProduct()
        {
            var whouse = caCurrent.CurrentWarehouse();
            int pid = int.Parse(Request.Params["pid"]);
            int orderid = int.Parse(Request.Params["order"]);
            Session["delivery"] = Request.Params["delivery"];
            ViewBag.Locations = new SelectList(_productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId), "LocationId", "LocationCode");
            return PartialView(new InventoryTransaction { ProductId = pid, OrderID = orderid });
        }

        public JsonResult _VerifySerial(string serial)
        {
            var cnt = _productServices.GetProductSerialBySerialCode(serial, CurrentTenantId, true);
            return Json(cnt != null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Deliveries()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult _Deliveries()
        {
            var data = OrderService.GetAllPurchaseOrderProcesses(CurrentTenantId, CurrentWarehouseId).OrderByDescending(x => x.DateCreated).Select(ops => new
            {
                ops.DeliveryNO,
                ops.OrderID,
                ops.DateCreated,
                ops.OrderProcessID,
                ops.Order.OrderNumber,
                ops.Order.Account.AccountCode,
                ops.Order.Account.CompanyName,
                ops.Order.OrderStatusID,
                ops.OrderProcessStatusId
            }).ToList();

            return PartialView(data);
        }

        public ActionResult _DeliveryDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = OrderService.GetOrderProcessDetailByOrderProcessId(ProcessId).Select(opd => new
            {
                opd.ProductMaster.Name,
                opd.ProductMaster.Serialisable,
                opd.ProductMaster.ProcessByPallet,
                opd.ProductMaster.DontMonitorStock,
                opd.ProductMaster.SKUCode,
                opd.QtyProcessed,
                opd.DateCreated,
                opd.OrderProcessDetailID,
                count = _productServices.GetInventroyTransactionCountbyOrderProcessDetailId(opd.OrderProcessDetailID, CurrentTenantId),

            }).ToList();
            return PartialView(data);

        }

        public ActionResult GoodsReceiveNote()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult _GoodsReceiveNote()
        {
            var data = OrderService.GetAllGoodsReceiveNotes(CurrentTenantId, CurrentWarehouseId).OrderByDescending(x => x.DateCreated).Select(ops => new
            {
                ops.ReceiveCountId,
                ops.OrderID,
                ops.DateCreated,
                ops.ReferenceNo,
                ops.Notes,
                ops.Order.OrderNumber,
                ops.Order.Account.AccountCode,
                ops.Order.Account.CompanyName
            }).ToList();

            return PartialView(data);
        }

        public ActionResult _GoodsReceiveNoteDetails(Guid Id)
        {
            ViewBag.processId = Id;

            var data = OrderService.GetGoodsReceiveNoteDetailsById(Id).Select(opd => new
            {
                opd.ProductMaster.Name,
                opd.ProductMaster.SKUCode,
                opd.Counted,
                opd.Demaged,
                opd.DateCreated,
                opd.ReceiveCountDetailId
            }).ToList();

            return PartialView(data);

        }

        #region BlindShipment
        public ActionResult BlindShipment(bool? delivery)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                acnts.AccountID,
                acnts.AccountNameCode
            }).ToList();
            ViewBag.AccountID = new SelectList(accounts, "AccountID", "AccountNameCode");
            ViewBag.AccountAddresses = new List<SelectListItem>() { new SelectListItem() { Text = "Select", Value = "0" } };
            ViewBag.ShowPriceInBlindShipment = caCurrent.CurrentWarehouse().ShowPriceInBlindShipment;
            ViewBag.ShowTaxInBlindShipment = caCurrent.CurrentWarehouse().ShowTaxInBlindShipment;
            ViewBag.ShowQtyInBlindShipment = caCurrent.CurrentWarehouse().ShowQtyInBlindShipment;
            ViewBag.ShowDepartmentInBlindShipment = caCurrent.CurrentWarehouse().ShowDepartmentInBlindShipment;
            ViewBag.ShowGroupInBlindShipment = caCurrent.CurrentWarehouse().ShowGroupInBlindShipment;
            ViewBag.ProductGroup = new List<SelectListItem>() { new SelectListItem() { Text = "Select Group", Value = "0" } };
            ViewBag.ProductDepartment = new List<SelectListItem>() { new SelectListItem() { Text = "Select Department", Value = "0" } };
            ViewBag.Groups = new SelectList(LookupServices.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            ViewBag.Departments = new SelectList(LookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            var taxes = (from gtax in LookupServices.GetAllValidGlobalTaxes(CurrentTenant.CountryID)
                         select new
                         {
                             TaxId = gtax.TaxID,
                             TaxName = gtax.TaxName + " - " + gtax.PercentageOfAmount + " %"

                         }).ToList();
            ViewBag.GlobalTaxes = new SelectList(taxes, "TaxId", "TaxName");
            ViewBag.DeliveryNo = GaneStaticAppExtensions.GenerateDateRandomNo();
            Session["bsList"] = new List<BSDto>();
            if (delivery != null && delivery == true)
            {
                ViewBag.type = (int)InventoryTransactionTypeEnum.SalesOrder;
                ViewBag.Title = "Create Delivery";
                ViewBag.delivery = delivery;
                return View();
            }

            ViewBag.Title = "Create Shipment";
            ViewBag.type = (int)InventoryTransactionTypeEnum.PurchaseOrder;
            return View();
        }
        public ActionResult _BlindShipment()
        {
            var pid = Request.Params["pid"];

            if (pid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!string.IsNullOrEmpty(pid))
            {

                int number;
                bool success = Int32.TryParse(pid, out number);
                if (success)
                {
                    int id = int.Parse(pid.ToString());
                    var product = _productServices.GetProductMasterById(id);
                    var locs = _productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId).ToList();
                    ViewBag.Locations = new SelectList(locs, "LocationId", "LocationWithCode", locs.FirstOrDefault()?.LocationId);
                    ViewBag.QuantityEnabled = true;

                    if (product == null)
                        return HttpNotFound();
                    if (product.Serialisable)
                        return PartialView("_AddSerial");
                    else if (product.ProcessByPallet && caCurrent.CurrentWarehouse().EnableGlobalProcessByPallet)
                    {
                        var palletTracking = _tenantLocationServices.GetTenantLocationById(CurrentWarehouseId);
                        if (palletTracking != null)
                        {
                            ViewBag.palletTracking = palletTracking.PalletTrackingScheme;
                            ViewBag.palletTrackingId = (int)palletTracking.PalletTrackingScheme;
                        }


                        return PartialView("_AddPalletes");
                    }
                    else
                    {
                        ViewBag.Title = "Save";
                        ViewBag.prodID = product?.Description;
                        ViewBag.Groups = _tenantServices.GetAllTenantConfig(CurrentTenantId)?.FirstOrDefault(u => u.EnableTimberProperties)?.EnableTimberProperties;
                        if (!string.IsNullOrEmpty(Request.Params["Edit"]))
                        {
                            var key = string.IsNullOrEmpty(Request.Params["key"]) ? 0 : int.Parse(Request.Params["key"].ToString());
                            var bsList = Session["bsList"] as List<BSDto>;
                            var itemtoedit = bsList.Find(a => a.Id == key);
                            ViewBag.Locations = new SelectList(locs, "LocationId", "LocationWithCode", itemtoedit?.LocationId);
                            ViewBag.quantity = itemtoedit.Quantity;
                            ViewBag.fscpercent = itemtoedit.FscPercent;
                            ViewBag.prodID = itemtoedit.ProductDesc;
                            ViewBag.Key = key;


                        }
                        return PartialView("_RecProduct");
                    }
                }
            }
            return Content("");
        }

        public JsonResult _SubmitProduct(BSDto product)
        {
            if (product?.Id > 0)
            {
                var bsList = Session["bsList"] as List<BSDto>;
                var itemtoedit = bsList.Find(a => a.Id == product?.Id);
                if (itemtoedit != null)
                {
                    itemtoedit.LocationId = product.LocationId;
                    itemtoedit.ProductDesc = product.ProductDesc;
                    itemtoedit.FscPercent = product.FscPercent;
                    itemtoedit.Quantity = product.Quantity;

                }
            }
            else
            {
                var model = _productServices.GetProductMasterById(product.ProductId);
                product.ProductName = product.SKU;
                product.SKU = model?.SKUCode;
                if (product.ProductGroupId.HasValue)
                {
                    product.GroupProduct = _productLookupService.GetProductGroupById(product.ProductGroupId ?? 0)?.ProductGroup;
                }
                else
                {

                    product.GroupProduct = model?.ProductGroup?.ProductGroup;
                }
                product.ProductDesc = product.ProductDesc;
                var lst = Session["bsList"] as List<BSDto>;
                if (lst.Count() > 0)
                {
                    product.Id = lst.Last().Id + 1;
                }
                else
                {
                    product.Id = 1;
                }

                lst.Add(product);
            }
            return null;
        }
        public JsonResult _SubmitSerial(List<BSDto> products)
        {
            var model = _productServices.GetProductMasterById(products.FirstOrDefault()?.ProductId ?? 0);
            products.ForEach(u => u.SKU = model.SKUCode);
            products.ForEach(u => u.ProductName = model.Name);
            products.ForEach(u => u.Quantity = 1);

            var lst = Session["bsList"] as List<BSDto>;
            products.ForEach(m => m.Id++);
            lst.AddRange(products);
            return null;
        }

        public JsonResult CreateProduct(BSDto product)
        {
            if ((product.IsNewProduct == true) && (product.ProductName != null))
            {
                ProductDetailRequest productDetailRequest = new ProductDetailRequest();
                productDetailRequest.IsNewProduct = product?.IsNewProduct ?? false;
                productDetailRequest.ProductName = product?.ProductName;
                productDetailRequest.TenantId = CurrentTenantId;
                productDetailRequest.TaxIds = product?.TaxId ?? 3;
                productDetailRequest.ProductDesc = product.ProductDesc;
                productDetailRequest.ProductDepartmentId = product.ProductDepartmentId;
                productDetailRequest.ProductGroupId = product.ProductGroupId;

                var products = StockTakeApiService.CreateProductOnStockTake(productDetailRequest);
                if (!string.IsNullOrEmpty(products?.Result?.FailureMessage))
                {

                    return Json(products.Result.FailureMessage);
                }
                product.ProductId = products.Result.ProductId;
                product.ProductName = product.ProductName;
                product.SKU = products.Result.ProductCode;
                product.GroupProduct = products.Result.ProductGroup;




            }
            product.Quantity = product.Quantity ?? 1;
            product.Price = product.Price ?? 0;
            var lst = Session["bsList"] as List<BSDto>;
            if (lst.Count() > 0)
            {
                product.Id = lst.Last().Id + 1;
            }
            else
            {
                product.Id = 1;
            }
          
            lst.Add(product);
            return null;
        }

        public ActionResult _BSList()
        {

            var model = Session["bsList"] as List<BSDto>;
            ViewBag.Groups = _tenantServices.GetAllTenantConfig(CurrentTenantId)?.FirstOrDefault(u => u.EnableTimberProperties)?.EnableTimberProperties;


            return PartialView(model.OrderByDescending(m => m.Id).ToList());
        }

        public JsonResult ProductType()
        {

            return Json(true, JsonRequestBehavior.AllowGet);
        }




        public JsonResult _ConfirmBS(int account, string delivery, int type, AccountShipmentInfo accountShipmentInfo)
        {

            try
            {
                var bsList = Session["bsList"] as List<BSDto>;
                if (bsList == null || bsList.Count == 0)
                    return Json(new { error = true, msg = "Session expired" });
                var order = OrderService.CreateBlindShipmentOrder(bsList, account, delivery, GeneratePO(type), CurrentTenantId, CurrentWarehouseId, CurrentUserId, type, accountShipmentInfo);
                return null;
            }
            catch (Exception exp)
            {

                return Json(new { error = true, msg = exp.Message });
            }
        }

        public JsonResult _RemoveItem(int Id)
        {
            var bsList = Session["bsList"] as List<BSDto>;
            var itemtoRemove = bsList.Find(a => a.Id == Id);
            bsList.Remove(itemtoRemove);
            return null;
        }
        public JsonResult _EditItem(int Id)
        {
            var bsList = Session["bsList"] as List<BSDto>;
            var itemtoedit = bsList.Find(a => a.Id == Id);
            return Json(itemtoedit, JsonRequestBehavior.AllowGet);
           
        }
        #endregion

        public JsonResult _GetAccountAddress(int accountId, int orderId)
        {
            List<Tuple<string, string>> addresses = new List<Tuple<string, string>>();

            try
            {
                if (orderId > 0)
                {

                    var secondryAddress = _accountServices.GetAllValidAccountContactsByAccountId(accountId, CurrentTenantId);
                    var list = OrderService.GetAccountContactId(orderId).ToList();
                    foreach (var item in secondryAddress)
                    {
                        if (!string.IsNullOrEmpty(item.ContactEmail))
                        {

                            addresses.Add(new Tuple<string, string>(item.AccountContactId.ToString(), item.ContactEmail));
                        }
                    }
                    var checkedIds = list.Select(u => u.AccountContactId);
                    string[] selected = checkedIds.Select(p => p.ToString()).ToArray();
                    var obj = new MultiSelectList(addresses, "Item1", "Item2", selected);
                    return Json(obj, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    var secondryAddress = _accountServices.GetAllValidAccountContactsByAccountId(accountId, CurrentTenantId);
                    foreach (var item in secondryAddress)
                    {
                        if (!string.IsNullOrEmpty(item.ContactEmail))
                        {
                            addresses.Add(new Tuple<string, string>(item.AccountContactId.ToString(), item.ContactEmail));
                        }

                    }
                    var selected = AccountServices.GetAllValidAccountContactsByAccountContactId(accountId).Select(u => u.AccountContactId).ToList();
                    var obj = new MultiSelectList(addresses, "Item1", "Item2", selected);

                    return Json(obj, JsonRequestBehavior.AllowGet);

                }

            }
            catch
            {
                return Json(new { error = true });

            }
        }


        public JsonResult _GetAccountShipmentAddress(int accountId)
        {
            var accountAdress = _accountServices.GetAccountAddressById(accountId);
            if (accountAdress == null)
            {
                return null;
            }
            return Json(new { AddressLine1 = accountAdress.AddressLine1, AddressLine2 = accountAdress.AddressLine2, AddressLine3 = accountAdress.AddressLine3, AddressLine4 = accountAdress.AddressLine4, ShipmentAddressPostcode = accountAdress.PostCode }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult _GetOrderDetail(int OrderId, int accounId, string PageSession)
        {
            var odList = OrderService.GetAllValidOrderDetailsByOrderId(OrderId).ToList();
            var orderDetails = new List<OrderDetail>();
            foreach (var item in odList)
            {
                var productPrice = _productServices.GetProductMasterById(item.ProductId)?.BuyPrice;
                item.Price = productPrice ?? 0;
                orderDetails.Add(_commonDbServices.SetDetails(item, accounId, "PurchaseOrders", ""));
            }
            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(PageSession, _mapper.Map(orderDetails, new List<OrderDetailSessionViewModel>()));
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInventroyTransactionCount(int OrderProcessDetailId)
        {
            var Count = _productServices.GetInventroyTransactionCountbyOrderProcessDetailId(OrderProcessDetailId, CurrentTenantId);
            ViewBag.count = Count;
            return Content(Count.ToString());
        }

        public JsonResult _GetProductDepartment(int accountId)
        {
            var model = LookupServices.GetProductDepartments(accountId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult _GetProductGroup(int DepartmentId)
        {
            var model = LookupServices.GetProductGroups(DepartmentId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult RefreshProductGroupAndDepartment(string value)
        {
            if (value == "Group")
            {
               var Groups = new SelectList(LookupServices.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
                return Json(Groups, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Departments = new SelectList(LookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
                return Json(Departments, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetAccountAddressByAccountId(int id)
        {
            var accountaddress = _accountServices.GetAllValidAccountAddressesByAccountId(id).OrderBy(u => u.AddTypeShipping).Select(u=> new{
                AccountAddressId=u.AddressID,
                AccountAddress=u.FullAddressValue

            });
            return Json(accountaddress, JsonRequestBehavior.AllowGet);
        }

    }



}

