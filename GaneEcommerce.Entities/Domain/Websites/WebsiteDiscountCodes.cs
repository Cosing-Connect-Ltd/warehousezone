using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name= "From Date")]
        [Column(TypeName = "date")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "To Date")]
        [Column(TypeName = "date")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "Min.Basket Value")]
        public decimal MinimumBasketValue { get; set; }
        [Display(Name = "Discount Percent")]
        public decimal DiscountPercent { get; set; }
        [Display(Name = "Free Shippig")]
        public bool FreeShippig { get; set; }
        [Display(Name = "Single Use")]
        public bool SingleUse { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public WebsiteDiscountTypeEnum DiscountType { get; set; }
        [ForeignKey("SiteID")]
        public virtual TenantWebsites TenantWebsites { get; set; }
        public virtual ICollection<WebsiteDiscountProductsMap> WebsiteDiscountProductsMap { get; set; }

        [NotMapped]
        public string SelectedProductIds { get; set; }
    }
}