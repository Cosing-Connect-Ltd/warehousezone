using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    public class AuthActivityGroup : PersistableEntity<int>
    {
        public AuthActivityGroup()
        {
            this.AuthActivityGroupMaps = new HashSet<AuthActivityGroupMap>();
        }

        [Key]
        [Display(Name = "Activity Group Id")]
        public int ActivityGroupId { get; set; }
        [MaxLength(200)]
        [Display(Name = "Activity Group Name")]
        public string ActivityGroupName { get; set; }
        [MaxLength(1000)]
        [Display(Name = "Activity Group Detail")]
        public string ActivityGroupDetail { get; set; }
        public int? ActivityGroupParentId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
        [Display(Name = "Group Icon")]
        public string GroupIcon { get; set; }
        public virtual ICollection<AuthActivityGroupMap> AuthActivityGroupMaps { get; set; }

    }
}
