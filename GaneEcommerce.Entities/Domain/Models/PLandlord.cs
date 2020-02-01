using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PLandlord : PContactInfo
    {
        [Key]
        public int PLandlordId { get; set; }
        [Required]
        [Display(Name = "Landlord Code")]
        public string LandlordCode { get; set; }
        [Required]
        [Display(Name = "Landlord Full name")]
        public string LandlordFullname { get; set; }
        [Display(Name = "Landlord Salutation")]
        public string LandlordSalutation { get; set; }
        [Display(Name = "Landlord Status")]
        public string LandlordStatus { get; set; }
        [Display(Name = "Landlord Added")]
        public DateTime? LandlordAdded { get; set; }
        [Display(Name = "Landlord Notes 1")]
        public string LandlordNotes1 { get; set; }
        [Display(Name = "Landlord Notes 2")]
        public string LandlordNotes2 { get; set; }
        [Display(Name = "User Notes 1")]
        public string UserNotes1 { get; set; }
        [Display(Name = "User Notes 2")]
        public string UserNotes2 { get; set; }
        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }
        [Display(Name = "Property")]
        public DateTime DateCreated { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? DateUpdated { get; set; }

        public bool? IsDeleted { get; set; }
        public int? UpdatedUserId { get; set; }


    }
}