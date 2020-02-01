using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class GoodsReturnRequestSync
    {
        public string SerialNo { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public decimal? Quantity { get; set; }
        public string ReturnReason { get; set; }
        public string deliveryNumber { get; set; }
        public string OrderNumber { get; set; }
        public int tenantId { get; set; }
        public int warehouseId { get; set; }
        public int userId { get; set; }
        public int? PalletTrackingId { get; set; }
        public int? OrderDetailID { get; set; }
        public bool? sellableFormat { get; set; }
        public bool? MissingTrackingNo { get; set; }
        public int? InventoryTransactionType { get; set; }
        public List<string> ProductSerials { get; set; }
        public List<PalleTrackingProcess> PalleTrackingProcess { get; set; }
        public Guid TransactionLogId { get; set; }
    }

    public class GoodsReturnResponse : GoodsReturnRequestSync
    {
        public bool IsSuccess { get; set; }
        public bool CanProceed { get; set; }
        public string FailureMessage { get; set; }

        public int orderId { get; set; }
    }


}