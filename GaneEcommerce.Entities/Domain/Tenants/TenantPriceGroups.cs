using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class TenantPriceGroups : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Group Id")]
        public int PriceGroupID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Discount Percentage")]
        public decimal Percent { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public bool ApplyDiscountOnTotal { get; set; }
        public bool ApplyDiscountOnSpecialPrice { get; set; }
    }
}
