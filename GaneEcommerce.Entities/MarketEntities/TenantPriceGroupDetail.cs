using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using Ganedata.Core.Models;

namespace Ganedata.Core.Entities.Domain
{
    
    public class TenantPriceGroupDetail : PersistableEntity<int>
    {
        public TenantPriceGroupDetail()
        {
        }
        [Key]
        public int PriceGroupDetailID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal SpecialPrice { get; set; }

        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual ProductMaster Product { get; set; }

        public int PriceGroupID { get; set; }
        [ForeignKey("PriceGroupID")]
        public virtual TenantPriceGroups PriceGroup { get; set; }

        public virtual ICollection<ProductSpecialAttributePrice> ProductSpecialAttributePrices { get; set; } = new HashSet<ProductSpecialAttributePrice>();
    }

    public class ProductSpecialAttributePrice : PersistableEntity<int> 
    {
        [Key]
        public int ProductSpecialAttributePriceId { get; set; }

        //For Auditing purpose
        public int ProductAttributeId { get; set; }
        [ForeignKey("ProductAttributeId")]
        public virtual ProductAttributes ProductAttribute { get; set; }

        //Size/Scoop that helps the UI to show its elements
        public LoyaltyProductAttributeTypeEnumSync ProductAttributeType { get; set; }
        public int ProductAttributeValueId { get; set; }
        [ForeignKey("ProductAttributeValueId")]
        public virtual ProductAttributeValues ProductAttributeValue { get; set; }

        public decimal? AttributeSpecificPrice { get; set; }
        public int SortOrder { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductMaster Product { get; set; }


        public int PriceGroupDetailID { get; set; }
        [ForeignKey("PriceGroupDetailID")]
        public virtual TenantPriceGroupDetail PriceGroupDetail { get; set; }
    }
}