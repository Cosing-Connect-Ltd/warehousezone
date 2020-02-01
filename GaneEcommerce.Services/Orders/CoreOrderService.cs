using System;
using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Entities.Enums;
using WMS.VModels;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class CoreOrderService : ICoreOrderService
    {
        private ITransferOrderService TransferOrderService { get; }
        public IOrderService OrderService { get; }
        public IPurchaseOrderService PurchaseOrderService { get; }
        public IWorksOrderService WorksOrderService { get; }
        public ISalesOrderService SalesOrderService { get; }

        public CoreOrderService(IOrderService orderService, IPurchaseOrderService purchaseOrderService, IWorksOrderService worksOrderService, ITransferOrderService transferOrderService, ISalesOrderService salesOrderService)
        {
            TransferOrderService = transferOrderService;
            OrderService = orderService;
            PurchaseOrderService = purchaseOrderService;
            WorksOrderService = worksOrderService;
            SalesOrderService = salesOrderService;
        }
        public string GenerateNextOrderNumber(InventoryTransactionTypeEnum type, int tenantId)
        {
            return OrderService.GenerateNextOrderNumber(type, tenantId);
        }
        public Order CreateOrderByOrderNumber(string orderNumber, int productId, int tenantId, int warehouseId, int transType, int userId,decimal quantity)
        {
            return OrderService.CreateOrderByOrderNumber(orderNumber, productId, tenantId, warehouseId, transType, userId,quantity);

        }
        public IQueryable<Order> GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId, int? warehouseId = null)
        {
            return OrderService.GetValidSalesOrderByOrderNumber(orderNumber, tenantId, warehouseId);
        }
        public string GetAuthorisedUserNameById(int userId)
        {
            return OrderService.GetAuthorisedUserNameById(userId);
        }

        public IEnumerable<AuthUser> GetAllAuthorisedUsers(int tenantId, bool includeSuperUser = false)
        {
            return OrderService.GetAllAuthorisedUsers(tenantId, includeSuperUser);
        }

        public IEnumerable<JobType> GetAllValidJobTypes(int tenantId)
        {
            return OrderService.GetAllValidJobTypes(tenantId);
        }

        public IEnumerable<JobSubType> GetAllValidJobSubTypes(int tenantId)
        {
            return OrderService.GetAllValidJobSubTypes(tenantId);
        }

        public Order GetOrderById(int orderId)
        {
            return OrderService.GetOrderById(orderId);
        }

        public bool IsOrderNumberAvailable(string orderNumber)
        {
            return OrderService.IsOrderNumberAvailable(orderNumber);
        }

        public Order CompleteOrder(int orderId, int userId)
        {
            return OrderService.CompleteOrder(orderId, userId);
        }
        public Order FinishinghOrder(int orderId, int userId, int wareHouseId)
        {
            return OrderService.FinishinghOrder(orderId, userId, wareHouseId);
        }
        public List<Order> GetDirectSaleOrders(int? orderId)
        {
            return SalesOrderService.GetDirectSaleOrders(orderId);
        }

        public Order CreateOrder(Order order, int tenantId, int warehouseId, int userId, IEnumerable<OrderDetail> orderDetails = null,
            IEnumerable<OrderNotes> orderNotes = null)
        {
            return OrderService.CreateOrder(order, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public Order SaveOrder(Order order, int tenantId, int warehouseId, int userId, IEnumerable<OrderDetail> orderDetails = null,
            IEnumerable<OrderNotes> orderNotes = null)
        {
            return OrderService.SaveOrder(order, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public IEnumerable<OrderDetail> GetAllValidOrderDetailsByOrderId(int orderId)
        {
            return OrderService.GetAllValidOrderDetailsByOrderId(orderId);
        }
        public IQueryable<ProductMaster> GetAllValidProduct(int currentTenantId)
        {
            return OrderService.GetAllValidProduct(currentTenantId);
        }

        public IEnumerable<OrderStatus> GetOrderStatusThatCanbeManaged()
        {
            return OrderService.GetOrderStatusThatCanbeManaged();
        }

        public List<OrderDetailsViewModel> GetDirectSalesOrderDetails(int id, int tenantId)
        {
            return OrderService.GetDirectSalesOrderDetails(id, tenantId);
        }
        public List<OrderProofOfDelivery> GetOrderProofsByOrderProcessId(int Orderid, int TenantId)
        {
            return OrderService.GetOrderProofsByOrderProcessId(Orderid, TenantId);
        }

        public List<OrderDetailsViewModel> GetPalletOrdersDetails(int id, int tenantId, bool excludeProcessed = false)
        {
            return OrderService.GetPalletOrdersDetails(id, tenantId, excludeProcessed);
        }

        public List<OrderDetailsViewModel> GetSalesOrderDetails(int id, int tenantId)
        {
            return OrderService.GetSalesOrderDetails(id, tenantId);
        }

        public List<OrderDetailsViewModel> GetTransferOrderDetails(int orderId, int tenantId)
        {
            return OrderService.GetTransferOrderDetails(orderId, tenantId);
        }

        public List<OrderDetailsViewModel> GetWorksOrderDetails(int id, int tenantId)
        {
            return OrderService.GetWorksOrderDetails(id, tenantId);
        }

        public IQueryable<TransferOrderViewModel> GetTransferInOrderViewModelDetails(int id, int tenantId, int? type = null)
        {
            return OrderService.GetTransferInOrderViewModelDetails(id, tenantId, type);
        }

        public IQueryable<TransferOrderViewModel> GetTransferOutOrderViewModelDetailsIq(int fromWarehouseId, int tenantId, int? type = null)
        {
            return OrderService.GetTransferOutOrderViewModelDetailsIq(fromWarehouseId, tenantId, type);
        }

        public List<OrderProcessDetail> GetOrderProcessDetailsByProcessId(int processId)
        {
            return OrderService.GetOrderProcessDetailsByProcessId(processId);
        }

        public List<OrderProcess> GetOrderProcesssDeliveriesForWarehouse(int warehouseId, int tenantId)
        {
            return OrderService.GetOrderProcesssDeliveriesForWarehouse(warehouseId, tenantId);
        }

        public List<OrderProcess> GetOrderProcessConsignmentsForWarehouse(int warehouseId, int tenantId)
        {
            return OrderService.GetOrderProcessConsignmentsForWarehouse(warehouseId, tenantId);
        }

        public List<OrderProcessDetail> GetOrderProcessesDetailsForOrderProduct(int orderId, int productId)
        {
            return OrderService.GetOrderProcessesDetailsForOrderProduct(orderId, productId);
        }

        public List<InventoryTransaction> GetAllReturnsForOrderProduct(int orderId, int productId)
        {
            return OrderService.GetAllReturnsForOrderProduct(orderId, productId);
        }

        public OrderProcess GetOrderProcessByDeliveryNumber(int orderId, int transactiontypeId, string deliveryNumber, int userId, DateTime? createdDate, int warehouseId = 0, AccountShipmentInfo shipmentInfo = null)
        {
            return OrderService.GetOrderProcessByDeliveryNumber(orderId, transactiontypeId, deliveryNumber, userId, createdDate, warehouseId, shipmentInfo);
        }

        public IQueryable<Order> GetAllOrders(int tenantId, int warehouseId = 0, bool excludeProforma = false, DateTime? reqDate = null, bool includeDeleted = false)
        {
            return OrderService.GetAllOrders(tenantId, warehouseId, excludeProforma, reqDate, includeDeleted);
        }

        public IEnumerable<OrderIdsWithStatus> GetAllOrderIdsWithStatus(int tenantId, int warehouseId)
        {
            return OrderService.GetAllOrderIdsWithStatus(tenantId, warehouseId);
        }

        public IQueryable<Order> GetAllOrdersIncludingNavProperties(int tenantId, int warehouseId)
        {
            return OrderService.GetAllOrdersIncludingNavProperties(tenantId, warehouseId);
        }

        public Order UpdateOrderStatus(int orderId, int statusId, int userId)
        {
            return OrderService.UpdateOrderStatus(orderId, statusId, userId);
        }

        public bool UpdateOrderProcessStatus(int orderProcessId, int UserId)
        {
            return OrderService.UpdateOrderProcessStatus(orderProcessId, UserId);
        }

        public Order GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId)
        {
            return OrderService.GetValidSalesOrderByOrderNumber(orderNumber, tenantId);
        }

        public List<OrderDetail> GetOrderDetailsForProduct(int orderId, int productId, int TenantId)
        {
            return OrderService.GetOrderDetailsForProduct(orderId, productId, TenantId);
        }

        public OrderDetail SaveOrderDetail(OrderDetail podetail, int tenantId, int userId)
        {
            return OrderService.SaveOrderDetail(podetail, tenantId, userId);
        }

        public void RemoveOrderDetail(int orderDetailId, int tenantId, int userId)
        {
            OrderService.RemoveOrderDetail(orderDetailId, tenantId, userId);
        }


        public OrderDetail GetOrderDetailsById(int orderDetailId)
        {
            return OrderService.GetOrderDetailsById(orderDetailId);
        }

        public List<OrderDetail> GetAllOrderDetailsForOrderAccount(int supplierAccountId, int poOrderId, int tenantId)
        {
            return OrderService.GetAllOrderDetailsForOrderAccount(supplierAccountId, poOrderId, tenantId);
        }

        public List<OrderProcessDetail> GetAllOrderProcessesByOrderDetailId(int orderDetailId, int warehouseId)
        {
            return OrderService.GetAllOrderProcessesByOrderDetailId(orderDetailId, warehouseId);
        }

        public IQueryable<OrderProcess> GetAllOrderProcesses(DateTime? ordersAfter, int? orderId, int? orderProcessStatusId = null, int? transactionTypeId = null, bool includeDeleted = false)
        {
            return OrderService.GetAllOrderProcesses(ordersAfter, orderId, orderProcessStatusId, transactionTypeId, includeDeleted);
        }

        public IQueryable<OrderProcess> GetAllOrderProcesses(DateTime? updatedAfter, int? orderId = 0)
        {
            return OrderService.GetAllOrderProcesses(updatedAfter, orderId);
        }

        public List<OrderProcessDetail> GetAllOrderProcessesDetails(DateTime? updatedAfter, int? orderProcessId)
        {
            return OrderService.GetAllOrderProcessesDetails(updatedAfter, orderProcessId);
        }

        public OrderProcess CreateOrderProcess(int orderId, string deliveryNo, int[] product, decimal[] qty, decimal[] qtyReceived,
            int[] lines, string serialStamp, int currentUserId, int currentTenantId, int warehouseId)
        {
            return OrderService.CreateOrderProcess(orderId, deliveryNo, product, qty, qtyReceived, lines, serialStamp,
                currentUserId, currentTenantId, warehouseId);
        }

        public OrderProcess SaveOrderProcess(int orderProcessId, int[] product, decimal[] qty, decimal[] qtyReceived, int[] lines,
            int currentUserId, int currentTenantId, int warehouseId)
        {
            return OrderService.SaveOrderProcess(orderProcessId, product, qty, qtyReceived, lines, currentUserId,
                currentTenantId, warehouseId);
        }

        public OrdersSync SaveOrderProcessSync(OrderProcessesSync orderProcess, Terminals terminal)
        {
            return OrderService.SaveOrderProcessSync(orderProcess, terminal);
        }

        public OrderProcess GetOrderProcessByOrderProcessId(int orderProcessid)
        {
            return OrderService.GetOrderProcessByOrderProcessId(orderProcessid);
        }

        public OrderProcessDetail GetOrderProcessDetailById(int orderProcessDetailId)
        {
            return OrderService.GetOrderProcessDetailById(orderProcessDetailId);
        }

        public List<OrderProcessViewModel> GetOrderProcessByWarehouseId(int warehouseId)
        {
            return OrderService.GetOrderProcessByWarehouseId(warehouseId);
        }

        public List<OrderStatus> GetAllOrderStatus()
        {
            return OrderService.GetAllOrderStatus();
        }

        public OrderStatus GetOrderStatusByName(string orderStatusName)
        {
            return OrderService.GetOrderStatusByName(orderStatusName);
        }

        public List<ProductSerialis> GetProductSerialsByNumber(string serialNo, int tenantId)
        {
            return OrderService.GetProductSerialsByNumber(serialNo, tenantId);
        }

        public InventoryTransaction GetLastInventoryTransactionsForSerial(string serial, int tenantId)
        {
            return OrderService.GetLastInventoryTransactionsForSerial(serial, tenantId);
        }

        public IQueryable<Order> GetAllPendingOrdersForProcessingForDate()
        {
            return OrderService.GetAllPendingOrdersForProcessingForDate();
        }

        public OrderNotes DeleteOrderNoteById(int orderNoteId, int userId)
        {
            return OrderService.DeleteOrderNoteById(orderNoteId, userId);
        }

        public OrderNotes UpdateOrderNote(int noteId, string notes, int userId, int? orderId = null)
        {
            return OrderService.UpdateOrderNote(noteId, notes, userId, orderId);
        }

        public List<OrderProcessDetail> GetOrderProcessDetailForOrders(int orderprocessId, List<int> orderIds = null)
        {
            return OrderService.GetOrderProcessDetailForOrders(orderprocessId, orderIds);
        }

        public PalletOrderProductsCollection GetAllProductsByOrdersInPallet(int palletId)
        {
            return OrderService.GetAllProductsByOrdersInPallet(palletId);
        }

        public Order CreateDirectSalesOrder(DirectSalesViewModel model, int tenantId, int userId, int warehouseId)
        {
            return OrderService.CreateDirectSalesOrder(model, tenantId, userId, warehouseId);
        }

        public DirectSalesViewModel GetDirectSalesModelByOrderId(int orderId)
        {
            return OrderService.GetDirectSalesModelByOrderId(orderId);
        }


        public List<PurchaseOrderViewModel> GetAllPurchaseOrders(int tenantId)
        {
            return PurchaseOrderService.GetAllPurchaseOrders(tenantId);
        }


        public InventoryTransactionType GetInventoryTransactionTypeById(int inventoryTransactionTypeId)
        {
            return OrderService.GetInventoryTransactionTypeById(inventoryTransactionTypeId);
        }

        public OrderStatus GetOrderstatusByName(string statusName)
        {
            return OrderService.GetOrderstatusByName(statusName);
        }

        public OrderStatus GetOrderstatusById(int id)
        {
            return OrderService.GetOrderstatusById(id);
        }
        public IEnumerable<OrderPTenantEmailRecipient> GetAccountContactId(int OrderId)
        {

            return PurchaseOrderService.GetAccountContactId(OrderId);
        }
        public Order CreatePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return PurchaseOrderService.CreatePurchaseOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public Order SavePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return PurchaseOrderService.SavePurchaseOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public IQueryable<OrderProcess> GetAllPurchaseOrderProcesses(int tenantId, int warehouseId)
        {
            return PurchaseOrderService.GetAllPurchaseOrderProcesses(tenantId, warehouseId);
        }

        public IEnumerable<OrderProcessDetail> GetOrderProcessDetailByOrderProcessId(int orderProcessId)
        {
            return PurchaseOrderService.GetOrderProcessDetailByOrderProcessId(orderProcessId);
        }

        public IEnumerable<OrderConsignmentTypes> GetAllValidConsignmentTypes(int tenantId)
        {
            return OrderService.GetAllValidConsignmentTypes(tenantId);
        }

        public Order CreateBlindShipmentOrder(List<BSDto> bsList, int accountId, string deliveryNumber, string poNumber, int tenantId,
            int warehouseId, int userId, int transType, AccountShipmentInfo accountShipmentInfo = null)
        {
            return PurchaseOrderService.CreateBlindShipmentOrder(bsList, accountId, deliveryNumber, poNumber, tenantId, warehouseId, userId, transType, accountShipmentInfo);
        }

        public List<OrderDetailsViewModel> GetPurchaseOrderDetailsById(int orderId, int tenantId)
        {
            return PurchaseOrderService.GetPurchaseOrderDetailsById(orderId, tenantId);
        }

        public InventoryTransaction SubmitReceiveInventoryTransaction(InventoryTransaction model, string deliveryNumber, int tenantId, int warehouseId, int userId)
        {
            return PurchaseOrderService.SubmitReceiveInventoryTransaction(model, deliveryNumber, tenantId, warehouseId, userId);
        }

        public Order UpdatePurchaseOrderStatus(int orderId, int orderStatusId, int userId)
        {
            return PurchaseOrderService.UpdatePurchaseOrderStatus(orderId, orderStatusId, userId);
        }




        public IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersCompleted(int tenantId, int warehouseId, int? type = null)
        {
            return PurchaseOrderService.GetAllPurchaseOrdersCompleted(tenantId, warehouseId, type);
        }
        public IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersInProgress(int tenantId, int warehouseId)
        {
            return PurchaseOrderService.GetAllPurchaseOrdersInProgress(tenantId, warehouseId);
        }

        public IQueryable<Order> GetValidSalesOrder(int tenantId, int warehouseId)
        {
            return OrderService.GetValidSalesOrder(tenantId, warehouseId);
        }

        public Order CancelPurchaseOrder(int orderId, int userId, int warehouseId)
        {
            return PurchaseOrderService.CancelPurchaseOrder(orderId, userId, warehouseId);
        }

        public Order CreateWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            return WorksOrderService.CreateWorksOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public Order SaveWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, bool isOrderComplete, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            return WorksOrderService.SaveWorksOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, isOrderComplete, orderDetails, orderNotes);
        }
        public Order UpdateWorksOrderBulkSingle(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return WorksOrderService.UpdateWorksOrderBulkSingle(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public List<Order> GetAllOrdersByGroupToken(Guid groupToken, int tenantId)
        {
            return WorksOrderService.GetAllOrdersByGroupToken(groupToken, tenantId);
        }

        public List<WorksOrderViewModel> GetAllPendingWorksOrders(int tenantId, Guid? groupToken = null)
        {
            return WorksOrderService.GetAllPendingWorksOrders(tenantId, groupToken);
        }

        public IQueryable<WorksOrderViewModel> GetAllPendingWorksOrdersIq(int tenantId, Guid? groupToken = null, int? propertyId = null)
        {
            return WorksOrderService.GetAllPendingWorksOrdersIq(tenantId, groupToken, propertyId);
        }

        public IQueryable<WorksOrderViewModel> GetAllCompletedWorksOrdersIq(int tenantId, int? propertyId, int? type = null)
        {
            return WorksOrderService.GetAllCompletedWorksOrdersIq(tenantId, propertyId, type);
        }
        public List<WorksOrderViewModel> GetAllPendingWorksOrders(int tenantId)
        {
            return WorksOrderService.GetAllPendingWorksOrders(tenantId);
        }

        public Order SaveWorksOrderBulkSingle(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null)
        {
            return WorksOrderService.SaveWorksOrderBulkSingle(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public Order CreateTransferOrder(Order order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId)
        {
            return TransferOrderService.CreateTransferOrder(order, orderDetails, tenantId, warehouseId, userId);
        }
        public Order SaveTransferOrder(Order order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId)
        {
            return TransferOrderService.SaveTransferOrder(order, orderDetails, tenantId, warehouseId, userId);
        }

        public List<ProductMarketReplenishModel> AutoTransferOrdersForMobileLocations(int tenantId)
        {
            return TransferOrderService.AutoTransferOrdersForMobileLocations(tenantId);
        }

        public Order CreateSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return SalesOrderService.CreateSalesOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public Order SaveSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return SalesOrderService.SaveSalesOrder(order, shipmentAndRecipientInfo, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }

        public bool DeleteSalesOrderDetailById(int orderDetailId, int userId)
        {
            return SalesOrderService.DeleteSalesOrderDetailById(orderDetailId, userId);
        }

        public IQueryable<OrderProcess> GetAllSalesConsignments(int tenantId, int warehouseId, int? InventoryTransactionId = null, int? orderstatusId = null)
        {
            return SalesOrderService.GetAllSalesConsignments(tenantId, warehouseId, InventoryTransactionId, orderstatusId);
        }

        public IQueryable<SalesOrderViewModel> GetAllActiveSalesOrdersIq(int tenantId, int warehouseId, int? statusId = null)
        {
            return SalesOrderService.GetAllActiveSalesOrdersIq(tenantId, warehouseId, statusId);
        }

        public IQueryable<SalesOrderViewModel> GetAllCompletedSalesOrdersIq(int tenantId, int warehouseId, int? type = null)
        {
            return SalesOrderService.GetAllCompletedSalesOrdersIq(tenantId, warehouseId, type);
        }

        public IQueryable<SalesOrderViewModel> GetAllDirectSalesOrdersIq(int tenantId, int warehouseId, int? statusId = null)
        {
            return SalesOrderService.GetAllDirectSalesOrdersIq(tenantId, warehouseId, statusId);
        }
        public IQueryable<SalesOrderViewModel> GetAllReturnOrders(int tenantId, int warehouseId, int? statusId = null)
        {
            return SalesOrderService.GetAllReturnOrders(tenantId, warehouseId, statusId);
        }


        public bool AuthoriseSalesOrder(int orderId, int userId, string notes, bool unauthorize = false)
        {
            return SalesOrderService.AuthoriseSalesOrder(orderId, userId, notes, unauthorize);
        }

        public List<SalesOrderViewModel> GetAllSalesOrdersForPalletsByAccount(int tenantId, int accountId)
        {
            return SalesOrderService.GetAllSalesOrdersForPalletsByAccount(tenantId, accountId);
        }
        public Order OrderProcessAutoComplete(int orderId, string deliveryNumber, int userId, bool includeProcessing, bool acknowledged)
        {
            return OrderService.OrderProcessAutoComplete(orderId, deliveryNumber, userId, includeProcessing, acknowledged);
        }

        public Order DeleteOrderById(int orderId, int userId)
        {
            return OrderService.DeleteOrderById(orderId, userId);
        }

        public List<AwaitingAuthorisationOrdersViewModel> GetAllOrdersAwaitingAuthorisation(int tenantId, int warehouseId, int? OrderStatusId = null)
        {
            return OrderService.GetAllOrdersAwaitingAuthorisation(tenantId, warehouseId, OrderStatusId);
        }

        public IQueryable<Order> GetAllWorksOrders(int tenantId)
        {
            return WorksOrderService.GetAllWorksOrders(tenantId);
        }

        public IQueryable<OrderReceiveCount> GetAllOrderReceiveCounts(int tenantId, int warehouseId, DateTime? dateUpdated)
        {
            return OrderService.GetAllOrderReceiveCounts(tenantId, warehouseId, dateUpdated);
        }

        public OrderReceiveCountSync SaveOrderReceiveCount(OrderReceiveCountSync countRecord, Terminals terminal)
        {
            return OrderService.SaveOrderReceiveCount(countRecord, terminal);
        }

        public IQueryable<OrderReceiveCount> GetAllGoodsReceiveNotes(int tenantId, int warehouseId)
        {
            return PurchaseOrderService.GetAllGoodsReceiveNotes(tenantId, warehouseId);
        }

        public IQueryable<OrderReceiveCountDetail> GetGoodsReceiveNoteDetailsById(Guid id)
        {
            return PurchaseOrderService.GetGoodsReceiveNoteDetailsById(id);
        }

        public PalletTracking GetVerifedPallet(string serial, int productId, int tenantId, int warehouseId, int? type = null, int? palletTrackingId = null, DateTime? dates = null, int? orderId = null)
        {
            return PurchaseOrderService.GetVerifedPallet(serial, productId, tenantId, warehouseId, type, palletTrackingId, dates, orderId);
        }
        public int ProcessPalletTrackingSerial(GoodsReturnRequestSync serials, string groupToken = null, bool process = false)
        {
            return PurchaseOrderService.ProcessPalletTrackingSerial(serials, groupToken, process);
        }

        public PalletTracking GetSerialByPalletTrackingScheme(int productId, int palletTrackingSchemeId, int teanantId, int warehouseId)
        {

            return SalesOrderService.GetSerialByPalletTrackingScheme(productId, palletTrackingSchemeId, teanantId, warehouseId);
        }
        public PalletTracking GetUpdatedSerial(int productId, int palletTrackingSchemeId, int tenantId, int warehouseId, List<string> serial)
        {

            return SalesOrderService.GetUpdatedSerial(productId, palletTrackingSchemeId, tenantId, warehouseId, serial);
        }
        public Order DeleteTransferOrder(int orderId, int tenantId, int warehouseId, int userId)
        {

            return TransferOrderService.DeleteTransferOrder(orderId, tenantId, warehouseId, userId);
        }
        public InventoryTransaction AddGoodsReturnPallet(List<string> serials, string orderNumber, int prodId, int transactionTypeId, decimal quantity, int? OrderId, int tenantId, int currentWarehouseId, int UserId, int palletTrackingId = 0)
        {
            return OrderService.AddGoodsReturnPallet(serials, orderNumber, prodId, transactionTypeId, quantity, OrderId, tenantId, currentWarehouseId, UserId, palletTrackingId);
        }
        public decimal QunatityForOrderDetail(int orderDetailId)
        {
            return OrderService.QunatityForOrderDetail(orderDetailId);


        }
        public string UpdateOrderProcessDetail(int OrderProcessDetailId, decimal Quantity, int CurrentUserId, int CurrentTenantId, int? SerialId, int? PalletTrackingId, bool? wastedReturn = false)
        {
            return OrderService.UpdateOrderProcessDetail(OrderProcessDetailId, Quantity, CurrentUserId, CurrentTenantId, SerialId, PalletTrackingId, wastedReturn);
        }
        public bool UpdateOrderInvoiceNumber(int orderProcessId, string InvoiceNumber, DateTime? InvoiceDate)
        {
            return OrderService.UpdateOrderInvoiceNumber(orderProcessId, InvoiceNumber, InvoiceDate);
        }
        public Order SaveDirectSalesOrder(Order order, int tenantId, int warehouseId,
         int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null)
        {
            return OrderService.SaveDirectSalesOrder(order, tenantId, warehouseId, userId, orderDetails, orderNotes);
        }
        public List<OrderProcess> GetALLOrderProcessByOrderProcessId(int orderProcessid)
        {
            return OrderService.GetALLOrderProcessByOrderProcessId(orderProcessid);
        }
        public bool UpdateDeliveryAddress(AccountShipmentInfo accountShipmentInfo)
        {


            return OrderService.UpdateDeliveryAddress(accountShipmentInfo);
        }

        public bool UpdateDateInOrder(int OrderId)
        {
            return OrderService.UpdateDateInOrder(OrderId);
        }
        public int[] GetOrderProcessbyOrderId(int OrderId)
        {
            return OrderService.GetOrderProcessbyOrderId(OrderId);
        }
        public bool CheckOrdersAuthroization(decimal OrderTotal, int InventoryTransactionType, int TenantId, int UserId, double? CreditLimit = null)
        {
            return OrderService.CheckOrdersAuthroization(OrderTotal, InventoryTransactionType, TenantId, UserId, CreditLimit);
        }

        public decimal GetDiscountOnTotalCost(int accountId, decimal OrdersTotal)
        {
            return OrderService.GetDiscountOnTotalCost(accountId, OrdersTotal);
        }

        public Order CreateShopOrder(int accountId, List<OrderDetail> orderDetail, int tenantId, int UserId, int warehouseId)
        {
            return OrderService.CreateShopOrder(accountId, orderDetail, tenantId, UserId, warehouseId);


        }
    }
}