using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    
    public class TenantPriceGroupDetail : PersistableEntity<int>
    {
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
    }
}