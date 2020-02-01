using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    
    public class AuthUserLoginActivity
    {
        [Key]
        [Display(Name = "Login Activity Id")]
        public int LoginActivityId { get; set; }
        [Display(Name = "Login Id")]
        public int UserLoginId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        [ForeignKey("UserLoginId")]
        public virtual AuthUserLogin AuthUserLogin { get; set; }
    }
}
