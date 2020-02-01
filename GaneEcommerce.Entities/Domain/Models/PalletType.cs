using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PalletType
    {
        [Key]
        [Display(Name = "Product Type")]
        public int PalletTypeId { get; set; }
        
        [Remote("IsPalletTypeAvailable", "PalletTypes", AdditionalFields = "PalletTypeId", ErrorMessage = "Pallet type already exists. ")]
        [Required(ErrorMessage = "Pallet Type is required")]
        [Display(Name = "Pallet Type")]
        public string Description { get; set; }
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
        public virtual ICollection<ProductMaster> Products { get; set; }
    }
}