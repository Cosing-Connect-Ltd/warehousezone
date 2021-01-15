using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class ProductLocationStocksSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<ProductLocationStocksSync> ProductLocationStocksSync { get; set; }
    }

    public class ProductLocationStocksSync
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public decimal Quantity { get; set; }
        public int WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}