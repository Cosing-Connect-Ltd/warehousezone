using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class WebsiteDiscountProductsMap : PersistableEntity<int>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("DiscountId")]
        public virtual WebsiteDiscountCodes WebsiteDiscountCodes { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
    }
}