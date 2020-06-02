using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WMS.CustomBindings;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class SalesOrdersController : BaseReportsController
    {
        private readonly ISalesOrderService _salesServices;
        private readonly IAccountServices _accountServices;
        private readonly IProductServices _productServices;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SalesOrdersController(IProductServices productServices, ISalesOrderService salesOrderService, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices,
            IAppointmentsService appointmentsService, IGaneConfigurationsHelper configurationsHelper, IEmailServices emailServices, ITenantLocationServices tenantLocationservices, IUserService userService, ITenantsServices tenantsServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, configurationsHelper, emailServices, tenantLocationservices, tenantsServices)
        {
            _salesServices = orderService;
            _accountServices = accountServices;
            _productServices = productServices;
            _mapper = mapper;
            _userService = userService;
        }
        // GET: SalesOrders
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Error = TempData["Error"];
            return View();
        }
        public ActionResult _SalesOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setName = "gvODetail";
            ViewBag.route = new { Controller = "SalesOrders", Action = "_SalesOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;

            var order = OrderService.GetOrderById(Id);
            ViewBag.CurrencySymbol = order?.AccountCurrency?.Symbol;
            VerifyOrderAccountStatus(order);

            return PartialView("_SalesOrderDetails", OrderService.GetSalesOrderDetails(Id, CurrentTenantId));
        }

        public JsonResult _SubmitPalleteSerials(List<string> serialList, int? pid, int? orderId, string DeliveryNo, int OrderDetailID, int? type)
        {
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;

            GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync();

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
            goodsReturnRequestSync.InventoryTransactionType = (InventoryTransactionTypeEnum)type;
            goodsReturnRequestSync.tenantId = CurrentTenantId;
            goodsReturnRequestSync.warehouseId = warehouseId;
            goodsReturnRequestSync.userId = CurrentUserId;

            int result = OrderService.ProcessPalletTrackingSerial(goodsReturnRequestSync);


            return Json(result < 0 ? false : true, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagItems(caTenant tenant, EnumAccountType accountType = EnumAccountType.All, Order order = null)
        {
            ViewBag.Accounts = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, accountType), "AccountID", "AccountNameCode");

            var transactionTypes = from InventoryTransactionTypeEnum d in Enum.GetValues(typeof(InventoryTransactionTypeEnum))
                                   where d == InventoryTransactionTypeEnum.SalesOrder || d == InventoryTransactionTypeEnum.Proforma || d == InventoryTransactionTypeEnum.Loan
                                   || d == InventoryTransactionTypeEnum.Exchange || d == InventoryTransactionTypeEnum.Quotation || d == InventoryTransactionTypeEnum.Samples
                                   select new { InventoryTransactionTypeId = (int)d, InventoryTransactionTypeName = d.ToString() };

            ViewBag.TransTypes = new SelectList(transactionTypes, "InventoryTransactionTypeId", "InventoryTransactionTypeName");

            var loans = (from ln in LookupServices.GetAllValidTenantLoanTypes(CurrentTenantId)
                         select new
                         {
                             LoanID = ln.LoanID,
                             LoanName = ln.LoanName + "  -  Loan Days= " + ln.LoanDays

                         }).ToList();
            ViewBag.TenantLoanTypes = new SelectList(loans, "LoanID", "LoanName");

            ViewBag.AuthUsers = new SelectList(OrderService.GetAllAuthorisedUsers(CurrentTenantId), "UserId", "UserName", order != null ? order.CreatedBy : CurrentUserId);
            ViewBag.Departments = new SelectList(LookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            ViewBag.AccountContacts = new SelectList(AccountServices.GetAllValidAccountContactsByAccountId(order?.AccountID ?? 0, CurrentTenantId), "AccountContactId", "ContactName", order?.AccountContactId);
        }
        public string GeneratePO()
        {
            return GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder);
        }
        public ActionResult Create(int? Id, string pageToken)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();

            if (Id == null)
            {
                int ids = 0;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(ids, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                Order NewOrder = new Order();
                NewOrder.OrderNumber = GeneratePO();
                NewOrder.IssueDate = DateTime.Today;
                SetViewBagItems(tenant, EnumAccountType.Customer);
                ViewBag.AccountContacts = new SelectList(
                AccountServices.GetAllTopAccountContactsByTenantId(CurrentTenantId), "AccountContactId", "ContactName");
                ViewBag.OrderDetails = new List<OrderDetail>();
                ViewBag.OrderProcesses = new List<OrderProcess>();

                ViewBag.IsShipmentToAccountAddress = true;
                ViewBag.AccountAddresses = new List<SelectListItem>() { new SelectListItem() { Text = "Select", Value = "0" } };
                ViewBag.ShipmentAccountAddressId = 0;
                ViewBag.IsShipmentToCustomAddress = false;
                ViewBag.ConsignmentTypes = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType");


                if (string.IsNullOrEmpty(pageToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }
                else
                {
                    ViewBag.ForceRegeneratePageToken = "False";
                    ViewBag.ForceRegeneratedPageToken = pageToken;
                }
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

                    var tempNotesList = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);
                    var tempList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                    Order = OrderService.CreateSalesOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(tempList, new List<OrderDetail>()), tempNotesList);

                    if (shipmentAndRecipientInfo.SendEmailWithAttachment)
                    {
                        var report = CreateSalesOrderPrint(Order.OrderID);
                        PrepareDirectory("~/UploadedFiles/reports/so/");
                        var reportPath = "~/UploadedFiles/reports/so/" + Order.OrderNumber + ".pdf";
                        report.ExportToPdf(Server.MapPath(reportPath));
                        var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Sales order", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                            worksOrderNotificationType: (WorksOrderNotificationTypeEnum)EmailTemplate);
                        if (result != "Success")
                        {
                            TempData["Error"] = result;
                        }
                    }

                    if (Order.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation)
                    {
                        var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Order Requires Authorisation", _mapper.Map(Order, new OrderViewModel()), null, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                            worksOrderNotificationType: WorksOrderNotificationTypeEnum.AwaitingOrderTemplate);

                        if (result != "Success")
                        {
                            TempData["Error"] = result;
                        }
                    }

                    //send ingredients email for certain categories
                    var productCategoryTc = _tenantServices.GetTenantConfigById(Order.TenentId)?.ProductCatagories;

                    if (Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder && !string.IsNullOrEmpty(productCategoryTc))
                    {
                        var productcategoryList = productCategoryTc.Split(',').Select(int.Parse).ToList();
                        if (Order.OrderDetails.Any(x => productcategoryList.Contains((int?)x.ProductMaster?.ProductGroupId ?? 0)))
                        {
                            var result = await GaneConfigurationsHelper.SendStandardMailProductGroup(Order.TenentId, Order.OrderNumber, Order.AccountID ?? 0);
                            if (result != "Success")
                            {
                                TempData["Error"] = result;
                            }
                        }
                    }

                    GaneOrderDetailsSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    GaneOrderNotesSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    return AnchoredOrderIndex("SalesOrders", "Index", "SOA");
                }

                int id = 0;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(id, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();

                SetViewBagItems(CurrentTenant, EnumAccountType.Customer);
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;

                ViewBag.IsShipmentToAccountAddress = true;
                ViewBag.AccountAddresses = new List<SelectListItem>() { new SelectListItem() { Text = "Select", Value = "0" } };
                ViewBag.ShipmentAccountAddressId = 0;
                ViewBag.IsShipmentToCustomAddress = false;

                ViewBag.ConsignmentTypes = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType");
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return View(Order);
            }
            catch (Exception Exp)
            {
                int ids = 0;
                var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(ids, CurrentTenantId);
                ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
                ViewBag.IsShipmentToAccountAddress = true;
                ViewBag.AccountAddresses = new List<SelectListItem>() { new SelectListItem() { Text = "Select", Value = "0" }
    };
                ViewBag.ShipmentAccountAddressId = 0;
                ViewBag.IsShipmentToCustomAddress = false;

                if (Exp.InnerException != null && Exp.InnerException.Message == "Duplicate Order Number")
                {
                    ModelState.AddModelError("OrderNumber", Exp.Message);
                }
                else
                {
                    ModelState.AddModelError("", Exp.Message);
                }
                var tenent = caCurrent.CurrentTenant();
                SetViewBagItems(tenent, EnumAccountType.Customer);
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;

                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
                ViewBag.ConsignmentTypes = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType");
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return View(Order);
            }
        }
        public ActionResult Edit(int? id, string pageToken)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            caTenant tenant = caCurrent.CurrentTenant();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order Order = OrderService.GetOrderById(id.Value);
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

            if (string.IsNullOrEmpty(pageToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }
            var odList = OrderService.GetAllValidOrderDetailsByOrderId(id.Value).ToList();

            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(odList, new List<OrderDetailSessionViewModel>()));

            SetViewBagItems(tenant, EnumAccountType.Customer, Order);

            int ids = 0;
            var accountaddress = _accountServices.GetAllValidAccountContactsByAccountId(ids, CurrentTenantId);
            ViewBag.AccountContactes = new SelectList(accountaddress, "AccountContactId", "ContactEmail", accountaddress.Select(x => x.AccountID).FirstOrDefault());
            var odNotes = Order.OrderNotes.Where(a => a.IsDeleted != true).ToList();

            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, odNotes);

            ViewBag.AccountAddresses = new List<SelectListItem>();
            if (Order.AccountID > 0)
            {
                var account = AccountServices.GetAccountsById(Order.AccountID.Value);
                if (account != null && account.AccountAddresses != null)
                {
                    ViewBag.AccountAddresses = new SelectList(account.AccountAddresses, "AddressID", "FullAddressValue", Order.ShipmentAccountAddressId);
                }
                else
                {
                    ViewBag.Warning = "No account address found";
                }

                if (Order.ConsignmentTypeId.HasValue && Order.ConsignmentTypeId.Value != (int)ConsignmentTypeEnum.Collection && Order.ShipmentAccountAddressId > 0)
                {
                    ViewBag.ShipmentAccountAddressId = Order.ShipmentAccountAddressId;
                    ViewBag.IsShipmentToAccountAddress = true;
                    ViewBag.IsShipmentToCustomAddress = false;
                }
                else
                {
                    ViewBag.ShipmentAccountAddressId = 0;
                    ViewBag.IsShipmentToAccountAddress = false;
                    ViewBag.IsShipmentToCustomAddress = true;
                }
            }

            ViewBag.ConsignmentTypes = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType", Order.ConsignmentTypeId);

            return View(Order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Order Order, FormCollection formCollection, OrderRecipientInfo shipmentAndRecipientInfo, string orderSaveAndProcess, int EmailTemplate)
        {

            if (ModelState.IsValid)
            {
                Order.OrderNumber = Order.OrderNumber.Trim();

                // get properties of user
                caUser user = caCurrent.CurrentUser();
                Order.DateUpdated = DateTime.UtcNow;
                Order.UpdatedBy = user.UserId;

                Order.WarehouseId = CurrentWarehouseId;

                var items = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);

                var nItems = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);

                Order = OrderService.SaveSalesOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(items, new List<OrderDetail>()), nItems);


                if (shipmentAndRecipientInfo.SendEmailWithAttachment)
                {
                    var report = CreateSalesOrderPrint(Order.OrderID);
                    PrepareDirectory("~/UploadedFiles/reports/so/");
                    var reportPath = "~/UploadedFiles/reports/so/" + Order.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Sales order has been updated", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: (WorksOrderNotificationTypeEnum)EmailTemplate);

                    if (result != "Success")
                    {
                        TempData["Error"] = result;
                    }

                }

                if (Order.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation)
                {
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Order Requires Authorisation", _mapper.Map(Order, new OrderViewModel()), null, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                             worksOrderNotificationType: WorksOrderNotificationTypeEnum.AwaitingOrderTemplate);

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
                    return Redirect(Url.RouteUrl(new { controller = "SalesOrders", action = "ProcessOrder", id = Order.OrderID }) + "?fragment=" + ViewBag.Fragment as string);
                }

                return AnchoredOrderIndex("SalesOrders", "Index", ViewBag.Fragment as string);
            }

            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
            SetViewBagItems(CurrentTenant, EnumAccountType.Customer);
            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "SupplierID", "CompanyName", Order.AccountID);

            ViewBag.AccountAddresses = new List<SelectListItem>();
            if (Order.AccountID > 0)
            {
                var account = AccountServices.GetAccountsById(Order.AccountID.Value);
                ViewBag.AccountAddresses = new SelectList(account.AccountAddresses, "AddressID", "FullAddressValue", Order.ShipmentAccountAddressId);
                if (Order.ConsignmentTypeId.HasValue && Order.ConsignmentTypeId.Value != (int)ConsignmentTypeEnum.Collection && Order.ShipmentAccountAddressId > 0)
                {
                    ViewBag.ShipmentAccountAddressId = Order.ShipmentAccountAddressId;
                    ViewBag.IsShipmentToAccountAddress = true;
                    ViewBag.IsShipmentToCustomAddress = false;
                }
                else
                {
                    ViewBag.ShipmentAccountAddressId = 0;
                    ViewBag.IsShipmentToAccountAddress = false;
                    ViewBag.IsShipmentToCustomAddress = true;
                }
            }

            ViewBag.ConsignmentTypes = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType", Order.ConsignmentTypeId);

            return View(Order);
        }


        private void VerifyOrderAccountStatus(Order order)
        {
            if (order.Account != null && order.Account.GlobalAccountStatus.AccountStatusID != (int)AccountStatusEnum.Active)
            {
                ViewBag.PreventProcessing = true;

                switch (order.Account.GlobalAccountStatus.AccountStatusID)
                {
                    case 2:
                        ViewBag.Error = "The Account is inactive. This order cannot be processed";
                        break;
                    case 3:
                        ViewBag.Error = "The Account is on Hold. This order cannot be processed";
                        break;
                    case 4:
                        ViewBag.Error = "The Account is on Stop. This order cannot be processed";
                        break;
                }
            }
            if (order.OrderStatusID == OrderStatusEnum.Hold)
            {
                ViewBag.PreventProcessing = true;
                ViewBag.Error = "This order is on hold and cannot be processed";
            }

            if (order.OrderStatusID == OrderStatusEnum.AwaitingAuthorisation)
            {
                ViewBag.PreventProcessing = true;
                ViewBag.Error = "This order is awaiting authorisation and cannot be processed";
            }
            if (order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Proforma || order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Quotation)
            {
                ViewBag.PreventProcessing = true;
                ViewBag.Error = "This order requires approval to a sales order for processing.";
            }
        }

        public ActionResult ProcessOrder(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order Order = OrderService.GetOrderById(id.Value);
            if (Order == null)
            {
                return HttpNotFound();
            }

            VerifyOrderAccountStatus(Order);

            var orderDto = _mapper.Map<ReceivePOVM>(Order);
            orderDto.DeliveryNumber = GaneStaticAppExtensions.GenerateDateRandomNo();
            orderDto.InventoryTransactionTypeId = (int)InventoryTransactionTypeEnum.SalesOrder;
            orderDto.AccountID = Order.AccountID ?? 0;
            ViewBag.Consignments = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType", Order.ConsignmentTypeId);

            //get http refferer

            string referrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "/";

            // route to appropriate controller / action
            ViewBag.RController = "SalesOrders";

            if (referrer.Contains("PickList"))
            {
                ViewBag.RController = "PickList";
            }

            if (TempData["AutoCompleteError"] != null)
            {
                ViewBag.Warning = TempData["AutoCompleteError"].ToString();
            }

            var latestOrderProcess = Order.OrderProcess.OrderByDescending(m => m.DateCreated).FirstOrDefault();
            if (latestOrderProcess != null)
            {
                orderDto.ShipmentAddressLine1 = latestOrderProcess.ShipmentAddressLine1;
                orderDto.ShipmentAddressLine2 = latestOrderProcess.ShipmentAddressLine2;
                orderDto.ShipmentAddressLine3 = latestOrderProcess.ShipmentAddressLine3;
                orderDto.ShipmentAddressLine4 = latestOrderProcess.ShipmentAddressLine4;
                orderDto.ShipmentAddressPostcode = latestOrderProcess.ShipmentAddressPostcode;
            }
            else if (Order.Account != null)
            {
                if (Order.Account.AccountAddresses.Any())
                {
                    var address = Order.Account.AccountAddresses.FirstOrDefault();
                    var shippingAddress = Order.Account.AccountAddresses.FirstOrDefault(m => m.AddTypeShipping == true);
                    if (shippingAddress != null)
                    {
                        address = shippingAddress;
                    }
                    if (address != null)
                    {
                        orderDto.ShipmentAddressLine1 = address.AddressLine1;
                        orderDto.ShipmentAddressLine2 = address.AddressLine2;
                        orderDto.ShipmentAddressLine3 = address.AddressLine3;
                        orderDto.ShipmentAddressLine4 = address.AddressLine4;
                        orderDto.ShipmentAddressPostcode = address.PostCode + ", " + address.GlobalCountry.CountryName;
                    }
                }
            }
            return View(orderDto);
        }

        public ActionResult Consignments()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        //public ActionResult _Consignments()
        //{
        //    var data = OrderService.GetAllSalesConsignments(CurrentTenantId, CurrentWarehouseId).OrderByDescending(x => x.DateCreated)
        //                .Select(ops => new
        //                {
        //                    ops.DeliveryNO,
        //                    ops.OrderID,
        //                    ops.DateCreated,
        //                    ops.OrderProcessID,
        //                    ops.Order.OrderNumber,
        //                    ops.Order.Account.AccountCode,
        //                    ops.Order.Account.CompanyName
        //                }).ToList();

        //    return PartialView(data);
        //}


        public ActionResult _Consignments(int? consignmentId)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "consignmentgridview");
            if (viewModel == null)
                viewModel = ConsignmentCustomBinding.CreateConsignmentGridViewModel();
            ViewBag.ConsignmentId = consignmentId;
            return _ConsignmentsGridActionCore(viewModel, consignmentId);


        }


        public ActionResult _ConsignmentsPaging(GridViewPagerState pager, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "consignmentgridview");
            viewModel.Pager.Assign(pager);
            return _ConsignmentsGridActionCore(viewModel, id);
        }

        public ActionResult _ConsignmentsFiltering(GridViewFilteringState filteringState, int? id)

        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "consignmentgridview");
            viewModel.ApplyFilteringState(filteringState);
            return _ConsignmentsGridActionCore(viewModel, id);
        }

        public ActionResult _ConsignmentsSorting(GridViewColumnState column, bool reset, int? id)
        {
            var viewModel = GridViewExtension.GetViewModel(ViewBag.productId + "consignmentgridview");
            viewModel.ApplySortingState(column, reset);
            return _ConsignmentsGridActionCore(viewModel, id);
        }

        public ActionResult _ConsignmentsGridActionCore(GridViewModel gridViewModel, int? Id)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ConsignmentCustomBinding.ConsignmentDataRowCount(args, CurrentTenantId, CurrentWarehouseId, Id);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ConsignmentCustomBinding.ConsignmentData(args, CurrentTenantId, CurrentWarehouseId, Id);
                    })
            );
            return PartialView("_Consignments", gridViewModel);
        }






        public ActionResult _ConsignmentDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = (from opd in OrderService.GetOrderProcessDetailsByProcessId(ProcessId)
                        select new
                        {
                            opd.ProductMaster.Name,
                            opd.ProductMaster.SKUCode,
                            opd.ProductMaster.Serialisable,
                            opd.ProductMaster.ProcessByPallet,
                            opd.ProductMaster.DontMonitorStock,
                            opd.QtyProcessed,
                            opd.DateCreated,
                            opd.OrderProcessDetailID

                        }).ToList();
            return PartialView(data);

        }

        public ActionResult _ConsignmentAddressDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;
            var data = (from opd in OrderService.GetALLOrderProcessByOrderProcessId(ProcessId)
                        select new
                        {
                            opd.OrderProcessID,
                            opd.ShipmentAddressLine1,
                            opd.ShipmentAddressLine2,
                            opd.ShipmentAddressLine3,
                            opd.ShipmentAddressLine4,
                            opd.ShipmentAddressPostcode,


                        }).ToList();

            return PartialView(data);

        }

        public JsonResult _deleteProduct(int id)
        {
            OrderService.DeleteSalesOrderDetailById(id, CurrentUserId);

            return null;
        }

        public JsonResult _VerifyPallete(string serial, int? pid, int? orderId, int? type, int palletTrackingId, string date)
        {
            bool status = false;
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            DateTime? dates = null;
            if (!string.IsNullOrEmpty(date))
            {
                dates = DateTime.Parse(date);
            }
            var recored = OrderService.GetVerifedPallet(serial, pid ?? 0, CurrentTenantId, warehouseId, type, palletTrackingId, dates, orderId);
            if (recored != null)
            {
                if (recored.Comments == "[#Expired#]")
                {
                    return Json("Expired", JsonRequestBehavior.AllowGet);
                }
                List<string> values = new List<string>();
                values.Add(recored.PalletTrackingId.ToString());
                values.Add(recored.PalletSerial);

                values.Add(recored.RemainingCases.ToString());
                return Json(values, JsonRequestBehavior.AllowGet);
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }



        public JsonResult _GetNextSerials(List<string> serial, int pid, int palletTrackingId)
        {
            if (palletTrackingId == 3)
            {
                List<string> dates = new List<string>();
                var date = _salesServices.GetUpdatedSerial(pid, palletTrackingId, CurrentTenantId, CurrentWarehouseId, serial)?.ExpiryDate;
                if (date != null)
                {
                    var MoYy = date.Value.Day + "/" + date.Value.Month + "/" + date.Value.Year;
                    dates.Add(MoYy);
                    dates.Add(date.Value.ToShortDateString());
                }

                return Json(dates, JsonRequestBehavior.AllowGet);

            }
            if (palletTrackingId == 4)
            {
                List<string> dates = new List<string>();
                var date = _salesServices.GetUpdatedSerial(pid, palletTrackingId, CurrentTenantId, CurrentWarehouseId, serial)?.ExpiryDate;
                if (date != null)
                {
                    var MoYy = date.Value.ToString("MMMM", CultureInfo.InvariantCulture) + "-" + date.Value.Year;
                    dates.Add(MoYy);
                    dates.Add(date.Value.ToShortDateString());
                }

                return Json(dates, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> dates = new List<string>();
                var serials = _salesServices.GetUpdatedSerial(pid, palletTrackingId, CurrentTenantId, CurrentWarehouseId, serial)?.PalletSerial;
                if (serials != null)
                {
                    dates.Add(serials);
                }

                return Json(dates, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult _EmailRecipientsPartial()
        {

            if (!string.IsNullOrEmpty(Request.Params["orderId"]) && string.IsNullOrEmpty(Request.Params["PORStatus"]))
            {
                int? orderId = int.Parse(Request.Params["orderId"]);
                int? templateId = null;
                if (!string.IsNullOrEmpty(Request.Params["templateId"]))
                {
                    templateId = int.Parse(Request.Params["templateId"]);
                }
                var order = OrderService.GetOrderById(orderId ?? 0);
                if (templateId == 8)
                {
                    var orderprocess = OrderService.GetOrderProcessByOrderProcessId(orderId ?? 0);
                    order = orderprocess.Order;
                }
                ViewBag.AccountContacts = new SelectList(AccountServices.GetAllValidAccountContactsByAccountId(order?.AccountID ?? 0, CurrentTenantId), "AccountContactId", "ContactName", order?.AccountContactId);
                var accountContact = AccountServices.GetAllValidAccountContactsByAccountId(order?.AccountID ?? 0, CurrentTenantId);
                List<Tuple<string, string>> email = new List<Tuple<string, string>>();
                if (accountContact.ToList().Count > 0)
                {
                    foreach (var item in accountContact)
                    {
                        if (!string.IsNullOrEmpty(item.ContactEmail))
                        {
                            email.Add(new Tuple<string, string>(item?.AccountContactId.ToString(), item?.ContactEmail));
                        }

                    }

                }
                var accountContactId = AccountServices.GetAllValidAccountContactsByAccountContactId(order?.AccountID ?? 0).Select(u => u.AccountContactId).ToList();
                var list = OrderService.GetAccountContactId(order.OrderID).ToList();
                var checkedIds = list.Select(u => u.AccountContactId);
                var selected = checkedIds.Select(p => p.ToString()).ToList();
                if (selected.Count > 0)
                {
                    ViewBag.AccountResult = new MultiSelectList(email, "Item1", "Item2", selected);
                }
                else
                {

                    ViewBag.AccountResult = new MultiSelectList(email, "Item1", "Item2", accountContactId);
                }
                ViewBag.OrdersId = order.OrderID;
                if (templateId.HasValue)
                {
                    ViewBag.purchaseorder = false;
                    ViewBag.DefaultMessage = _tenantServices.GetAllTenantConfig(CurrentTenantId)?.FirstOrDefault()?.DefaultCustomMessage;
                }
                else
                {
                    ViewBag.purchaseorder = true;
                }
                ViewBag.CustomEmails = true;
            }
            else if (!string.IsNullOrEmpty(Request.Params["accountId"]))
            {
                int AcccountId = int.Parse(Request.Params["accountId"].ToString());
                ViewBag.accountIds = AcccountId;
                if (!string.IsNullOrEmpty(Request.Params["StartDate"]))
                {
                    DateTime startDate = DateTime.Parse(Request.Params["StartDate"].ToString());
                    ViewBag.StartDates = startDate;
                }
                if (!string.IsNullOrEmpty(Request.Params["EndDate"]))
                {
                    DateTime endDate = DateTime.Parse(Request.Params["EndDate"].ToString());
                    ViewBag.EndDates = endDate;
                }
                ViewBag.FinancialTransactionReport = true;
                var accountContact = AccountServices.GetAllValidAccountContactsByAccountId(AcccountId, CurrentTenantId).Where(u => u.ConTypeStatment = true).ToList();
                if (accountContact.Count > 0)
                {
                    var selected = accountContact.Select(u => u.AccountContactId.ToString()).ToList();
                    var email = accountContact.Select(u => new
                    {
                        AccountContactId = u.AccountContactId,
                        ContactEmail = u.ContactEmail

                    }).ToList();
                    ViewBag.AccountResult = new MultiSelectList(email, "AccountContactId", "ContactEmail", selected);
                }
                else
                {
                    ViewBag.AccountResult = new MultiSelectList(accountContact.ToList(), "AccountContactId", "ContactEmail", accountContact?.FirstOrDefault()?.AccountContactId.ToString());
                }
            }
            else if (!string.IsNullOrEmpty(Request.Params["orderId"]) && !string.IsNullOrEmpty(Request.Params["PORStatus"]))
            {
                ViewBag.FinancialTransactionReport = true;
                int OrderId = int.Parse(Request.Params["orderId"].ToString());
                ViewBag.OrdersId = OrderId;
                var orders = OrderService.GetOrderById(OrderId);
                var accountContact = AccountServices.GetAllValidAccountContactsByAccountId(orders?.Account?.AccountID ?? 0, CurrentTenantId).Where(u => u.ConTypeInvoices = true).ToList();
                if (accountContact.Count > 0)
                {
                    var selected = accountContact.Select(u => u.AccountContactId.ToString()).ToList();
                    var email = accountContact.Select(u => new
                    {
                        AccountContactId = u.AccountContactId,
                        ContactEmail = u.ContactEmail

                    }).ToList();
                    ViewBag.AccountResult = new MultiSelectList(email, "AccountContactId", "ContactEmail", selected);
                }
                else
                {
                    ViewBag.AccountResult = new MultiSelectList(accountContact.ToList(), "AccountContactId", "ContactEmail", accountContact?.FirstOrDefault()?.AccountContactId.ToString());
                }

            }

            return PartialView("EmailRecipientsPartial");

        }


        public async Task<JsonResult> SaveCustomEmails(OrderRecipientInfo shipmentAndRecipientInfo)

        {

            if (string.IsNullOrEmpty(shipmentAndRecipientInfo.CustomRecipients) && shipmentAndRecipientInfo.AccountEmailContacts == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Order = OrderService.GetOrderById(shipmentAndRecipientInfo.orderId ?? 0);
                if (shipmentAndRecipientInfo.InventoryTransactionType == 8)
                {
                    var orderprocess = OrderService.GetOrderProcessByOrderProcessId(shipmentAndRecipientInfo.orderId ?? 0);
                    Order = orderprocess.Order;
                }
                AccountServices.UpdateOrderPTenantEmailRecipients(shipmentAndRecipientInfo.AccountEmailContacts, Order.OrderID, CurrentUserId);

                if (shipmentAndRecipientInfo.InventoryTransactionType == 8)
                {


                    var report = CreateDeliveryNotePrint(shipmentAndRecipientInfo.orderId ?? 0, null);
                    PrepareDirectory("~/UploadedFiles/reports/dn/");
                    var reportPath = "~/UploadedFiles/reports/dn/" + Order.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Sales Order Confirmation", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: WorksOrderNotificationTypeEnum.SalesOrderUpdateTemplate);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.SalesOrder)
                {
                    var report = CreateSalesOrderPrint(Order.OrderID);
                    PrepareDirectory("~/UploadedFiles/reports/so/");
                    var reportPath = "~/UploadedFiles/reports/so/" + Order.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Sales order", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: WorksOrderNotificationTypeEnum.SalesOrderTemplate);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var report = CreatePurchaseOrderPrint(Order.OrderID);
                    PrepareDirectory("~/UploadedFiles/reports/po/");
                    var reportPath = "~/UploadedFiles/reports/po/" + Order.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Purchase Order", _mapper.Map(Order, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: WorksOrderNotificationTypeEnum.PurchaseOrderTemplate);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public async Task<JsonResult> SendEmailReport(OrderRecipientInfo shipmentAndRecipientInfo)
        {

            if (string.IsNullOrEmpty(shipmentAndRecipientInfo.CustomRecipients) && shipmentAndRecipientInfo.AccountEmailContacts == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {

                if (shipmentAndRecipientInfo.AccountEmailContacts != null)
                {
                    var accountContact = _accountServices.GetAllValidAccountContactsByAccountContactIds(shipmentAndRecipientInfo.AccountEmailContacts).ToList();
                    var accountContactEmails = string.Join(";", accountContact.Select(u => u.ContactEmail));
                    shipmentAndRecipientInfo.CustomRecipients = string.IsNullOrEmpty(accountContactEmails) ? shipmentAndRecipientInfo.CustomRecipients : (shipmentAndRecipientInfo.CustomRecipients + ";" + accountContactEmails);
                }

                if (shipmentAndRecipientInfo.orderId.HasValue)
                {
                    var orders = OrderService.GetOrderById(shipmentAndRecipientInfo.orderId ?? 0);
                    var report = CreateSalesOrderPrint((shipmentAndRecipientInfo.orderId ?? 0), true);
                    PrepareDirectory("~/UploadedFiles/reports/dso/");
                    var reportPath = "~/UploadedFiles/reports/dso/" + orders.OrderNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{orders.OrderNumber} - Direct Sales Order Report", _mapper.Map(orders, new OrderViewModel()), reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                         worksOrderNotificationType: WorksOrderNotificationTypeEnum.DirectSalesOrderReportTemplate);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var report = CreateFinancialTransactionReport(shipmentAndRecipientInfo.AccountId, shipmentAndRecipientInfo.StartDate, shipmentAndRecipientInfo.EndDate);
                    PrepareDirectory("~/UploadedFiles/reports/AS/");
                    var reportPath = "~/UploadedFiles/reports/AS/ASR" + shipmentAndRecipientInfo.AccountId + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));
                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"Account Statement Report ", null, reportPath, shipmentAndRecipientInfo: shipmentAndRecipientInfo,
                        worksOrderNotificationType: WorksOrderNotificationTypeEnum.AccountStatementTemplate, TenantId: CurrentTenantId, accountId: shipmentAndRecipientInfo.AccountId);
                    return Json(result, JsonRequestBehavior.AllowGet);

                }


            }
        }


        public ActionResult UpdateDeliveryStatus(int? orderProcessId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = OrderService.UpdateOrderProcessStatus(orderProcessId ?? 0, CurrentUserId);

            if (result)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }


        }


        public ActionResult _EditDelivery()
        {
            if (!string.IsNullOrEmpty(Request.Params["OrderprocessDetailId"]))
            {
                if (!string.IsNullOrEmpty(Request.Params["OrderprocessDetailId"]))
                {
                    decimal Qty = 0;
                    int PalletTrackingId = int.Parse(string.IsNullOrEmpty(Request.Params["PalletTrackingId"]) ? "0" : Request.Params["PalletTrackingId"].ToString());
                    int OrderProcessDetailId = int.Parse(Request.Params["OrderprocessDetailId"]);
                    var orderprocessDetail = OrderService.GetOrderProcessDetailById(OrderProcessDetailId);
                    if (PalletTrackingId > 0 && !string.IsNullOrEmpty("ProductType"))
                    {
                        Qty = _productServices.GetInventoryTransactionsByPalletTrackingId(PalletTrackingId, OrderProcessDetailId);
                        orderprocessDetail.QtyProcessed = Qty;
                    }

                    return View(orderprocessDetail);
                }
            }
            return View();


        }

        public ActionResult InventoryTransactionEdit(int OrderprocessDetailId)
        {
            ViewBag.OrderProcessDetailId = OrderprocessDetailId;
            var orderProcessDetail =
                _productServices.GetAllPalletByOrderProcessDetailId(OrderprocessDetailId, CurrentTenantId);

            return PartialView("_DelShipEdit", orderProcessDetail);
        }

        public ActionResult EditSerialProduct(int OrderProcessDetailId, bool? type)
        {
            ViewBag.OrderProcessDetailId = OrderProcessDetailId;
            var orderProcessDetail =
                _productServices.GetAllProductSerialbyOrderProcessDetailId(OrderProcessDetailId, CurrentTenantId, type).Select(u => new
                {
                    u.ProductMaster.Name,
                    u.SerialID,
                    u.SerialNo,
                    u.DateCreated
                });
            return PartialView("_SerialEdit", orderProcessDetail);

        }

        public JsonResult UpdateOrderProcessDetail(int OrderProcessDetailId, decimal Quantity, int? serialId, int? PalletTrackingId, bool? wastedReturn)
        {

            string status = OrderService.UpdateOrderProcessDetail(OrderProcessDetailId, Quantity, CurrentUserId, CurrentTenantId, serialId, PalletTrackingId, wastedReturn);


            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDeliveryAddress()
        {
            int ProcessId = int.Parse(!string.IsNullOrEmpty(Request.Params["ProcessId"]) ? Request.Params["ProcessId"] : "0");
            bool editStatus = bool.Parse(!string.IsNullOrEmpty(Request.Params["editdelivery"]) ? Request.Params["editdelivery"] : "false");
            ViewBag.DeliveryEdit = editStatus;
            var data = OrderService.GetOrderProcessByOrderProcessId(ProcessId);
            return View(data);
        }
        public JsonResult UpdateDeliveryAddress(AccountShipmentInfo accountShipmentInfo)
        {
            accountShipmentInfo.UserId = CurrentUserId;
            var status = OrderService.UpdateDeliveryAddress(accountShipmentInfo);
            return Json(status);
        }


        public ActionResult _AssignPicker(int OrderId)
        {
            ViewBag.OrderIds = OrderId;
            var UserId = OrderService.GetOrderById(OrderId).PickerId;
            ViewBag.Picker = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "DisplayName", UserId);
            return View();
        }

        public JsonResult SaveAssignPicker(int OrderId, int? PickerId)
        {
            var status = OrderService.UpdatePickerId(OrderId, PickerId, CurrentUserId);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

    }
}