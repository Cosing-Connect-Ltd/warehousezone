using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class ApiCredentials : PersistableEntity<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApiUrl { get; set; }
        public string ApiKey { get; set; }
        public ApiTypes ApiTypes { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string AccountNumber { get; set; }
        public int DefaultWarehouseId { get; set; }
        public int? SiteID { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("DefaultWarehouseId")]
        public virtual TenantLocations Warehouse { get; set; }


    }
}