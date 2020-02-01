using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface ITransferOrderService
    {
        Order CreateTransferOrder(Order order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId);
        Order SaveTransferOrder(Order order, List<OrderDetail> orderDetails, int tenantId, int warehouseId, int userId);
        List<ProductMarketReplenishModel> AutoTransferOrdersForMobileLocations(int tenantId);
        Order DeleteTransferOrder(int orderId, int tenantId, int warehouseId, int userId);

    }

}