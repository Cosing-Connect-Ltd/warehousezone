using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class ProductSerialSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<ProductSerialSync> ProductSerials { get; set; }
    }
    public class ProductSerialSync
    {
        public int SerialID { get; set; }
        public string SerialNo { get; set; }
        public int ProductId { get; set; }
        public InventoryTransactionTypeEnum CurrentStatus { get; set; }
        public string Batch { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal BuyPrice { get; set; }
    }
}