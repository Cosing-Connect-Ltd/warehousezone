using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Assets
    {
        public Assets()
        {
            AssetLog = new HashSet<AssetLog>();
        }

        [Key]
        [Display(Name = "Asset Id")]
        public int AssetId { get; set; }
        [MaxLength(50)]
        [Display(Name = "Name")]
        public string AssetName { get; set; }
        [Display(Name = "Description")]
        public string AssetDescription { get; set; }
        [MaxLength(100)]
        [Display(Name = "Asset Code")]
        public string AssetCode { get; set; }
        [Display(Name = "Asset Tag")]
        public string AssetTag { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime DateCreated { get; set; }
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
        public int TenantId { get; set; }
        public virtual ICollection<AssetLog> AssetLog { get; set; }

    }
}