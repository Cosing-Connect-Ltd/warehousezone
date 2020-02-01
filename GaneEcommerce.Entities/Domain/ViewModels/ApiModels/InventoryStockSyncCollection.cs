using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class InventoryStockSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<InventoryStockSync> InventoryStocks { get; set; }
    }


    public class InventoryStockSync
    {
        public int InventorystockId { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }

        public decimal InStock { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsDeleted { get; set; }

        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public ProductLocationsResponse LocationDetails { get; set; }
    }
}