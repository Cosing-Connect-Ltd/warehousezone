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
            ProductGroups = new HashSet<ProductGroups>();
        }
        [Key]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        [StringLength(250)]
        [Required]
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
        [Display(Name = "File")]
        public string ImagePath { get; set; }
        public virtual ICollection<ProductMaster> Products { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public int? AccountID { get; set; }
        public string DeliverectCategoryId { get; set; }
        public int SortOrder { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public virtual ICollection<ProductGroups> ProductGroups { get; set; }


    }
}
