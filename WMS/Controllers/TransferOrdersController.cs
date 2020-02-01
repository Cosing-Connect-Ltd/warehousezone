using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class TransferOrdersController : BaseController
    {
        private readonly IMapper _mapper;

        public TransferOrdersController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _mapper = mapper;
        }
        // GET: TransferOrder
        public ActionResult Index()
        {
            ViewBag.trasnferOrder = true;
            return View();
        }
        public ActionResult Create(int? Id, string pageSessionToken)
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            // get properties of tenant
            if (Id != null)
            {
                Order NewOrder = new Order();
                NewOrder.OrderNumber = GenerateNextOrderNumber(InventoryTransactionTypeEnum.TransferIn);
                NewOrder.IssueDate = DateTime.Today;
                NewOrder.InventoryTransactionTypeId = (int)Id;
                SetViewBagItems(Id);
                ViewBag.OrderDetails = new List<OrderDetail>();
                ViewBag.OrderProcesses = new List<OrderProcess>();

                if (string.IsNullOrEmpty(pageSessionToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }

                return View(NewOrder);
            }
            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = pageSessionToken ?? Guid.NewGuid().ToString();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order Order, string pageSessionToken)
        {
            // get properties of tenant
            try
            {
                var orderDetailsList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(pageSessionToken);
                OrderService.CreateTransferOrder(Order, _mapper.Map(orderDetailsList, new List<OrderDetail>()), CurrentTenantId, CurrentWarehouseId, CurrentUserId);
                return AnchoredOrderIndex("TransferOrders", "Index", ViewBag.Fragment as string);
            }
            catch (Exception exp)
            {
                if (exp.InnerException != null && exp.InnerException.Message == "Duplicate Order Number")
                {
                    ModelState.AddModelError("OrderNumber", exp.Message);
                }
                else
                {
                    ModelState.AddModelError("", exp.Message);
                }
                ViewBag.ForceRegeneratePageToken = "True";
                ViewBag.ForceRegeneratedPageToken = pageSessionToken ?? Guid.NewGuid().ToString();
                SetViewBagItems(Order.TransactionType.InventoryTransactionTypeId);
                return View(Order);
            }
        }
        public ActionResult Edit(int? id, string pageSessionToken)
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

            SetViewBagItems(Order.TransactionType.InventoryTransactionTypeId);
            var odList = OrderService.GetAllValidOrderDetailsByOrderId(id.Value).ToList();
            if (odList.Count > 0)
            {
                if (string.IsNullOrEmpty(pageSessionToken))
                {
                    ViewBag.ForceRegeneratePageToken = "True";
                    ViewBag.ForceRegeneratedPageToken = Guid.NewGuid().ToString();
                }
                GaneOrderDetailsSessionHelper.SetOrderDetailSessions(ViewBag.ForceRegeneratedPageToken, _mapper.Map(odList, new List<OrderDetailSessionViewModel>()));
            }
            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = pageSessionToken ?? Guid.NewGuid().ToString();
            return View(Order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order Order, string PageSessionToken)
        {
            if (ModelState.IsValid)
            {
                var orderDetailsList = GaneOrderDetailsSessionHelper.GetOrderDetailSession(PageSessionToken);
                OrderService.SaveTransferOrder(Order, _mapper.Map(orderDetailsList, new List<OrderDetail>()), CurrentTenantId, CurrentWarehouseId, CurrentUserId);
                return AnchoredOrderIndex("TransferOrders", "Index", ViewBag.Fragment as string);
            }
            ViewBag.ForceRegeneratePageToken = "True";
            ViewBag.ForceRegeneratedPageToken = PageSessionToken ?? Guid.NewGuid().ToString();
            ViewBag.SupplierID = new SelectList(AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier), "AccountID", "CompanyName", Order.AccountID);
            return View(Order);
        }
        public ActionResult _TransferOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setname = "gvTransferOrderDetails";
            ViewBag.route = new { Controller = "TransferOrders", Action = "_TransferOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            VerifyOrderStatus(Id);

            return PartialView("_TransferOrderDetails", OrderService.GetTransferOrderDetails(Id, CurrentWarehouseId));
        }
        public ActionResult ProcessOrder(int? id)
        {
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
            VerifyOrderStatus(Order.OrderID);
            ViewBag.Consignments = new SelectList(OrderService.GetAllValidConsignmentTypes(CurrentTenantId), "ConsignmentTypeId", "ConsignmentType");
            var orderDto = new VModels.TransferOrderVM
            {
                OrderID = Order.OrderID,
                OrderNumber = Order.OrderNumber,
                DeliveryNumber = GaneStaticAppExtensions.GenerateDateRandomNo(),
                Type = Order.TransactionType.InventoryTransactionTypeId,
                InventoryTransactionTypeId = Order.TransactionType.InventoryTransactionTypeId,
                Warehouse = Order.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferIn ? Order.TransferWarehouse.WarehouseName : Order.TransferWarehouse.WarehouseName
            };

            return View(orderDto);
        }

     

        public ActionResult DeleteTransferOrder(int? id)
        {
            if (!caSession.AuthoriseSession()){ return Json(false, JsonRequestBehavior.AllowGet); }
            OrderService.DeleteTransferOrder(id??0,CurrentTenantId, CurrentWarehouseId, CurrentUserId);
            return Json(true,JsonRequestBehavior.AllowGet); ;
        }

        private void SetViewBagItems(int? Id)
        {
            var whouses = LookupServices.GetAllWarehousesForTenant(CurrentTenantId, CurrentWarehouseId).Select(
                whs => new
                {
                    TransferWarehouseId = whs.WarehouseId,
                    WarehouseName = whs.WarehouseName
                }).ToList();


            ViewBag.Warehouses = new SelectList(whouses, "TransferWarehouseId", "WarehouseName");

            ViewBag.AuthUsers = new SelectList(OrderService.GetAllAuthorisedUsers(CurrentTenantId), "UserId", "UserName", CurrentUserId);
            ViewBag.Departments = new SelectList(LookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
        }

        public ActionResult Consignments()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult _Consignments()
        {
            var data = OrderService.GetOrderProcessConsignmentsForWarehouse(CurrentWarehouseId, CurrentTenantId)
                       .Select(ops => new
                       {
                           ops.DeliveryNO,
                           ops.OrderID,
                           ops.DateCreated,
                           ops.OrderProcessID,
                           ops.Order.OrderNumber,
                       }).ToList();
            return PartialView(data);
        }

        public ActionResult _ConsignmentDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = OrderService.GetOrderProcessDetailsByProcessId(ProcessId)
                .Select(opd => new
                {
                    opd.ProductMaster.Name,
                    opd.ProductMaster.SKUCode,
                    opd.QtyProcessed,
                    opd.DateCreated,
                    opd.OrderProcessDetailID
                }).ToList();
            return PartialView(data);

        }

        public ActionResult Deliveries()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public ActionResult _Deliveries()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var data = OrderService.GetOrderProcesssDeliveriesForWarehouse(CurrentWarehouseId, CurrentTenantId).Select(
                ops => new
                {
                    ops.DeliveryNO,
                    ops.OrderID,
                    ops.DateCreated,
                    ops.OrderProcessID,
                    ops.Order.OrderNumber,
                }).ToList();
            return PartialView(data);
        }

        public ActionResult _DeliveryDetails(int ProcessId)
        {
            ViewBag.processId = ProcessId;

            var data = (from opd in OrderService.GetOrderProcessDetailsByProcessId(ProcessId)
                        select new
                        {
                            opd.ProductMaster.Name,
                            opd.ProductMaster.SKUCode,
                            opd.QtyProcessed,
                            opd.DateCreated,
                            opd.OrderProcessDetailID
                        }).ToList();
            return PartialView(data);
        }
    }
}