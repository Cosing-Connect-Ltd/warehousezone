using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteNavigation : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public string Image { get; set; }
        public string ImageAltTag { get; set; }
        public string HoverImage { get; set; }
        public string HoverImageAltTag { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public WebsiteNavigationType Type { get; set; }
        public int? ContentPageId { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("ParentId")]
        public virtual WebsiteNavigation Parent { get; set; }
        public virtual ICollection<ProductsNavigationMap> ProductsNavigationMap { get; set; }
        [NotMapped]
        public string SelectedProductIds { get; set; }
    }
}