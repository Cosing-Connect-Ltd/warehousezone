using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    
    public class AuthActivityGroupMap : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Group Map Id")]
        public int ActivityGroupMapId { get; set; }
        [Display(Name = "Activity Id")]
        public int ActivityId { get; set; }
        [Display(Name = "Group Id")]
        public int ActivityGroupId { get; set; }
        [ForeignKey("ActivityId")]
        public virtual AuthActivity AuthActivity { get; set; }
        [ForeignKey("ActivityGroupId")]
        public virtual AuthActivityGroup AuthActivityGroup { get; set; }
    }
}
