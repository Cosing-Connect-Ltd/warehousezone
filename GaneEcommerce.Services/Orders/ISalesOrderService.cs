using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface ISalesOrderService
    {
        Order CreateSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null);

        Order SaveSalesOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null);

        bool DeleteSalesOrderDetailById(int orderDetailId, int userId);

        IQueryable<OrderProcess> GetAllSalesConsignments(int tenantId, int warehouseId, int? InventoryTransactionId = null, OrderProcessStatusEnum? orderstatusId = null);

        IQueryable<SalesOrderViewModel> GetAllActiveSalesOrdersIq(int tenantId, int warehouseId, IEnumerable<OrderStatusEnum> statusIds = null);

        IQueryable<SalesOrderViewModel> GetAllCompletedSalesOrdersIq(int tenantId, int warehouseId, OrderStatusEnum? type = null);

        IQueryable<SalesOrderViewModel> GetAllPickerAssignedSalesOrders(int tenantId, int warehouseId, IEnumerable<OrderStatusEnum> statusIds = null);

        IQueryable<SalesOrderViewModel> GetAllPickerUnassignedSalesOrders(int tenantId, int warehouseId, IEnumerable<OrderStatusEnum> statusIds = null);

        IQueryable<SalesOrderViewModel> GetAllPaidDirectSalesOrdersIq(int tenantId, int warehouseId, OrderStatusEnum? statusId = null);

        IQueryable<SalesOrderViewModel> GetAllReturnOrders(int tenantId, int warehouseId, OrderStatusEnum? statusId = null);

        List<ProductOrdersDetailViewModel> GetAllOrdersByProductId(InventoryTransactionTypeEnum[] inventoryTransactionType, int productId, DateTime startDate, DateTime endDate, int tenantId, int warehouseId, int[] accountIds, int[] ownerIds, int[] accountSectorIds, int? marketId);

        List<ProductOrdersDetailViewModel> GetPurhaseOrderAgainstProductId(int[] productIds, int tenantId, int warehouseId);
        bool AuthoriseSalesOrder(int orderId, int userId, string notes, bool unauthorize = false);

        List<SalesOrderViewModel> GetAllSalesOrdersForPalletsByAccount(int tenantId, int accountId);

        PalletTracking GetSerialByPalletTrackingScheme(int productId, int palletTrackingSchemeId, int teanantId, int warehouseId);

        PalletTracking GetUpdatedSerial(int productId, int palletTrackingSchemeId, int tenantId, int warehouseId, List<string> serial);

        List<Order> GetDirectSaleOrders(int? orderId);
    }
}