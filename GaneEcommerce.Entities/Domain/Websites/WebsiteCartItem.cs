using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteCartItem : PersistableEntity<int>
    {
        public WebsiteCartItem()
        {
            KitProductCartItems = new HashSet<KitProductCartItem>();
        }
        public int Id { get; set; }
        public int SiteID { get; set; }
        public int ProductId { get; set; }

      

        public int UserId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }

        public ICollection<KitProductCartItem> KitProductCartItems { get; set; }
    }
}