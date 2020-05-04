using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteVouchers : PersistableEntity<int>
    {
        public Guid Id { get; set; }
        public int SiteID { get; set; }
        //auto generated atleast 16 characters
        public string Code { get; set; }
        public decimal Value { get; set; }
        public bool Shared { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }
    }
}