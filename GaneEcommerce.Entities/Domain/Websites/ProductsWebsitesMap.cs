using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductsWebsitesMap : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SiteID { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual ICollection<ProductsNavigationMap> ProductsNavigationMap { get; set; }
    }
}