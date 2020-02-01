using System;
using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IWorksOrderService
    {
        Order CreateWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null);

        Order SaveWorksOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId, int userId, bool isOrderComplete, List<OrderDetail> orderDetails = null, IEnumerable<OrderNotes> orderNotes = null);

        Order SaveWorksOrderBulkSingle(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null,
            IEnumerable<OrderNotes> orderNotes = null);
        Order UpdateWorksOrderBulkSingle(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null,
            List<OrderNotes> orderNotes = null);
        List<Order> GetAllOrdersByGroupToken(Guid groupToken, int tenantId);
        List<WorksOrderViewModel> GetAllPendingWorksOrders(int tenantId, Guid? groupToken = null);
        IQueryable<WorksOrderViewModel> GetAllPendingWorksOrdersIq(int tenantId, Guid? groupToken = null, int? propertyId=null);
        IQueryable<WorksOrderViewModel> GetAllCompletedWorksOrdersIq(int tenantId, int? propertyId,int? type=null);

        IQueryable<Order> GetAllWorksOrders(int tenantId);
    }
}