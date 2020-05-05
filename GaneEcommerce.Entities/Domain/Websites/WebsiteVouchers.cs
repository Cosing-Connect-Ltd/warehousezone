using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name="Code")]
        public string Code { get; set; }
        [Display(Name = "Value")]
        public decimal Value { get; set; }
        public bool Shared { get; set; }
        [Display(Name = "Users")]
        public int? UserId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }
    }
}