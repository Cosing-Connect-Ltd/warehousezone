using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{

    [Serializable]
    public class TenantDepartments : PersistableEntity<int>
    {
        public TenantDepartments()
        {
            Products = new HashSet<ProductMaster>();
        }
        [Key]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        [StringLength(250)]
        [Required]
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        public virtual ICollection<ProductMaster> Products { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }

        public int? AccountID { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
    }
}
