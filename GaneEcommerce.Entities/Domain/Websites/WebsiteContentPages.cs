using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteContentPages : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public string Title { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Contant { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
    }
}