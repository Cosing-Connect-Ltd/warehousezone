using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteContentPages : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public string Title { get; set; }

        public string ShortDescription { get; set; }
        [Display(Name = "Meta Title")]
        public string MetaTitle { get; set; }
        [Display(Name = "Meta Description")]
        public string MetaDescription { get; set; }
        [Required]
        [Display(Name = "Page Url")]
        public string pageUrl { get; set; }
        public string Image { get; set; }
        public string ImageAltTag { get; set; }
        [Required]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Content")]
        public string Content { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Content Type")]
        public ContentType Type { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}