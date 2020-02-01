using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class LocationGroup
    {
        [Key]
        [Display(Name = "Location Group Id")]
        public int LocationGroupId { get; set; }  
        [StringLength(50)]
        [Display(Name = "Description")]
        [Remote("IsLocationGroupAvailable","Locations", AdditionalFields = "LocationGroupId")]
        [Required(ErrorMessage = "Location Group is required")]
        public string Locdescription { get; set; }
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
        public virtual ICollection<Locations> Locations { get; set; }
    }
}