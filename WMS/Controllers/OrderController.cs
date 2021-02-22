using AutoMapper;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using Elmah;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class OrderController : BaseReportsController
    {
        private readonly IProductLookupService _productLookupService;
        private readonly IProductServices _productServices;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IProductPriceService _productPriceService;
        private readonly ICommonDbServices _commonDbServices;
        private readonly IEmailServices _emailServices;
        private readonly IPalletingService _palletingService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public OrderController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAppointmentsService appointmentsService, IPalletingService palletingService,
            IProductLookupService productLookupService, IProductServices productServices, ITenantLocationServices tenantLocationServices, IProductPriceService productPriceService, IGaneConfigurationsHelper ganeConfigurationsHelper,
            IEmailServices emailServices, ICommonDbServices commonDbServices, ITenantLocationServices tenantLocationservices, ITenantsServices tenantsServices, IUserService userService, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, ganeConfigurationsHelper, emailServices, tenantLocationservices, tenantsServices)
        {
            _productLookupService = productLookupService;
            _productServices = productServices;
            _tenantLocationServices = tenantLocationServices;
            _productPriceService = productPriceService;
            _commonDbServices = commonDbServices;
            _emailServices = emailServices;
            _palletingService = palletingService;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: /PO/
        public ActionResult Index(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (Id.HasValue)
            {
                var Order = OrderService.FinishinghOrder(Id.Value, CurrentUserId, CurrentTenantId);
            }

            return View();
        }

        public async Task<ActionResult> Complete(int? id, string frag, string referrerController = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Order = OrderService.CompleteOrder(id.Value, CurrentUserId);
            if (Order == null)
            {
                return HttpNotFound();
            }

            if (Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder)
            {
                var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{Order.OrderNumber} - Works order completed", _mapper.Map(Order, new OrderViewModel()), sendImmediately: true,
                    worksOrderNotificationType: WorksOrderNotificationTypeEnum.WorksOrderCompletedTemplate);

                if (result != "Success")
                {
                    TempData["Error"] = result;
                }
            }

            string controller = "PurchaseOrders";

            if (referrerController == "PickList")
            {
                controller = "PickList";
            }

            if (frag != null) { frag = "#" + frag; }

            return Redirect(Url.Action("Index", controller) + frag);
        }

        public async Task<JsonResult> CanAutoCompleteOrder(int id)
        {
            var suffix = "SO";
            try
            {
                var order = OrderService.GetOrderById(id);
                suffix = GetAnchorForInventoryTransactionTypeId(order.InventoryTransactionTypeId);
                var orderDetails = order.OrderDetails.Where(u => u.IsDeleted != true);
                if (order != null && orderDetails.All(m => m.ProcessedQty >= m.Qty || m.ProductMaster.IsAutoShipment == true || m.ProductMaster.ProductType == ProductKitTypeEnum.Virtual))
                {
                    if (orderDetails.All(m => m.ProcessedQty >= m.Qty))
                    {
                        OrderService.OrderProcessAutoComplete(id, string.Empty, CurrentUserId, false, true);
                    }
                    else
                    {
                        await AutoCompleteOrder(new AutoCompleteOrderViewModel
                        {
                            OrderID = id,
                            ForceComplete = true,
                            IncludeProcessing = true,
                            DeliveryNumber = GaneStaticAppExtensions.GenerateDateRandomNo()
                        });
                    }

                    return Json(new { Success = true, Suffix = suffix }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, Suffix = suffix }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message, Suffix = suffix }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> AutoCompleteOrder(AutoCompleteOrderViewModel model)
        {
            var suffix = "SO";
            try
            {
                var order = OrderService.GetOrderById(model.OrderID);
                suffix = GetAnchorForInventoryTransactionTypeId(order.InventoryTransactionTypeId);
                var result = OrderService.OrderProcessAutoComplete(model.OrderID, model.DeliveryNumber, CurrentUserId, model.IncludeProcessing, model.ForceComplete);
                if (result == null)
                {
                    return Json(new { RequiresProcessing = true, Success = false, Message = "There are still some items that are not dispatched for this order. What would you like to do?", Suffix = suffix }, JsonRequestBehavior.AllowGet);
                }
                if (order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WorksOrder)
                {
                    await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Works order completed", _mapper.Map(order, new OrderViewModel()), sendImmediately: true,
                        worksOrderNotificationType: WorksOrderNotificationTypeEnum.WorksOrderCompletedTemplate);
                }
            }
            catch (Exception ex)
            {
                TempData["AutoCompleteError"] = ex.Message + ". Please review and process manually.";
                return Json(new { Success = false, Message = ex.Message, Suffix = suffix }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Suffix = suffix }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsOrderNumberAvailable(string OrderNumber, int OrderID = 0)
        {
            var result = OrderService.IsOrderNumberAvailable(OrderNumber, OrderID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult POHistory()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public async Task<ActionResult> OrderNotification(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Order info provided is invalid.");
            }
            var orderId = int.Parse(id);
            var order = OrderService.GetOrderById(orderId);
            if (order == null)
            {
                throw new Exception("Order info provided is invalid.");
            }

            var customRecipients = Request.Params["CustomRecipients"];

            var report = new XtraReport();
            var orderTypeId = order.InventoryTransactionTypeId;
            var reportPath = "";
            var orderTypeString = "";
            var reportsDirectory = "~/UploadedFiles/reports/";
            PrepareDirectory(reportsDirectory);
            var returnController = "PurchaseOrders";
            var returnAction = "Index";
            var fragment = "PO";
            switch (orderTypeId)
            {
                case InventoryTransactionTypeEnum.PurchaseOrder:
                    report = CreatePurchaseOrderPrint(int.Parse(id));
                    PrepareDirectory(reportsDirectory + "/po");
                    reportPath = reportsDirectory + "po/" + order.OrderNumber + ".pdf";
                    orderTypeString = "Purchase order";
                    break;

                case InventoryTransactionTypeEnum.SalesOrder:
                    report = CreateSalesOrderPrint(int.Parse(id));
                    PrepareDirectory(reportsDirectory + "/so");
                    reportPath = reportsDirectory + "/so/" + order.OrderNumber + ".pdf";
                    orderTypeString = "Sales order";
                    returnController = "SalesOrder";
                    returnAction = "Index";
                    fragment = "SO";
                    break;

                case InventoryTransactionTypeEnum.WorksOrder:
                    report = CreateWorksOrderPrint(int.Parse(id));
                    PrepareDirectory(reportsDirectory + "/wo");
                    reportPath = reportsDirectory + "/wo/" + order.OrderNumber + ".pdf";
                    orderTypeString = "Works order";
                    returnController = "Order";
                    returnAction = "Index";
                    fragment = "WO";
                    break;

                default:
                    throw new Exception("Report type not supported. Type ID : + " + orderTypeId);
            }

            report.ExportToPdf(Server.MapPath(reportPath));

            var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($" #{order.OrderNumber} - {orderTypeString}", _mapper.Map(order, new OrderViewModel()), reportPath);

            if (result != "Success")
            {
                TempData["Error"] = result;
            }

            return AnchoredOrderIndex(returnController, returnAction, fragment);
        }

        /// <summary>
        /// function used to create unique Purchase Order numbers
        /// will count how many digits po has and append zero's
        /// to make 9 digit PO Number
        /// </summary>
        /// <returns></returns>
        public string GeneratePO()
        {
            return GenerateNextOrderNumber(InventoryTransactionTypeEnum.PurchaseOrder);
        }

        public JsonResult IsPONumberAvailable(string PONumber, int POID = 0)
        {
            if (!String.IsNullOrEmpty(PONumber)) PONumber = PONumber.Trim();

            var po = OrderService.GetValidSalesOrderByOrderNumber(PONumber, CurrentTenantId);

            return Json(po == null || po.OrderID == POID, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PrintPO(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order po = OrderService.GetOrderById(id);

            return View(po);
        }

        public ActionResult GetNextOrderNumber(string id)
        {
            return Json(GenerateNextOrderNumber(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult _PurchaseOrders(int? type)
        {
            ViewBag.type = type;

            if (type == 1)
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView");

                if (viewModel == null)
                    viewModel = OrdersCustomBinding.CreatePurchaseOrdersGridViewModel();
                return _PurchaseOrdersInProgressGridActionCore(viewModel);
            }
            else
            {
                ViewBag.name = "_PurchaseOrderListGridView_Completed";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView_Completed");

                if (viewModel == null)
                    viewModel = OrdersCustomBinding.CreatePurchaseOrdersGridViewModel();

                return _PurchaseOrdersGridActionCore(viewModel);
            }
        }

        public ActionResult _PurchaseOrdersPaging(GridViewPagerState pager, int? type)
        {
            if (type == 1)
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView");
                viewModel.Pager.Assign(pager);
                return _PurchaseOrdersInProgressGridActionCore(viewModel);
            }
            else
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView_Completed";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView_Completed");
                viewModel.Pager.Assign(pager);
                return _PurchaseOrdersGridActionCore(viewModel);
            }
        }

        public ActionResult _PurchaseOrdersFiltering(GridViewFilteringState filteringState, int? type)
        {
            if (type == 1)
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView");
                viewModel.ApplyFilteringState(filteringState);
                return _PurchaseOrdersInProgressGridActionCore(viewModel);
            }
            else
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView_Completed";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView_Completed");
                viewModel.ApplyFilteringState(filteringState);
                return _PurchaseOrdersGridActionCore(viewModel);
            }
        }

        public ActionResult _PurchaseOrdersSorting(GridViewColumnState column, bool reset, int? type)
        {
            if (type == 1)
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView");
                viewModel.ApplySortingState(column, reset);
                return _PurchaseOrdersInProgressGridActionCore(viewModel);
            }
            else
            {
                ViewBag.type = type;
                ViewBag.name = "_PurchaseOrderListGridView_Completed";
                var viewModel = GridViewExtension.GetViewModel("_PurchaseOrderListGridView_Completed");
                viewModel.ApplySortingState(column, reset);
                return _PurchaseOrdersGridActionCore(viewModel);
            }
        }

        public ActionResult _PurchaseOrdersGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.PurchaseOrdersGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.PurchaseOrdersGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_PurchaseOrders", gridViewModel);
        }

        public ActionResult _PurchaseOrdersInProgressGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.PurchaseOrdersInProgressGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.PurchaseOrdersInProgressGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_PurchaseOrders", gridViewModel);
        }

        public ActionResult _WorksOrders(int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderListGridView");

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreatePurchaseOrdersGridViewModel();

            return _WorksOrdersGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersPaging(GridViewPagerState pager, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderListGridView");
            viewModel.Pager.Assign(pager);
            return _WorksOrdersGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersFiltering(GridViewFilteringState filteringState, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _WorksOrdersGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersSorting(GridViewColumnState column, bool reset, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderListGridView");
            viewModel.ApplySortingState(column, reset);
            return _WorksOrdersGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersGridActionCore(GridViewModel gridViewModel, int? id = null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.WorksOrdersGetDataRowCount(args, CurrentTenantId, id);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.WorksOrdersGetData(args, CurrentTenantId, id);
                    })
            );
            return PartialView("_WorksOrders", gridViewModel);
        }

        public ActionResult _WorksOrdersOnBulkCreation(string id)
        {
            if (id == null) return HttpNotFound();
            var groupToken = Guid.Parse(id);
            var model = OrderService.GetAllPendingWorksOrders(CurrentTenantId, groupToken);
            return PartialView("_WorksOrdersBulkCreated", model.ToList());
        }

        public ActionResult _WorksOrdersCompleted(int? propertyId)
        {
            ViewBag.PropertyId = propertyId;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderCompletedListGridView");
            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreatePurchaseOrdersGridViewModel();

            return _WorksOrdersCompletedGridActionCore(viewModel, propertyId);
        }

        public ActionResult _WorksOrdersCompletedPaging(GridViewPagerState pager, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderCompletedListGridView");
            viewModel.Pager.Assign(pager);
            return _WorksOrdersCompletedGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersCompletedFiltering(GridViewFilteringState filteringState, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderCompletedListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _WorksOrdersCompletedGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersCompletedSorting(GridViewColumnState column, bool reset, int? id)
        {
            ViewBag.PropertyId = id;
            var viewModel = GridViewExtension.GetViewModel("_WorksOrderCompletedListGridView");
            viewModel.ApplySortingState(column, reset);
            return _WorksOrdersCompletedGridActionCore(viewModel, id);
        }

        public ActionResult _WorksOrdersCompletedGridActionCore(GridViewModel gridViewModel, int? propertyId = null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.WorksOrdersCompletedGetDataRowCount(args, CurrentTenantId, propertyId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.WorksOrdersCompletedGetData(args, CurrentTenantId, propertyId);
                    })
            );
            return PartialView("_WorksOrdersCompleted", gridViewModel);
        }

        public ActionResult _WorksOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setname = "gvWODetails" + Id.ToString();
            ViewBag.route = new { Controller = "Order", Action = "_WorksOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            return PartialView("_WorksOrderDetails", OrderService.GetWorksOrderDetails(Id, CurrentTenantId));
        }

        public ActionResult _SalesOrders(int? type)
        {
            var tenantConfig = _tenantServices.GetTenantConfigById(CurrentTenantId);
            ViewBag.ShowDeliveryService = tenantConfig.ShowDeliveryServiceInOrdersList;
            ViewBag.ShowShopSiteName = tenantConfig.ShowExternalShopSiteNameInOrdersList;
            var viewModel = GetSalesOrdersListViewModel(type);
            return PrepareSalesOrdersListActionResult(viewModel, type);
        }

        public ActionResult _SalesOrdersPaging(GridViewPagerState pager, int? type)
        {
            SetViewBagItems();
            var viewModel = GetSalesOrdersListViewModel(type);
            viewModel.Pager.Assign(pager);
            return PrepareSalesOrdersListActionResult(viewModel, type);
        }

        public ActionResult _SalesOrdersFiltering(GridViewFilteringState filteringState, int? type)
        {
            SetViewBagItems();
            var viewModel = GetSalesOrdersListViewModel(type);
            viewModel.ApplyFilteringState(filteringState);
            return PrepareSalesOrdersListActionResult(viewModel, type);
        }

        public ActionResult _SalesOrdersSorting(GridViewColumnState column, bool reset, int? type)
        {
            SetViewBagItems();
            var viewModel = GetSalesOrdersListViewModel(type);
            viewModel.ApplySortingState(column, reset);
            return PrepareSalesOrdersListActionResult(viewModel, type);
        }

        public void SetViewBagItems()
        {
            var tenantConfig = _tenantServices.GetTenantConfigById(CurrentTenantId);
            ViewBag.ShowDeliveryService = tenantConfig.ShowDeliveryServiceInOrdersList;
            ViewBag.ShowShopSiteName = tenantConfig.ShowExternalShopSiteNameInOrdersList;
        }

        private GridViewModel GetSalesOrdersListViewModel(int? type)
        {
            var gridControlName = "_SalesOrderListGridView_Awaiting";
            switch (type)
            {
                case 1:
                    gridControlName = "_SalesOrderListGridView_Active";
                    break;

                case 2:
                    gridControlName = "_SalesOrderListGridView_Completed";
                    break;

                case 3:
                    gridControlName = "_SalesOrderListGridView_PickerAssigned";
                    break;

                case 4:
                    gridControlName = "_SalesOrderListGridView_PickerUnassigned";
                    break;
                case 5:
                    gridControlName = "_SalesOrderListGridView_PickingOnHold";
                    break;
            }

            ViewBag.Name = gridControlName;
            var viewModel = GridViewExtension.GetViewModel(gridControlName);

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreateSalesOrdersGridViewModel();
            return viewModel;
        }

        private ActionResult PrepareSalesOrdersListActionResult(GridViewModel viewModel, int? type)
        {
            ViewBag.Type = type;

            switch (type)
            {
                case 1:
                    return _SalesOrdersActiveGridActionCore(viewModel, type);

                case 2:
                    return _SalesOrdersCompletedGridActionCore(viewModel, type);

                case 3:
                    return _SalesOrdersPickerAssignedGridActionCore(viewModel);

                case 4:
                    return _SalesOrdersPickerUnassignedGridActionCore(viewModel);
                case 5:
                    return _SalesOrdersUpdatingGridActionCore(viewModel, type);

                default:
                    return _SalesOrdersAwaitingGridActionCore(viewModel, type);
            }
        }

        public ActionResult _SalesOrdersCompletedGridActionCore(GridViewModel gridViewModel, int? type)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderCompletedGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.SalesOrderCompletedGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }
        public ActionResult _SalesOrdersUpdatingGridActionCore(GridViewModel gridViewModel, int? type)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderUpdatingGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                new GridViewCustomBindingGetDataHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderUpdatingGetData(args, CurrentTenantId, CurrentWarehouseId);
                })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }

        public ActionResult _SalesOrdersPickerAssignedGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderPickerAssignedGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.SalesOrderPickerAssignedGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }

        public ActionResult _SalesOrdersPickerUnassignedGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderPickerUnassignedGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.SalesOrderPickerUnassignedGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }

        public ActionResult _SalesOrdersAwaitingGridActionCore(GridViewModel gridViewModel, int? type)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderAwaitingGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.SalesOrderAwaitingGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }

        public ActionResult _SalesOrdersActiveGridActionCore(GridViewModel gridViewModel, int? type)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.SalesOrderActiveGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.SalesOrderActiveGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_SalesOrders", gridViewModel);
        }

        public async Task<ActionResult> AuthoriseOrder(AuthoriseSalesOrderModel model)
        {
            var authorised = OrderService.AuthoriseSalesOrder(model.OrderID, CurrentUserId, model.AuthorisedNotes, model.UnAuthorise);

            if (authorised)
            {
                var order = OrderService.GetOrderById(model.OrderID);
                var url = Request.Url.Scheme + "://" + Request.Url.Authority + "/orders/edit/" + order.OrderID;
                var body = "The following order has been authorised successfully.<br/><b><a href='" + url + "'>Authorised Order #" + order.OrderNumber + "</a></b>";
                var subject = "#" + order.OrderNumber + " - Order Authorised";
                string result = await GaneConfigurationsHelper.SendStandardMailNotification(order.TenentId, subject, body, null, null, true);

                if (result != "Success")
                {
                    ViewBag.Warning = result;
                }

                return Json(order.InventoryTransactionTypeId, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AwaitingAuthorisation()
        {
            if (TempData["ErrorAwaitingAuthorization"] != null)
            {
                ViewBag.Error = TempData["ErrorAwaitingAuthorization"].ToString();
            }

            if (TempData["SuccessDS"] != null)
            {
                ViewBag.Success = TempData["SuccessDS"].ToString();
            }
            return View();
        }

        public ActionResult _AwaitingAuthorisationOrders()
        {
            OrderStatusEnum? OrderStatusid = OrderStatusEnum.AwaitingAuthorisation;
            if (Request.Params.AllKeys.Contains("selectedStatus"))
            {
                if (!string.IsNullOrEmpty(Request.Params["selectedStatus"].ToString()))
                {
                    OrderStatusid = (OrderStatusEnum)int.Parse(Request.Params["selectedStatus"].ToString());
                }
            }

            var result = OrderService.GetAllOrdersAwaitingAuthorisation(CurrentTenantId, CurrentWarehouseId, OrderStatusid);

            return PartialView("_AwaitingAuthorisationOrders", result);
        }

        public ActionResult _DirectSalesOrders(int? type)
        {
            ViewBag.Type = type;

            var viewModel = GridViewExtension.GetViewModel("_DirectSalesOrderListGridView" + type);

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreateSalesOrdersGridViewModel();

            return _DirectSalesOrdersGridActionCore(viewModel);
        }

        public ActionResult _DirectSalesOrdersPaging(GridViewPagerState pager, int? type)
        {
            ViewBag.Type = type;
            var viewModel = GridViewExtension.GetViewModel("_DirectSalesOrderListGridView" + type);
            viewModel.Pager.Assign(pager);
            return _DirectSalesOrdersGridActionCore(viewModel);
        }

        public ActionResult _DirectSalesOrdersFiltering(GridViewFilteringState filteringState, int? type)
        {
            //saad
            ViewBag.Type = type;
            var viewModel = GridViewExtension.GetViewModel("_DirectSalesOrderListGridView" + type);
            viewModel.ApplyFilteringState(filteringState);
            return _DirectSalesOrdersGridActionCore(viewModel);
        }

        public ActionResult _DirectSalesOrdersSorting(GridViewColumnState column, bool reset, int? type)
        {
            ViewBag.Type = type;

            var viewModel = GridViewExtension.GetViewModel("_DirectSalesOrderListGridView" + type);
            viewModel.ApplySortingState(column, reset);
            return _DirectSalesOrdersGridActionCore(viewModel);
        }

        public ActionResult _DirectSalesOrdersGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.DirectSalesOrderGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.DirectSalesOrderGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_DirectSalesOrders", gridViewModel);
        }

        public ActionResult _PODetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setName = "gvPODetail" + Id.ToString();
            ViewBag.route = new { Controller = "Order", Action = "_PODetails", Id = ViewBag.orderid };
            return PartialView("_PODetails", OrderService.GetPurchaseOrderDetailsById(Id, CurrentTenantId));
        }

        public ActionResult _SalesOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setname = "gvSODetails" + Id.ToString();
            ViewBag.route = new { Controller = "Order", Action = "_SalesOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            return PartialView("_SalesOrderDetails", OrderService.GetSalesOrderDetails(Id, CurrentTenantId));
        }
        
        public ActionResult _DirectSalesOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setname = "gvDODetails" + Id.ToString();
            ViewBag.route = new { Controller = "Order", Action = "_DirectSalesOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            return PartialView("_DirectSalesOrderDetails", OrderService.GetDirectSalesOrderDetails(Id, CurrentTenantId));
        }

        public ActionResult _DeliveryProof(int id)
        {
            var orderProof = OrderService.GetOrderProofsByOrderProcessId(id, CurrentTenantId);
            return PartialView("_deliveryProof", orderProof);
        }

        public ActionResult _TransferOrderDetails(int Id)
        {
            //setname and routevalues are required to reuse order detail list.
            ViewBag.orderid = Id;
            ViewBag.setname = "gvTransferOrderDetails" + Id.ToString();
            ViewBag.route = new { Controller = "Order", Action = "_TransferOrderDetails", Id = ViewBag.orderid };
            ViewBag.orderid = Id;
            return PartialView("_TransferOrderDetails", OrderService.GetTransferOrderDetails(Id, CurrentWarehouseId));
        }

        public ActionResult _TransferInOrders()
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferInOrderListGridView");

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreateTransferInGridViewModel();

            return _TransferInGridActionCore(viewModel);
        }

        public ActionResult _TransferInPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferInOrderListGridView");
            viewModel.Pager.Assign(pager);
            return _TransferInGridActionCore(viewModel);
        }

        public ActionResult _TransferInFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferInOrderListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _TransferInGridActionCore(viewModel);
        }

        public ActionResult _TransferInSorting(GridViewColumnState column, bool reset, int? type)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferInOrderListGridView");
            viewModel.ApplySortingState(column, reset);
            return _TransferInGridActionCore(viewModel);
        }

        public ActionResult _TransferInGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.TransferInGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.TransferInGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_TransferInOrders", gridViewModel);
        }

        public ActionResult _TransferOutOrders()
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferOutOrderListGridView");

            if (viewModel == null)
                viewModel = OrdersCustomBinding.CreateTransferInGridViewModel();

            return _TransferOutGridActionCore(viewModel);
        }

        public ActionResult _TransferOutPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferOutOrderListGridView");
            viewModel.Pager.Assign(pager);
            return _TransferOutGridActionCore(viewModel);
        }

        public ActionResult _TransferOutFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferOutOrderListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return _TransferOutGridActionCore(viewModel);
        }

        public ActionResult _TransferOutSorting(GridViewColumnState column, bool reset, int? type)
        {
            var viewModel = GridViewExtension.GetViewModel("_TransferOutOrderListGridView");
            viewModel.ApplySortingState(column, reset);
            return _TransferOutGridActionCore(viewModel);
        }

        public ActionResult _TransferOutGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    OrdersCustomBinding.TransferOutGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        OrdersCustomBinding.TransferOutGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_TransferOutOrders", gridViewModel);
        }

        public ActionResult ProcessPO(int? id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPO(Order model)
        {
            return RedirectToAction("Index");
        }

        public PartialViewResult _AddSerial()
        {
            int receivedQty = int.Parse(Request.Params["rec_qty"]);
            int qty = int.Parse(Request.Params["qty"]);
            ViewBag.ProcessedQuantity = receivedQty;
            int procQty = qty == receivedQty ? 0 : (qty > receivedQty ? (qty - receivedQty) : (receivedQty + qty));
            ViewBag.Processed = procQty;
            ViewBag.RequiredQuantity = qty;
            if (Request.UrlReferrer.AbsolutePath.Contains("PurchaseOrders"))
                ViewBag.Locations = new SelectList(_productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId), "LocationId", "LocationCode");
            ViewBag.QuantityEnabled = false;
            return PartialView();
        }

        public JsonResult _VerifySerial(string serial, int type, int? pid, int? orderid)
        {
            bool status = false;
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;
            VerifySerilaStockStatusEnum stockStatus = VerifySerilaStockStatusEnum.NotExist;

            var lastTransaction = OrderService.GetLastInventoryTransactionsForSerial(serial, CurrentTenantId);
            if (lastTransaction != null)
            {
                if (lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferIn ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.AdjustmentIn ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns ||
                   lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Allocated
                   && lastTransaction.WarehouseId == warehouseId)
                {
                    stockStatus = VerifySerilaStockStatusEnum.InStock;
                }
                else
                {
                    stockStatus = VerifySerilaStockStatusEnum.OutofStock;
                }
            }

            switch (type)
            {
                case (int)InventoryTransactionTypeEnum.PurchaseOrder:
                    status = (stockStatus == VerifySerilaStockStatusEnum.NotExist) ? status = true : false;
                    break;

                case (int)InventoryTransactionTypeEnum.SalesOrder:
                case (int)InventoryTransactionTypeEnum.WorksOrder:
                case (int)InventoryTransactionTypeEnum.Samples:
                    status = (stockStatus == VerifySerilaStockStatusEnum.InStock) ? status = true : false;
                    break;

                case (int)InventoryTransactionTypeEnum.Returns:
                case (int)InventoryTransactionTypeEnum.WastedReturn:
                case (int)InventoryTransactionTypeEnum.Wastage:
                case (int)InventoryTransactionTypeEnum.AdjustmentOut:
                    if (stockStatus == VerifySerilaStockStatusEnum.InStock)
                    {
                        status = false;
                        return Json(new { errorcode = 1 }, JsonRequestBehavior.AllowGet);
                    }
                    else if (stockStatus == VerifySerilaStockStatusEnum.InStock && lastTransaction?.OrderID != orderid)
                    {
                        status = false;
                        return Json(new { errorcode = 2 }, JsonRequestBehavior.AllowGet);
                    }
                    else if (stockStatus == VerifySerilaStockStatusEnum.NotExist) { status = false; }
                    else { status = true; }
                    break;

                case (int)InventoryTransactionTypeEnum.TransferIn:
                    status = (stockStatus == VerifySerilaStockStatusEnum.OutofStock && lastTransaction.InventoryTransactionTypeId == InventoryTransactionTypeEnum.TransferOut) ? status = true : false;
                    break;

                case (int)InventoryTransactionTypeEnum.TransferOut:
                    status = (stockStatus == VerifySerilaStockStatusEnum.InStock) ? status = true : false;
                    break;

                default:
                    break;
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _SubmitSerials(List<string> serialList, int product, string delivery, int? cons_Type, int? order, int? location, InventoryTransactionTypeEnum type, int? lineid, FormCollection form, AccountShipmentInfo shipmentInfo, string groupToken = null)
        {
            GoodsReturnRequestSync goodsReturnRequestSync = new GoodsReturnRequestSync();
            try
            {
                string orderNumber = "";
                if (order.HasValue)
                {
                    goodsReturnRequestSync.ProductSerials = serialList;
                    goodsReturnRequestSync.Quantity = serialList.Count;
                    goodsReturnRequestSync.OrderId = order ?? 0;
                    goodsReturnRequestSync.ProductId = product;
                    goodsReturnRequestSync.InventoryTransactionType = type;
                    goodsReturnRequestSync.LocationId = location ?? 0;
                    goodsReturnRequestSync.OrderDetailID = lineid;
                    goodsReturnRequestSync.tenantId = CurrentTenantId;
                    goodsReturnRequestSync.warehouseId = CurrentWarehouseId;
                    goodsReturnRequestSync.userId = CurrentUserId;
                    goodsReturnRequestSync.deliveryNumber = delivery;
                    Inventory.StockTransaction(goodsReturnRequestSync, cons_Type, groupToken, shipmentInfo);
                }
                else
                {
                    orderNumber = GenerateNextOrderNumber((InventoryTransactionTypeEnum)type);
                    goodsReturnRequestSync.ProductSerials = serialList;
                    goodsReturnRequestSync.Quantity = serialList.Count;
                    goodsReturnRequestSync.OrderId = order ?? 0;
                    goodsReturnRequestSync.ProductId = product;
                    goodsReturnRequestSync.InventoryTransactionType = type;
                    goodsReturnRequestSync.LocationId = location ?? 0;
                    goodsReturnRequestSync.OrderDetailID = lineid;
                    goodsReturnRequestSync.deliveryNumber = delivery;
                    goodsReturnRequestSync.OrderNumber = orderNumber;
                    goodsReturnRequestSync.tenantId = CurrentTenantId;
                    goodsReturnRequestSync.warehouseId = CurrentWarehouseId;
                    goodsReturnRequestSync.userId = CurrentUserId;

                    Inventory.StockTransaction(goodsReturnRequestSync, cons_Type, groupToken, shipmentInfo);
                }
                if (type == InventoryTransactionTypeEnum.Returns || type == InventoryTransactionTypeEnum.Wastage || type == InventoryTransactionTypeEnum.WastedReturn)
                {
                    return Json(new { orderid = order ?? 0, productId = product, orderNumber = orderNumber, groupToken = groupToken }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = false });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, message = exp.Message });
            }
        }

        public PartialViewResult _RecProduct()
        {
            var whouse = caCurrent.CurrentWarehouse();
            int pid = int.Parse(Request.Params["pid"]);
            int orderid = int.Parse(Request.Params["order"]);
            Session["delivery"] = Request.Params["delivery"] ?? null;
            Session["consignmenttype"] = Request.Params["consignmenttype"] ?? null;
            Session["type"] = Request.Params["type"] ?? null;
            int type = Convert.ToInt32(Request.Params["type"]);
            Session["line_id"] = Request.Params["line_id"] ?? null;
            var orderDetailId = int.Parse(Request.Params["line_id"] ?? "0");
            ViewBag.QuantityEnabled = true;
            var quantity = 0.0m;

            var productOrderDetail = OrderService.GetOrderDetailsForProduct(orderid, pid, CurrentTenantId);
            if (productOrderDetail != null && productOrderDetail.Any())
            {
                var orderDetail = productOrderDetail.FirstOrDefault(m => m.OrderDetailID == orderDetailId);
                if (orderDetail != null)
                {
                    quantity = orderDetail.Qty;
                    var processed = OrderService.GetAllOrderProcessesByOrderDetailId(orderDetailId, CurrentWarehouseId).Sum(m => m.QtyProcessed);
                    quantity = quantity - processed;
                }
            }
            if (quantity < 0)
            {
                quantity = 0;
            }

            var product = _productServices.GetProductMasterById(pid);
            if (product?.ProcessByCase != null && product?.ProcessByCase == true)
            {
                ViewBag.cases = product?.ProcessByCase;
                ViewBag.processcase = product?.ProductsPerCase ?? 1;

                ViewBag.caseProcess = string.Format("{0:0.00}", (quantity / (product?.ProductsPerCase ?? 1)));
            }

            ViewBag.QuantityEnabled = true;

            if (product != null && product.Serialisable)
            {
                ViewBag.QuantityEnabled = false;
            }

            ViewBag.Title = Request.UrlReferrer.AbsolutePath.Contains("PurchaseOrders") ? "Receive Product" : "Process Product";

            var data = _productLookupService.GetAllProductLocationsByProductId(pid, whouse.WarehouseId).
                Select(plocation => new
                {
                    ProductName = plocation.ProductMaster.Name,
                    plocation.LocationId,
                    plocation.Locations.LocationType?.LocTypeName,
                    plocation.Locations.LocationGroup?.Locdescription,
                    plocation.Locations.LocationCode,
                    plocation.Locations.TenantWarehouses.WarehouseName
                }).ToList();

            if (data.Count() > 0)
            {
                ViewBag.Locations = new SelectList(data, "LocationId", "LocationCode");
            }
            else
            {
                ViewBag.Locations = new SelectList(_productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId), "LocationId", "LocationCode");
            }

            if (pid > 0)
            {
                if (product != null)
                {
                    ViewBag.ProductName = product.NameWithCode;
                    ViewBag.RequiresBatchNumber = product.RequiresBatchNumberOnReceipt;
                    ViewBag.RequiresExpiryDate = product.RequiresExpiryDateOnReceipt;
                }
            }
            ViewBag.quantity = quantity;
            return PartialView(new InventoryTransaction { ProductId = pid, InventoryTransactionTypeId = (InventoryTransactionTypeEnum)type, OrderID = orderid, Quantity = quantity });
        }

        public JsonResult GetProductLocations(int id)
        {
            var productLocations = _commonDbServices.LocationsByProductDetails(id, CurrentWarehouseId);
            return Json(productLocations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult _SubmitRecProduct(InventoryTransaction model, FormCollection form, AccountShipmentInfo shipmentInfo)
        {
            try
            {
                model.TenentId = CurrentTenantId;
                model.WarehouseId = CurrentWarehouseId;
                model.CreatedBy = CurrentUserId;

                int type = (int)model.InventoryTransactionTypeId;

                int? cons_type = null;
                if (Request.UrlReferrer.AbsolutePath.Contains("PurchaseOrders")) { }
                else if (Request.UrlReferrer.AbsolutePath.Contains("SalesOrders"))
                { cons_type = GaneStaticAppExtensions.ParseToNullableInt(Session["consignmenttype"]?.ToString()); }
                else if (Request.UrlReferrer.AbsolutePath.Contains("TransferOrders"))
                {
                    type = int.Parse(Session["type"].ToString());
                }

                var delivery = Session["delivery"]?.ToString();
                var line_id = int.Parse(Session["line_id"] != null ? Session["line_id"].ToString() : "0");

                if (!string.IsNullOrEmpty(model.BatchNumber) && !_productServices.IsValidBatchForProduct(model.ProductId, model.BatchNumber))
                {
                    return Json(new { error = true, errorMessage = "Batch number belongs to different product." }, JsonRequestBehavior.AllowGet);
                }

                //TODO: IN PROGRESS MULTI PRODUCT LOCATIONS
                var productPickLocations = form.AllKeys.Where(m => m.Contains("PickQuantity")).ToList();

                var pickLocations = new List<CommonLocationViewModel>();
                foreach (var item in productPickLocations)
                {
                    if (string.IsNullOrEmpty(form[item])) continue;
                    var cItem = new CommonLocationViewModel();
                    var ids = item.Split('_');
                    cItem.LocationCode = ids[1].Replace(",", "");
                    if (ids.Length > 3)
                    {
                        cItem.BatchNumber = ids[2].Replace(",", "");
                    }
                    cItem.Quantity = decimal.Parse((form[item] ?? "0").Replace(",", ""));
                    pickLocations.Add(cItem);
                }

                Inventory.StockTransaction(model, (InventoryTransactionTypeEnum)type, cons_type, delivery, line_id, pickLocations, shipmentInfo);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                throw exp;
            }
        }

        public JsonResult _SubmitProcessedItems(CommonProductProcessingViewModel processModel, AccountShipmentInfo shipmentInfo)
        {
            try
            {
                int type = processModel.InventoryTransactionTypeId;
                if (type < 1)
                {
                    if (Request.UrlReferrer.AbsolutePath.Contains("PurchaseOrders"))
                    {
                        type = (int)InventoryTransactionTypeEnum.PurchaseOrder;
                    }
                    else if (Request.UrlReferrer.AbsolutePath.Contains("SalesOrders"))
                    {
                        type = (int)InventoryTransactionTypeEnum.SalesOrder;
                    }
                    else
                    {
                        throw new Exception("Transaction Type cannot be found");
                    }
                }

                var product = _productServices.GetProductMasterById(processModel.ProductID);
                var order = OrderService.GetOrderById(processModel.OrderID);

                var qtyToBeProcessed = processModel.Quantity * (processModel.IsCaseQuantity && product.ProductsPerCase.HasValue ? product.ProductsPerCase.Value : 1);

                var totalProcessedQuantity = order.OrderProcess.SelectMany(s => s.OrderProcessDetail).Where(x => x.IsDeleted != true).Sum(m => m.QtyProcessed);

                var requiredOrderQuantity = order.OrderDetails.Where(x => x.IsDeleted != true).Sum(m => m.Qty);

                requiredOrderQuantity -= totalProcessedQuantity;

                if ((type == (int)InventoryTransactionTypeEnum.SalesOrder || type == (int)InventoryTransactionTypeEnum.WorksOrder || type == (int)InventoryTransactionTypeEnum.TransferOut) && qtyToBeProcessed > requiredOrderQuantity)
                {
                    return Json(new { error = true, Message = $"You cannot process more than the required quantity. Max Allowed = {requiredOrderQuantity}, Requested = {qtyToBeProcessed}" }, JsonRequestBehavior.AllowGet);
                }

                var model = new InventoryTransaction
                {
                    ProductId = processModel.ProductID,
                    Quantity = qtyToBeProcessed,
                    OrderID = processModel.OrderID,
                    WarehouseId = CurrentWarehouseId,
                    InventoryTransactionTypeId = (InventoryTransactionTypeEnum)type,
                    TenentId = CurrentTenantId,
                    CreatedBy = CurrentUserId,
                    DateCreated = DateTime.UtcNow,
                };

                Inventory.StockTransaction(model, (InventoryTransactionTypeEnum)type, 0, processModel.DeliveryNo, processModel.OrderDetailID, null, shipmentInfo);

                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exp)
            {
                ErrorSignal.FromCurrentContext().Raise(exp);
                throw exp;
            }
        }

        public ActionResult _PriceHistory()
        {
            var product = int.Parse(Request.Params["product"]);
            var account = 0;
            if (!string.IsNullOrEmpty(Request.Params["account"]))
            {
                account = int.Parse(Request.Params["account"]);
            }
            var ordertype = int.Parse(Request.Params["ordertype"]);

            var model = _productPriceService.GetProductPriceHistoryForAccount(product, account).Where(m => m.TypeIdentifier == ordertype).ToList();

            return PartialView("_PriceHistory", model);
        }

        public ActionResult _PriceHistoryList()
        {
            var product = int.Parse(Request.Params["product"]);
            var account = int.Parse(Request.Params["account"]);
            var ordertype = int.Parse(Request.Params["ordertype"]);
            var model = _productPriceService.GetProductPriceHistoryForAccount(product, account);

            return PartialView("_PriceHistoryList", model);
        }

        public ActionResult _PendingProcessList()
        {
            var viewModel = GridViewExtension.GetViewModel("_PendingListGridView");

            if (viewModel == null)
                viewModel = PickListCustomBinding.CreatePickListGridViewModel();

            if (string.IsNullOrEmpty(viewModel.FilterExpression) && ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("_PendingListGridView"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["_PendingListGridView"];
                var decodedValue = HttpUtility.UrlDecode(cookie.Value);
                var filterParams = decodedValue
                    .Split('|')
                    .ToList();
                var lengthParam = filterParams.Where(x => x.StartsWith("filter")).SingleOrDefault();

                if (!string.IsNullOrEmpty(lengthParam))
                {
                    var index = filterParams.IndexOf(lengthParam);
                    var savedFilterExpression = filterParams[index + 1];
                    viewModel.FilterExpression = savedFilterExpression;
                }
            }

            return _PickListGridActionCore(viewModel);
        }

        public ActionResult _PickListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("_PendingListGridView");
            viewModel.Pager.Assign(pager);
            return _PickListGridActionCore(viewModel);
        }

        public ActionResult _PickListFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("_PendingListGridView");

            viewModel.ApplyFilteringState(filteringState);
            return _PickListGridActionCore(viewModel);
        }

        public ActionResult _PickListSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("_PendingListGridView");
            viewModel.ApplySortingState(column, reset);
            return _PickListGridActionCore(viewModel);
        }

        public ActionResult _PickListGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    PickListCustomBinding.PickListGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId, CurrentUserId);
                }),
                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        PickListCustomBinding.PickListGetData(args, CurrentTenantId, CurrentWarehouseId, CurrentUserId);
                    })
            );

            return PartialView("_PendingProcessList", gridViewModel);
        }

        private bool getQuantity(int orderId)
        {
            var lines = OrderService.GetAllValidOrderDetailsByOrderId(orderId);
            foreach (var item in lines)
            {
                var processedQty = (from op in OrderService.GetAllOrderProcessesByOrderDetailId(item.OrderDetailID, CurrentWarehouseId)
                                    select (int?)op.QtyProcessed).Sum() ?? 0;

                var available = _productServices.GetAllInventoryStocksByProductId(item.ProductId).First().Available;
                if (available == 0) return false;
                if ((item.Qty - processedQty)
                     <= available)
                    continue;
                else
                {
                    return false;
                }
            }
            return true;
        }

        public JsonResult _GetAccountContacts(int Id)
        {
            var model = AccountServices.GetAllValidAccountContactsByAccountId(Id, CurrentTenantId)
                         .Select(m => new
                         {
                             m.AccountContactId,
                             m.ContactName
                         }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _OrderNotes(string PageSessionToken)
        {
            ViewBag.OrderID = Request.Params["OrderID"];
            return PartialView();
        }

        public ActionResult _OrderNotesList(string id, string PageSessionToken)
        {
            var nList = GaneOrderNotesSessionHelper.GetOrderNotesSession(PageSessionToken);
            ViewBag.NotesGridName = id;
            return PartialView(nList);
        }

        public JsonResult _saveNotes(OrderNotes model, string PageSessionToken)
        {
            if (model.OrderID > 0)
            {
                model = OrderService.UpdateOrderNote(model.OrderNoteId, model.Notes, CurrentUserId, model.OrderID);
            }
            var note = GaneOrderNotesSessionHelper.UpdateOrderNotesSession(PageSessionToken, model);
            return Json(note, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _deleteNotes(int id, string PageSessionToken)
        {
            if (id > 0)
            {
                OrderService.DeleteOrderNoteById(id, CurrentUserId);
            }
            GaneOrderNotesSessionHelper.RemoveOrderNotesSession(PageSessionToken, id);

            return Json(true);
        }

        public ActionResult DeleteOrder(int id)
        {
            if (!caSession.AuthoriseSession()) { return Json(false, JsonRequestBehavior.AllowGet); }

            OrderService.DeleteOrderById(id, CurrentUserId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateOrderStatus(int orderId, OrderStatusEnum statusId)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var order = OrderService.UpdateOrderStatus(orderId, statusId, CurrentUserId);

            if (order != null && order.OrderStatusID == statusId)
            {
                var notificationSendResult = await NotifyOrderStatusChange(order);

                if (string.IsNullOrEmpty(notificationSendResult))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                return Json(notificationSendResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<string> NotifyOrderStatusChange(Order order)
        {
            if (order.InventoryTransactionTypeId != InventoryTransactionTypeEnum.SalesOrder ||
                order.InventoryTransactionTypeId != InventoryTransactionTypeEnum.DirectSales ||
                order.Warehouse?.SelectedNotifiableOrderStatuses == null ||
                !order.Warehouse.SelectedNotifiableOrderStatuses.Contains(order.OrderStatusID))
            {
                return null;
            }

            var results = string.Empty;

            if (order.SendOrderStatusByEmail)
            {
                var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{order.OrderNumber} - Order status updated to {order.OrderStatusID}", _mapper.Map(order, new OrderViewModel()), worksOrderNotificationType: WorksOrderNotificationTypeEnum.OrderStatusNotification);

                if (result != "Success")
                {
                    results = result;
                }
            }

            if (order.SendOrderStatusBySms)
            {
                var result = await _userService.SendSmsBroadcast(order.Account.AccountOwner.UserMobileNumber, order.Tenant.TenantName, order.Account.OwnerUserId.ToString(), $"#{order.OrderNumber} - Order status updated to {order.OrderStatusID}");

                if (result != true)
                {
                    results += "Failed to send SMS Notification!";
                }
            }

            return results;
        }

        public ActionResult _updateNote(int id, string note, string PageSessionToken)
        {
            if (id < 0)
            {
                var newNote = new OrderNotes() { Notes = note, OrderNoteId = id };
                GaneOrderNotesSessionHelper.UpdateOrderNotesSession(PageSessionToken, newNote);
            }
            else
            {
                OrderService.UpdateOrderNote(id, note, CurrentUserId);
            }
            return Json(true);
        }

        public JsonResult GetAccountAddresses(int id)
        {
            var account = AccountServices.GetAccountsById(id);

            var list = AccountServices.GetAllValidAccountAddressesByAccountId(account.AccountID).Select(m => new SelectListItem() { Text = m.FullAddressValue, Value = m.AddressID.ToString() }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowEmail()
        {
            int orderId = int.Parse(!string.IsNullOrEmpty(Request.Params["orderId"]) ? Request.Params["orderId"] : "0");
            int invoicemasterId = int.Parse(!string.IsNullOrEmpty(Request.Params["invoicemasterId"]) ? Request.Params["invoicemasterId"] : "0");
            int TemplateId = int.Parse(!string.IsNullOrEmpty(Request.Params["TemplateId"]) ? Request.Params["TemplateId"] : "0");
            ViewBag.orderid = orderId;
            ViewBag.invoicemasterId = invoicemasterId;
            ViewBag.templateId = TemplateId;
            return View();
        }

        public PartialViewResult _ShowEmailPartial(int Id, int InvoiceMasterId, int? TemplateId)
        {
            ViewBag.orderId = Id;
            ViewBag.invoicemasterId = InvoiceMasterId;
            ViewBag.templateId = TemplateId;
            var emailTenantqueue = _emailServices.GetAllTenantEmailNotificationQueuesbyOrderId(Id, InvoiceMasterId, CurrentTenantId, TemplateId);
            return PartialView(emailTenantqueue);
        }

        public bool SyncDate(int OrderId)
        {
            return OrderService.UpdateDateInOrder(OrderId);
        }

        public JsonResult _GetTenantDeliveryServices(DeliveryMethods deliveryMethod)
        {
            var model = _palletingService.GetAllTenantDeliveryServices(CurrentTenantId, deliveryMethod)
                         .Select(m => new
                         {
                             m.Id,
                             m.NetworkDescription,
                             m.NetworkCode
                         }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}