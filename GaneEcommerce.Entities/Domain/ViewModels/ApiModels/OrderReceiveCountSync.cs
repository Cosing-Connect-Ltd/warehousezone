using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{

    public class OrderReceiveCountSyncCollection
    {
        public OrderReceiveCountSyncCollection()
        {
            OrderReceiveCountSync = new List<OrderReceiveCountSync>();
        }
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<OrderReceiveCountSync> OrderReceiveCountSync { get; set; }
    }

    public class OrderReceiveCountSync
    {
        public Guid ReceiveCountId { get; set; }
        public int OrderID { get; set; }
        public string ReferenceNo { get; set; }
        public string Notes { get; set; }
        public int WarehouseId { get; set; }
        public int TenantId { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<OrderReceiveCountDetailSync> OrderReceiveCountDetail { get; set; }
    }

    public class OrderReceiveCountDetailSync
    {
        public Guid ReceiveCountDetailId { get; set; }
        public Guid ReceiveCountId { get; set; }
        public int ProductId { get; set; }
        public decimal Counted { get; set; }
        public decimal Demaged { get; set; }
        public int OrderDetailID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int TenentId { get; set; }
        public bool? IsDeleted { get; set; }
    }

}