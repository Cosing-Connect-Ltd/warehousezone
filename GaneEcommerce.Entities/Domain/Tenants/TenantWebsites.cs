namespace Ganedata.Core.Entities.Domain
{
    using Ganedata.Core.Entities.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // This table will hold different loan types. eg. short term loan, long term loan and loan period will be defined in days.
    [Serializable]
    public partial class TenantWebsites : PersistableEntity<int>
    {
        public TenantWebsites()
        {
            ProductsWebsitesMap = new HashSet<ProductsWebsitesMap>();
        }

        [Key]
        public int SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public TenantWebsiteTypes SiteType { get; set; }
        public string SiteApiUrl { get; set; }
        public string ApiToken { get; set; }
        public int WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations Warehouse { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<ProductsWebsitesMap> ProductsWebsitesMap { get; set; }
    }
}
