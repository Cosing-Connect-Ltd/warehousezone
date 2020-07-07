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
    public class WebsiteDeliveryNavigation : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string TextDescription { get; set; }
        
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Icon Name")]
        public string IconName { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [ForeignKey("SiteId")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}