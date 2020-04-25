using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductsNavigationMap : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int ProductWebsiteMapId { get; set; }
        public int NavigationId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("NavigationId")]
        public virtual WebsiteNavigation WebsiteNavigation { get; set; }
        [ForeignKey("ProductWebsiteMapId")]
        public virtual ProductsWebsitesMap ProductsWebsitesMap { get; set; }
    }
}