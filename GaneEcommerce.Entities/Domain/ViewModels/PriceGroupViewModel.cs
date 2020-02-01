using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    public class PriceGroupViewModel
    {
        public int PriceGroupId { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name = "Discount Percent")]
        public decimal? Percent { get; set; }
        public int TenantId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Apply Discount On Total")]
        public bool ApplyDiscountOnTotal { get; set; }
        [Display(Name = "Apply Discount On Special Price")]
        public bool ApplyDiscountOnSpecialPrice { get; set; }
    }
}