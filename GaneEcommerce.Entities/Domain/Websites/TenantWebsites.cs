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
        [Display(Name = "Site Name")]
        public string SiteName { get; set; }
        [Display(Name = "Description")]
        public string SiteDescription { get; set; }
        public WebsiteThemeEnum Theme { get; set; }
        public int DefaultWarehouseId { get; set; }
        [Required]
        [Display(Name = "Host Name")]
        public string HostName { get; set; }
        [Required]
        [Display(Name = "Base File Path")]
        public string BaseFilePath { get; set; }
        public string Logo { get; set; }
        [Display(Name = "Facebook Url")]
        [Url]
        public string FacebookUrl { get; set; }
        [Display(Name = "Twitter Url")]
        [Url]
        public string TwitterUrl { get; set; }
        [Display(Name = "LinkedIn Url")]
        [Url]
        public string LinkedInUrl { get; set; }
        [Display(Name = "Youtube Url")]
        [Url]
        public string YoutubeUrl { get; set; }
        [Display(Name = "Instagram Url")]
        [Url]
        public string InstaGramUrl { get; set; }
        public string FooterText { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Price Including Tax")]
        public bool ShowPricesIncludingTax { get; set; }
        [Display(Name = "Collection Is Available")]
        public bool IsCollectionAvailable { get; set; }
        [Display(Name = "Delivery Is Available")]
        public bool IsDeliveryAvailable { get; set; }

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
        public virtual ICollection<WebsiteWarehouses> WebsiteWarehouses { get; set; }
    }
}
