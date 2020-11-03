using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ApiCredentials : PersistableEntity<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string SiteTitle { get; set; }
        public string Password { get; set; }
        [DisplayName("Api URL")]
        public string ApiUrl { get; set; }
        [DisplayName("Api Key")]
        public string ApiKey { get; set; }
        [DisplayName("Api Type")]
        public ApiTypes ApiTypes { get; set; }
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [DisplayName("Location")]
        public int DefaultWarehouseId { get; set; }
        [DisplayName("Website")]
        public int? SiteID { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("DefaultWarehouseId")]
        public virtual TenantLocations Warehouse { get; set; }
    }
}