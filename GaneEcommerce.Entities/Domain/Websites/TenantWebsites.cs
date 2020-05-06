using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{

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
        public WebsiteThemeEnum Theme { get; set; }
        public string SiteApiUrl { get; set; }
        public string ApiToken { get; set; }
        public int DefaultWarehouseId { get; set; }
        public string HostName { get; set; }
        public string Logo { get; set; }
        [Url]
        public string FacebookUrl { get; set; }
        [Url]
        public string TwitterUrl { get; set; }
        [Url]
        public string LinkedInUrl { get; set; }
        [Url]
        public string YoutubeUrl { get; set; }
        [Url]
        public string InstaGramUrl { get; set; }
        public string FooterText { get; set; }
        public bool IsActive { get; set; }
        public string WebsiteContactAddress { get; set; }
        public string WebsiteContactPhone { get; set; }
        public string WebsiteContactEmail { get; set; }
        [ForeignKey("DefaultWarehouseId")]
        public virtual TenantLocations Warehouse { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<ProductsWebsitesMap> ProductsWebsitesMap { get; set; }
        public virtual ICollection<WebsiteContentPages> WebsiteContentPages { get; set; }
        public virtual ICollection<WebsiteSlider> WebsiteSlider { get; set; }
        public virtual ICollection<WebsiteNavigation> WebsiteNavigation { get; set; }
    }
}
