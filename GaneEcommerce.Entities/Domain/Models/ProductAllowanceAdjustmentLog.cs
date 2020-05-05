using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class ProductAllowanceAdjustmentLog : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        public int AllowanceId { get; set; }
        public string Reason { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        [ForeignKey("AllowanceId")]
        public virtual ProductAllowance ProductAllowance { get; set; }
    }
}