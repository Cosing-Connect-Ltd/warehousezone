using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteWarehouses : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public int WarehouseId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantLocations { get; set; }
    }
}