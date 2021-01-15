using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{

    public class LocationSyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<LocationSync> LocationSync { get; set; }
    }
    public class LocationSync
    {
        public int LocationId { get; set; }
        public int WarehouseId { get; set; }
        public int? LocationGroupId { get; set; }
        public int? LocationTypeId { get; set; }
        public string LocationType { get; set; }
        public string LocationGroup { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public string WarehouseName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}