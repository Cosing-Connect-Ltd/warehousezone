using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class GlobalAccountStatus
    {
        public GlobalAccountStatus()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        [Display(Name = "Account Status Id")]
        public int AccountStatusID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}