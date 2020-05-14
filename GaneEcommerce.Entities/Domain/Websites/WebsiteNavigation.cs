using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Image Alt")]
        public string ImageAltTag { get; set; }
        [Display(Name = "Hover Image")]
        public string HoverImage { get; set; }
        [Display(Name = "Hover Alt")]
        public string HoverImageAltTag { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Parent")]
        public int? ParentId { get; set; }
        public WebsiteNavigationType Type { get; set; }
        [Display(Name = "Content Page")]
        public int? ContentPageId { get; set; }
        [Display(Name = "Show In Navigation")]
        public bool ShowInNavigation { get; set; }
        [Display(Name = "Show In Footer")]
        public bool ShowInFooter { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("ParentId")]
        public virtual WebsiteNavigation Parent { get; set; }
        public virtual ICollection<ProductsNavigationMap> ProductsNavigationMap { get; set; }
        [NotMapped]
        public string SelectedProductIds { get; set; }
        [ForeignKey("ContentPageId")]
        public virtual WebsiteContentPages WebsiteContentPages { get; set; }
    }
}