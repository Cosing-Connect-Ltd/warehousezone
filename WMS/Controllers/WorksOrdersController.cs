using AutoMapper;
using Elmah;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class WorksOrdersController : BaseReportsController
    {
        private readonly IAppointmentsService _appointmentsService;
        private readonly IGaneConfigurationsHelper _ganeConfigurationsHelper;
        private readonly IMapper _mapper;

        public WorksOrdersController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAppointmentsService appointmentsService,
            IGaneConfigurationsHelper ganeConfigurationsHelper, IEmailServices emailServices, ITenantLocationServices tenantLocationservices, ITenantsServices tenantsServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, ganeConfigurationsHelper, emailServices, tenantLocationservices, tenantsServices)
        {
            _appointmentsService = appointmentsService;
            _ganeConfigurationsHelper = ganeConfigurationsHelper;
            _mapper = mapper;
        }

        // GET: WorksOrders
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Error = TempData["Error"];
            return View();
        }

        public ActionResult Create(int? Id, string pageToken)
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (Id == null)
            {
                Order NewOrder = new Order();
                NewOrder.OrderNumber = GeneratePO();
                SetViewBagItems();

                List<object> expecHours = new List<object>();
                for (int ctr = 1; ctr <= 10; ctr++)
                {
                    expecHours.Add(new { ExpectedHours = ctr });
                }

                ViewBag.AuthUserId = CurrentUserId;
                ViewBag.AccountContacts = new SelectList(AccountServices.GetAllTopAccountContactsByTenantId(CurrentTenantId), "AccountContactId", "ContactName");
                ViewBag.OrderDetails = new List<OrderDetail>();
                ViewBag.OrderProcesses = new List<OrderProcess>();
                if (string.IsNullOrEmpty(pageToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }
                return View(NewOrder);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Order Order, OrderRecipientInfo shipmentAndRecipientInfo)
        {
            // get properties of tenant
            try
            {
                if (ModelState.IsValid)
                {
                    var tempList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                    var orderNotesList = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);

                    OrderService.CreateWorksOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(tempList, new List<OrderDetail>()), orderNotesList);

                    var result = await _ganeConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Works order logged", _mapper.Map(Order, new OrderViewModel()),
                        sendImmediately: true, shipmentAndRecipientInfo: shipmentAndRecipientInfo, worksOrderNotificationType: WorksOrderNotificationTypeEnum.WorksOrderLogTemplate);

                    if (result != "Success")
                    {
                        TempData["Error"] = result;
                    }

                    return AnchoredOrderIndex("WorksOrders", "Index", ViewBag.Fragment as string);
                }
                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return View(Order);
            }
            catch (Exception Exp)
            {
                if (Exp.InnerException != null && Exp.InnerException.Message == "Duplicate Order Number")
                {
                    ModelState.AddModelError("OrderNumber", Exp.Message);
                }
                else if (Exp.InnerException != null && Exp.InnerException.Message.Contains("Template"))
                {
                    ModelState.AddModelError("PropertyTenantIds", Exp.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Exp.Message);
                }

                SetViewBagItems();
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return View(Order);
            }
        }

        public ActionResult Edit(int? id, string pageToken)
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Order = OrderService.GetOrderById(id.Value);
            if (Order == null)
            {
                return HttpNotFound();
            }

            SetViewBagItems(Order);

            ViewBag.AccountContacts = new SelectList(AccountServices.GetAllValidAccountContactsByAccountId(Order.AccountID ?? 0, CurrentTenantId), "AccountContactId", "ContactName");

            if (string.IsNullOrEmpty(pageToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }

            var odList = OrderService.GetAllValidOrderDetailsByOrderId(id.Value).ToList();
            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(odList, new List<OrderDetailSessionViewModel>()));

            var orderNotes = Order.OrderNotes.Where(a => a.IsDeleted != true).ToList();
            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, orderNotes);

            ViewBag.Notes = new SelectList(Order.OrderNotes.Where(a => a.IsDeleted != true).Select(a => a.Notes).ToList());

            return View(Order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Order Order, OrderRecipientInfo shipmentAndRecipientInfo, string orderSaveAndComplete, string orderSaveAndProcess)
        {
            var orderCompleted = false;
            if (orderSaveAndComplete == "1")
            {
                Order.OrderStatusID = OrderStatusEnum.Complete;
                orderCompleted = true;
            }

            if (ModelState.IsValid)
            {
                var tempList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                var orderNotesList = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);
                OrderService.SaveWorksOrder(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, orderCompleted, _mapper.Map(tempList, new List<OrderDetail>()), orderNotesList);
                if (orderCompleted)
                {
                    try
                    {
                        if (shipmentAndRecipientInfo.SendEmailWithAttachment)
                        {
                            var result = await _ganeConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Works order completed", _mapper.Map(Order, new OrderViewModel()),
                                 sendImmediately: true, shipmentAndRecipientInfo: shipmentAndRecipientInfo, worksOrderNotificationType: WorksOrderNotificationTypeEnum.WorksOrderCompletedTemplate);

                            if (result != "Success")
                            {
                                TempData["Error"] = result;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Exception while creating tenant email notification queue - " + ex.Message.ToString(), ex.InnerException);
                    }
                }

                ViewBag.Fragment = Request.Form["fragment"];

                if (orderSaveAndProcess == "1")
                {
                    return Redirect(Url.RouteUrl(new { controller = "WorksOrders", action = "ProcessOrder", id = Order.OrderID }) + "?fragment=" + ViewBag.Fragment as string);
                }

                return AnchoredOrderIndex("WorksOrders", "Index", ViewBag.Fragment as string);
            }

            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = shipmentAndRecipientInfo.PageSessionToken ?? Guid.NewGuid().ToString();
            SetViewBagItems(Order);

            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "SupplierID", "CompanyName", Order.AccountID);
            return View(Order);
        }

        public ActionResult CreateBulkOrder(Guid? id, string pageToken, bool? layout = false, bool? returnViews = false)
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Layout = layout;
            ViewBag.returnView = returnViews;
            var groupToken = id ?? Guid.NewGuid();

            var NewOrder = OrderService.GetAllOrdersByGroupToken(groupToken, CurrentTenantId).FirstOrDefault() ?? new Order();


            NewOrder.OrderNumber = GeneratePO();
            SetViewBagItems();

            List<object> expecHours = new List<object>();
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                expecHours.Add(new { ExpectedHours = ctr });
            }

            ViewBag.OrderGroupToken = groupToken.ToString();
            ViewBag.AuthUserId = CurrentUserId;
            ViewBag.AccountContacts = new SelectList(AccountServices.GetAllTopAccountContactsByTenantId(CurrentTenantId), "AccountContactId", "ContactName");
            ViewBag.OrderDetails = new List<OrderDetail>();
            ViewBag.OrderProcesses = new List<OrderProcess>();

            if (string.IsNullOrEmpty(pageToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }

            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderDetailSessionViewModel>());
            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderNotes>());

            return View("CreateBulk", NewOrder);
        }

        public ActionResult CreateBulk()
        {
            return RedirectToAction("CreateBulkOrder", new { id = Guid.NewGuid() });
        }

        public async Task<ActionResult> SaveOrdersBulk(Order Order, FormCollection formCollection, OrderRecipientInfo shipmentAndRecipientInfo)
        {
            try
            {
                if (Order.OrderGroupToken.HasValue && shipmentAndRecipientInfo.SendEmailWithAttachment)
                {
                    var orders = OrderService.GetAllOrdersByGroupToken(Order.OrderGroupToken.Value, CurrentTenantId).ToList();

                    foreach (var order in orders)
                    {
                        try
                        {
                            var result = await _ganeConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Works order logged", _mapper.Map(order, new OrderViewModel()),
                                sendImmediately: true, shipmentAndRecipientInfo: shipmentAndRecipientInfo);

                            if (result != "Success")
                            {
                                TempData["Error"] = result;
                            }

                        }
                        catch (Exception ex)
                        {
                            ErrorSignal.FromCurrentContext().Raise(ex);
                            throw ex;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Form["Viewreturn"]))
                {
                    bool? returnView = Convert.ToBoolean(Request.Form["Viewreturn"]);
                    if (returnView == true)
                    {
                        return AnchoredOrderIndex("PProperties", "Index", "AO");
                    }
                }
                return AnchoredOrderIndex("WorksOrders", "Index", ViewBag.Fragment as string);

            }
            catch (Exception Exp)
            {
                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                ModelState.AddModelError("", Exp.Message);
                return View("CreateBulkSingle");
            }
        }

        public ActionResult CreateBulkSingle(string pageToken)
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Order NewOrder = new Order();
            NewOrder.OrderNumber = GeneratePO();
            SetViewBagItems();

            List<object> expecHours = new List<object>();
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                expecHours.Add(new { ExpectedHours = ctr });
            }

            ViewBag.OrderGroupToken = Guid.NewGuid().ToString();
            ViewBag.AuthUserId = CurrentUserId;
            ViewBag.AccountContacts = new SelectList(AccountServices.GetAllTopAccountContactsByTenantId(CurrentTenantId), "AccountContactId", "ContactName");
            ViewBag.OrderDetails = new List<OrderDetail>();
            ViewBag.OrderProcesses = new List<OrderProcess>();

            if (string.IsNullOrEmpty(pageToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }
            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderDetailSessionViewModel>());
            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, new List<OrderNotes>());

            return PartialView(NewOrder);
        }

        public ActionResult SaveOrdersBulkSingle(Order Order, OrderRecipientInfo shipmentAndRecipientInfo)
        {
            // get properties of tenant
            try
            {
                if (ModelState.IsValid)
                {
                    var orderDetailsList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                    var orderNotesList = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);
                    OrderService.SaveWorksOrderBulkSingle(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(orderDetailsList, new List<OrderDetail>()), orderNotesList);

                    GaneOrderDetailsSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    GaneOrderNotesSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    return Json(new { success = true, message = "Saved" }, JsonRequestBehavior.AllowGet);

                }
                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return Json(new { success = false, message = "Model state not valid, failed to save!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Exp)
            {
                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                ModelState.AddModelError("", Exp.Message);
                return Json(new { success = false, message = Exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateOrdersBulkSingle(Order Order, OrderRecipientInfo shipmentAndRecipientInfo)
        {
            // get properties of tenant
            try
            {
                if (ModelState.IsValid)
                {
                    var orderDetailsList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(shipmentAndRecipientInfo.PageSessionToken);
                    var orderNotesList = GaneOrderNotesSessionHelper.GetOrderNotesSession(shipmentAndRecipientInfo.PageSessionToken);
                    OrderService.UpdateWorksOrderBulkSingle(Order, shipmentAndRecipientInfo, CurrentTenantId, CurrentWarehouseId, CurrentUserId, _mapper.Map(orderDetailsList, new List<OrderDetail>()), orderNotesList);
                    GaneOrderDetailsSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    GaneOrderNotesSessionHelper.ClearSessionTokenData(shipmentAndRecipientInfo.PageSessionToken);
                    return Json(new { success = true, message = "Saved" }, JsonRequestBehavior.AllowGet);

                }

                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;

                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                return Json(new { success = false, message = "Model state not valid, failed to save!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Exp)
            {
                SetViewBagItems();
                ViewBag.OrderDetails = Order.OrderDetails;
                ViewBag.OrderProcesses = Order.OrderProcess;
                ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
                ModelState.AddModelError("", Exp.Message);
                return Json(new { success = false, message = Exp.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditBulkSingle(int id, string PageSessionToken)
        {
            SetViewBagItems();

            List<object> expecHours = new List<object>();
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                expecHours.Add(new { ExpectedHours = ctr });
            }

            var order = OrderService.GetOrderById(id);

            ViewBag.OrderGroupToken = order.OrderGroupToken;
            ViewBag.AuthUserId = CurrentUserId;
            ViewBag.AccountContacts = new SelectList(AccountServices.GetAllTopAccountContactsByTenantId(CurrentTenantId), "AccountContactId", "ContactName");
            ViewBag.OrderDetails = order.OrderDetails;
            ViewBag.OrderProcesses = new List<OrderProcess>();

            if (string.IsNullOrEmpty(PageSessionToken))
            {
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
            }
            else
            {
                ViewBag.ForceRegeneratedPageToken = PageSessionToken;
            }

            var details = OrderService.GetAllValidOrderDetailsByOrderId(id).ToList();

            GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(details, new List<OrderDetailSessionViewModel>()));
            GaneOrderNotesSessionHelper.SetOrderNotesSessions(ViewBag.ForceRegeneratedPageToken, order.OrderNotes.Where(m => m.IsDeleted != true).ToList());

            return PartialView(order);
        }


        public string GeneratePO()
        {
            return GenerateNextOrderNumber(InventoryTransactionTypeEnum.WorksOrder);
        }

        public ActionResult PrintWorksOrderByAppointment(int? id)
        {
            if (id.HasValue)
            {
                var appointment = _appointmentsService.GetAppointmentById(id.Value);
                if (appointment != null)
                {
                    return RedirectToAction("WorksOrderPrint", "Reports", new { id = appointment.OrderId });
                }
            }
            return RedirectToAction("Index", "Appointments");
        }

        public async Task<ActionResult> DispatchNotifications(int tenantId)
        {
            await _ganeConfigurationsHelper.DispatchTenantEmailNotificationQueues(tenantId);
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> DailyChecks()
        {
            await GaneConfigurationsHelper.UpdateIsCurrentTenantFlags();

            await PropertyService.UpdateAllPropertyTenantsFlags();

            return RedirectToAction("Index", "Home");
        }

        public JsonResult GetPropertyTenants(int id, int orderid = 0)
        {
            var allPropertyTenants = PropertyService.GetAllCurrentTenants(id).ToList();
            var orderSelectedTenantIds = new List<int>();
            if (orderid != 0)
            {
                orderSelectedTenantIds = PropertyService.GetAppointmentRecipientTenants(orderid).Select(m => m.PTenantId).ToList();
            }
            var proprtyTenants = allPropertyTenants.Select(m => new SelectListItem() { Text = m.TenantFullName, Value = m.PTenantId.ToString(), Selected = orderSelectedTenantIds.Contains(m.PTenantId) });
            return Json(proprtyTenants, JsonRequestBehavior.AllowGet);
        }


        public class EmailTenantViewModel
        {
            public int TenantId { get; set; }
            public int OrderId { get; set; }
        }

        [HttpPost]
        public JsonResult GeEmailForPTenant(EmailTenantViewModel model)
        {
            var tenant = PropertyService.GetPropertyTenantById(model.TenantId);
            var result = string.Empty;
            if (tenant != null)
            {
                result = tenant.Email;

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        ///Reports//12
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

            var orderDto = _mapper.Map<ReceivePOVM>(Order);
            orderDto.InventoryTransactionTypeId = (int)InventoryTransactionTypeEnum.WorksOrder;

            var appt = Order.Appointmentses.Where(m => !m.IsCanceled)
                .OrderByDescending(m => m.AppointmentId)
                .FirstOrDefault();
            if (appt != null)
            {
                var resource = _appointmentsService.GetAppointmentResourceById(appt.ResourceId.Value);
                if (resource != null)
                {
                    orderDto.WorksResourceName = resource.Name;
                }
            }

            if (TempData["AutoCompleteError"] != null)
            {
                ViewBag.Warning = TempData["AutoCompleteError"].ToString();
            }
            //get http refferer
            if (Request.UrlReferrer != null)
            {
                string Referrer = Request.UrlReferrer.ToString();

                ViewBag.RController = "WorksOrders";

                ViewBag.RAction = "Index";
                if (Referrer.Contains("PickList"))
                {
                    ViewBag.RController = "PickList";
                    ViewBag.RAction = "Index";
                }
            }

            return View(orderDto);
        }
        public ActionResult _WorksOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setName = "gvWODetail";
            ViewBag.route = new { Controller = "WorksOrders", Action = "_WorksOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            return PartialView("_WorksOrderDetails", OrderService.GetWorksOrderDetails(Id, CurrentTenantId));
        }
        #region Popups
        public ActionResult _Property()
        {

            var landlords = PropertyService.GetAllValidPropertyLandlords().Select(m => new SelectListItem() { Value = m.PLandlordId.ToString(), Text = m.LandlordFullname + "(" + m.LandlordCode + ")" });
            ViewBag.CurrentLandlordIds = new SelectList(landlords, "Value", "Text");
            var pTenents = (from ptnts in PropertyService.GetAllCurrentTenants()
                            select new
                            {
                                ptnts.PTenantId,
                                ptnts.TenantFullName

                            }).ToList();
            ViewBag.CurrentPTenentIds = new SelectList(pTenents, "PTenantId", "TenantFullName");
            return PartialView();
        }

        public JsonResult _PropertySubmit(PProperty model)
        {
            try
            {

                model = PropertyService.SavePProperty(model, CurrentUserId);
                return Json(new { error = false, id = model.PPropertyId, code = model.PropertyCode });
            }
            catch
            {
                return Json(new { error = true });

            }
        }
        public ActionResult _Landlord()
        {
            return PartialView();
        }
        public JsonResult _LandlordSubmit(PLandlord model)
        {
            try
            {
                model = PropertyService.SavePLandlord(model, CurrentUserId);
                return Json(new { error = false, id = model.PLandlordId, code = model.LandlordCode });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, msg = exp.Message });
            }
        }
        public ActionResult _PTenent()
        {
            return PartialView();
        }
        public JsonResult _PTenentSubmit(PTenant model)
        {
            try
            {
                model = PropertyService.SavePTenant(model, CurrentUserId);
                return Json(new { error = false, id = model.PTenantId, code = model.TenantFullName });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, msg = exp.Message });
            }
        }
        #endregion
    }
}