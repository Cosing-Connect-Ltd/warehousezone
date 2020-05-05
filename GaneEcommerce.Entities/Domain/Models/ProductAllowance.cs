using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductAllowance : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required]

        public decimal Quantity { get; set; }
        [Required]
        [Display(Name = "Roles")]
        public int RolesId { get; set; }
        [Display(Name="Per X Day")]
        public int PerXDays { get; set; }
        public int? ProductId { get; set; }
        [Display(Name = "Allowance Group")]
        public int? AllowanceGroupId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        [ForeignKey("RolesId")]
        public virtual Roles Roles { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }
        [ForeignKey("AllowanceGroupId")]
        public virtual ProductAllowanceGroup ProductAllowanceGroup { get; set; }
        public virtual ICollection<ProductAllowanceAdjustmentLog> ProductAllowanceAdjustmentLogs { get; set; }
    }
}