using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{   
    public class AuthUserprofile : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Profile Id")]
        public int UserProfileId { get; set; }
        [Display(Name = "User Id")]
        public int UserId { get; set; }
        [MaxLength(200)]
        [Display(Name = "Profile Key")]
        public string UserProfileKey { get; set; }
        [Display(Name = "Profile Value")]
        public string UserProfileKeyValue { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("UserId")]
        public virtual AuthUser AuthUser { get; set; }
    }
}
