namespace Ganedata.Core.Entities.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    public partial class TenantWarranty : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Warranty Id")]
        public int WarrantyID { get; set; }
        [Display(Name = "Warranty Name")]
        public string WarrantyName { get; set; }
        [Display(Name = "Warranty Detail")]
        public string WarrantyDescription { get; set; }
        [Required]
        [Display(Name = "Is Percent")]
        // fixed price = 0, Percentage = 1
        public bool IsPercent { get; set; }
        // If WarrantyType = 1
        [Display(Name = "Percent Of Price")]
        public decimal PercentageOfPrice { get; set; }
        // If WarrantyType = 0
        [Display(Name = "Fixed Amount")]
        public decimal FixedPrice { get; set; }

        public int WarrantyDays { get; set; }

        [Display(Name = "Postage Type")]
        public int PostageTypeId { get; set; }

        // 0 = No hot swap, 1 = Hot swap agreed
        [Required]
        [Display(Name = "Hot Swap")]
        public bool HotSwap { get; set; }

        [ForeignKey("PostageTypeId")]
        public virtual OrderConsignmentTypes OrderConsignmentTypes { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}
