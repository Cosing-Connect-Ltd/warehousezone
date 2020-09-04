using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductKitType
    {
        [Key]
        [Display(Name = "Location Type Id")]
        public int Id { get; set; }
        [Remote("IsKitTypeAvailable", "ProductKitTypes", AdditionalFields = "Id", ErrorMessage = "Kit Type already exists")]
        [Required(ErrorMessage = "Kit Type is required")]
        [StringLength(50)]
        [Display(Name = "Kit Type")]
        public string Name { get; set; }
        [Display(Name = "Sort")]
        public int SortOrder { get; set; }
        [Display(Name = "Use In Parent Calculations")]
        public bool? UseInParentCalculations { get; set; }
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
    }
}