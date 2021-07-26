using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IOrderService
    {
        string GenerateNextOrderNumber(InventoryTransactionTypeEnum type, int tenantId);

        string GetAuthorisedUserNameById(int userId);
        IEnumerable<AuthUser> GetAllAuthorisedUsers(int tenantId, bool includeSuperUser = false);
        IEnumerable<JobType> GetAllValidJobTypes(int tenantId);
        IEnumerable<JobSubType> GetAllValidJobSubTypes(int tenantId);
        IQueryable<Order> GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId, int? warehouseId = null);
        Order GetOrderById(int orderId);
        bool IsOrderNumberAvailable(string orderNumber, int orderId);
        Order CompleteOrder(int orderId, int userId);
        Order FinishinghOrder(int orderId, int userId, int wareHouseId);

        Order CreateOrder(Order order, int tenantId, int warehouseId, int userId, IEnumerable<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null);

        Order SaveOrder(Order order, int tenantId, int warehouseId, int userId,
            IEnumerable<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null);
        IEnumerable<OrderDetail> GetAllValidOrderDetailsByOrderId(int orderId);
        IQueryable<Order> GetValidSalesOrder(int tenantId, int warehouseId);
        List<OrderDetailsViewModel> GetSalesOrderDetails(int id, int tenantId);
        List<OrderDetailsViewModel> GetDirectSalesOrderDetails(int id, int tenantId);

        List<OrderProofOfDelivery> GetOrderProofsByOrderProcessId(int OrderId, int TenantId);
        List<OrderDetailsViewModel> GetPalletOrdersDetails(int id, int tenantId, bool excludeProcessed = false);
        List<OrderDetailsViewModel> GetTransferOrderDetails(int orderId, int warehouseId);
        List<OrderDetailsViewModel> GetWorksOrderDetails(int id, int tenantId);
        IQueryable<TransferOrderViewModel> GetTransferInOrderViewModelDetails(int toWarehouseId, int tenantId, OrderStatusEnum? type = null);
        IQueryable<TransferOrderViewModel> GetTransferOutOrderViewModelDetailsIq(int fromWarehouseId, int tenantId, OrderStatusEnum? type = null);
        List<OrderProcessDetail> GetOrderProcessDetailsByProcessId(int processId);
        List<OrderProcess> GetOrderProcesssDeliveriesForWarehouse(int warehouseId, int tenantId);
        List<OrderProcess> GetOrderProcessConsignmentsForWarehouse(int warehouseId, int tenantId);
        List<OrderProcessDetail> GetOrderProcessesDetailsForOrderProduct(int orderId, int productId);
        List<InventoryTransaction> GetAllReturnsForOrderProduct(int orderId, int productId);

        IQueryable<Order> GetOrdersHistory(int UserId, int SiteId);

        OrderProcess GetOrderProcessByDeliveryNumber(int orderId, InventoryTransactionTypeEnum InventoryTransactionTypeId, string deliveryNumber, int userId, DateTime? createdDate = null, int warehouseId = 0, AccountShipmentInfo shipmentInfo = null);
        IQueryable<Order> GetAllOrders(int tenantId, int warehouseId = 0, bool excludeProforma = false, DateTime? reqDate = null, bool includeDeleted = false);
        IQueryable<Order> GetAllDirectSalesOrdersByAccount(int tenantId, int accountId, DateTime? reqDate = null, bool includeDeleted = false);
        IEnumerable<OrderIdsWithStatus> GetAllOrderIdsWithStatus(int tenantId, int warehouseId = 0);
        IQueryable<Order> GetAllOrdersIncludingNavProperties(int tenantId, int warehouseId = 0);
        Order UpdateOrderStatus(int orderId, OrderStatusEnum statusId, int userId);

        Order GetValidSalesOrderByOrderNumber(string orderNumber, int tenantId);

        List<OrderDetail> GetOrderDetailsForProduct(int orderId, int productId, int TenantId);

        OrderDetail SaveOrderDetail(OrderDetail podetail, int tenantId, int userId);
        void RemoveOrderDetail(int orderDetailId, int tenantId, int userId);
        OrderDetail GetOrderDetailsById(int orderDetailId);
        List<OrderDetail> GetAllOrderDetailsForOrderAccount(int supplierAccountId, int poOrderId, int tenantId);
        List<OrderProcessDetail> GetAllOrderProcessesByOrderDetailId(int orderDetailId, int warehouseId);
        IQueryable<OrderProcess> GetAllOrderProcesses(DateTime? ordersAfter, int? orderId = 0, OrderProcessStatusEnum? orderProcessStatusId = null, InventoryTransactionTypeEnum? transactionTypeId = null, bool includeDeleted = false);
        List<OrderProcessDetail> GetAllOrderProcessesDetails(DateTime? updatedAfter, int? orderProcessId = 0);

        OrderProcess CreateOrderProcess(int orderId, string deliveryNo, int[] product, decimal[] qty,
            decimal[] qtyReceived, int[] lines, string serialStamp, int currentUserId, int currentTenantId,
            int warehouseId);

        OrderProcess SaveOrderProcess(int orderProcessId, int[] product, decimal[] qty, decimal[] qtyReceived,
            int[] lines, int currentUserId, int currentTenantId, int warehouseId);

        OrdersSync SaveOrderProcessSync(OrderProcessesSync orderProcess, Terminals terminal);

        IQueryable<ProductMaster> GetAllValidProduct(int currentTenantId);

        OrderProcess GetOrderProcessByOrderProcessId(int orderProcessid);
        List<OrderProcess> GetALLOrderProcessByOrderProcessId(int orderProcessid);

        OrderProcessDetail GetOrderProcessDetailById(int orderProcessDetailId);

        List<OrderProcessViewModel> GetOrderProcessByWarehouseId(int warehouseId);

        List<ProductSerialis> GetProductSerialsByNumber(string serialNo, int tenantId);

        InventoryTransaction GetLastInventoryTransactionsForSerial(string serial, int tenantId);
        Order CreateOrderByOrderNumber(string orderNumber, int productId, int tenantId, int warehouseId, InventoryTransactionTypeEnum transType, int userId, decimal Quantity);
        Order GetOrderByOrderNumber(string orderNumber);
        IQueryable<Order> GetAllPendingOrdersForProcessingForDate();

        Order DeleteOrderById(int orderId, int userId);
        OrderNotes DeleteOrderNoteById(int orderNoteId, int userId);
        OrderNotes UpdateOrderNote(int noteId, string notes, int userId, int? orderId = null);
        List<OrderProcessDetail> GetOrderProcessDetailForOrders(int orderprocessId, List<int> orderIds = null);
        PalletOrderProductsCollection GetAllProductsByOrdersInPallet(int palletId);
        Order CreateDirectSalesOrder(DirectSalesViewModel model, int tenantId, int userId, int warehouseId);
        DirectSalesViewModel GetDirectSalesModelByOrderId(int orderId);
        Order OrderProcessAutoComplete(int orderId, string deliveryNumber, int userId, bool includeProcessing, bool acknowledged);
        List<AwaitingAuthorisationOrdersViewModel> GetAllOrdersAwaitingAuthorisation(int tenantId, int warehouseId, OrderStatusEnum? OrderStatusId = null);
        IQueryable<OrderReceiveCount> GetAllOrderReceiveCounts(int tenantId, int warehouseId, DateTime? dateUpdated);
        OrderReceiveCountSync SaveOrderReceiveCount(OrderReceiveCountSync countRecord, Terminals terminal);
        InventoryTransaction AddGoodsReturnPallet(List<string> serials, string orderNumber, int prodId, InventoryTransactionTypeEnum transactionTypeId, decimal quantity, int? OrderId, int tenantId, int currentWarehouseId, int UserId, int palletTrackigId = 0);

        decimal QunatityForOrderDetail(int orderDetail);
        bool UpdateOrderProcessStatus(int orderProcessId, int UserId);

        string UpdateOrderProcessDetail(int OrderProcessDetailId, decimal Quantity, int CurrentUserId, int TenantId, int? serialId, int? PalletTrackingId, bool? wastedReturn = false);

        bool UpdateOrderInvoiceNumber(int orderProcessId, string InvoiceNumber, DateTime? InvoiceDate);
        Order SaveDirectSalesOrder(Order order, int tenantId, int warehouseId,
         int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null);
        bool UpdateDeliveryAddress(AccountShipmentInfo accountShipmentInfo);
        bool UpdateDateInOrder(int OrderId);
        int[] GetOrderProcessbyOrderId(int OrderId);

        bool CheckOrdersAuthroization(decimal OrderTotal, InventoryTransactionTypeEnum InventoryTransactionType, int TenantId, int UserId, double? CreditLimit = null);
        decimal GetDiscountOnTotalCost(int accountId, decimal OrdersTotal);

        Order CreateShopOrder(CheckoutViewModel orderDetail, int tenantId, int UserId, int warehouseId, int SiteId, ShopDeliveryTypeEnum deliveryType = ShopDeliveryTypeEnum.Standard);
        bool UpdatePickerId(int OrderId, int? pickerId, int userId);
        bool UpdateOrdersPicker(int[] orderIds, int? pickerId, int userId);
        bool UpdateOrderPaypalPaymentInfo(int orderId, string nonce, Braintree.Transaction transaction);
        Order UpdateLoyaltPointsForAccount(int orderId);
        void MapToOrderSync(Order source, OrdersSync target);

        List<OrderProcessLowStockItems> ValidateStockAvailability(int orderId);
    }
}