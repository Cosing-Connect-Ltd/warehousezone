using System;
using System.Collections.Generic;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Models
{

    public class MarketJobsSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<MyJobSync> Jobs { get; set; }
    }

    public class MyJobSync : MarketJobAllocationModel
    {

    }

    public class MarketJobSync
    {
        public int MarketJobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AccountID { get; set; }
        public int LatestStatusID { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
        public string SerialNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid TransactionLogId { get; set; }
    }
}