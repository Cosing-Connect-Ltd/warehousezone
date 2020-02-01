using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Models
{
    public class StockTakeSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<StockTakeSync> StockTakes { get; set; }
    }
    public class StockTakeSync
    {
        public int StockTakeId { get; set; }
        public string StockTakeReference { get; set; }
        public string StockTakeDescription { get; set; }

        public int StockTakeStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WarehouseName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsDeleted { get; set; }
        public int WarehouseId { get; set; }

    }


}