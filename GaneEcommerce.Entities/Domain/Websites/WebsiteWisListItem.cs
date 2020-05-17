using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteWishListItem : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public int UserId { get; set; }
        public int ProductId{ get; set; }
        public bool IsNotification { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }

        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }
    }
}