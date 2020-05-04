using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteDiscountCodes : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int SiteID { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal MinimumBasketValue { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool FreeShippig { get; set; }
        public bool SingleUse { get; set; }
        public bool IsActive { get; set; }
        public WebsiteDiscountTypeEnum DiscountType { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        public virtual ICollection<WebsiteDiscountProductsMap> WebsiteDiscountProductsMap { get; set; }
    }
}