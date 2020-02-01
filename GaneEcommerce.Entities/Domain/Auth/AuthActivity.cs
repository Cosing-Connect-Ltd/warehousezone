using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class AuthActivity : PersistableEntity<int>
    {
        public AuthActivity()
        {
            AuthActivityGroupMaps = new HashSet<AuthActivityGroupMap>();
            AuthPermissions = new HashSet<AuthPermission>();
        }

        [Key]
        [Display(Name = "Activity Id")]
        public int ActivityId { get; set; }
        [MaxLength(200)]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; }
        [Display(Name = "Activity Controller")]
        public string ActivityController { get; set; }
        [Display(Name = "Activity Action")]
        public string ActivityAction { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Include in Navigation")]
        public bool? RightNav { get; set; }
        [Display(Name = "Exclude Permissions")]
        public bool? ExcludePermission { get; set; }
        [Display(Name = "Super Admin")]
        public bool? SuperAdmin { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        public virtual ICollection<AuthActivityGroupMap> AuthActivityGroupMaps { get; set; }
        public virtual ICollection<AuthPermission> AuthPermissions { get; set; }
        public int? ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
    }
}
