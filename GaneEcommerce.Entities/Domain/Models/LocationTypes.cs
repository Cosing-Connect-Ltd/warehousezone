using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class LocationTypes
    {
        [Key]
        [Display(Name = "Location Type Id")]
        public int LocationTypeId { get; set; }
        [Remote("IsLocationTypeAvailable","Locations", AdditionalFields = "LocationTypeId", ErrorMessage = "Location type already exists")]
        [Required(ErrorMessage = "Location Type is required")]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string LocTypeName { get; set; }
        [StringLength(50)]
        [Display(Name = "Description")]
        public string LocTypeDescription { get; set; }
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