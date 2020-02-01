using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductGroups
    {
        public ProductGroups()
        {
            Products = new HashSet<ProductMaster>();
        }

        [Key]
        [Display(Name = "Product Group Id")]
        public int ProductGroupId { get; set; }


        //[StringLength(20)]
        [Remote("IsProductGroupAvailable", "ProductGroup", AdditionalFields = "ProductGroupId", ErrorMessage = "Product group name already exists.")]
        [Required(ErrorMessage = "Product Group Name is required")]
        [Display(Name = "Product Group")]
        public string ProductGroup { get; set; }
        public string IconPath { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenentId { get; set; }
        public int SortOrder { get; set; }
        public virtual ICollection<ProductMaster> Products { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual TenantDepartments TenantDepartments { get; set; }

    }
}
