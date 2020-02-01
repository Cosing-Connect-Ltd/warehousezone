using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class AuthUserLogin
    {
        public AuthUserLogin()
        {
            this.AuthUserLoginActivities = new HashSet<AuthUserLoginActivity>();
        }

        [Key]
        [Display(Name = "Login Id")]
        public int UserLoginId { get; set; }
        [Display(Name = "User")]
        public int UserId { get; set; }
        [Display(Name = "Login Date")]
        public DateTime DateLoggedIn { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        public virtual ICollection<AuthUserLoginActivity> AuthUserLoginActivities { get; set; }
        [ForeignKey("UserId")] 
        public virtual AuthUser AuthUser { get; set; }
    }
}
