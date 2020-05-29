using System;
using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using WMS.VModels;

namespace Ganedata.Core.Services
{
    public interface IPurchaseOrderService
    {
        List<PurchaseOrderViewModel> GetAllPurchaseOrders(int tenantId);
        Order CreatePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId,
            int warehouseId, int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null);
        Order SavePurchaseOrder(Order order, OrderRecipientInfo shipmentAndRecipientInfo, int tenantId, int warehouseId,
            int userId, List<OrderDetail> orderDetails = null, List<OrderNotes> orderNotes = null);
        IQueryable<OrderProcess> GetAllPurchaseOrderProcesses(int tenantId, int warehouseId);
        IEnumerable<OrderProcessDetail> GetOrderProcessDetailByOrderProcessId(int orderProcessId);
        IQueryable<OrderReceiveCount> GetAllGoodsReceiveNotes(int tenantId, int warehouseId);
        IQueryable<OrderReceiveCountDetail> GetGoodsReceiveNoteDetailsById(Guid id);
        Order CreateBlindShipmentOrder(List<BSDto> bsList, int accountId, string deliveryNumber, string poNumber,
            int tenantId, int warehouseId, int userId, InventoryTransactionTypeEnum transType, AccountShipmentInfo accountShipmentInfo = null);
        List<OrderDetailsViewModel> GetPurchaseOrderDetailsById(int orderId, int tenantId);
        InventoryTransaction SubmitReceiveInventoryTransaction(InventoryTransaction model, string deliveryNumber,
            int tenantId, int warehouseId, int userId);
        Order UpdatePurchaseOrderStatus(int orderId, int orderStatusId, int userId);
        Order CancelPurchaseOrder(int orderId, int userId, int warehouseId);
        IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersInProgress(int tenantId, int warehouseId);
        IQueryable<PurchaseOrderViewModel> GetAllPurchaseOrdersCompleted(int tenantId, int warehouseId, int? type = null);
        PalletTracking GetVerifedPallet(string serial, int productId, int tenantId, int warehouseId, int? type = null, int? palletTrackingId = null, DateTime? dates = null, int? orderId = null);
        int ProcessPalletTrackingSerial(GoodsReturnRequestSync serials, string groupToken = null, bool process = false);

        IEnumerable<OrderPTenantEmailRecipient> GetAccountContactId(int OrderId);

    }
}