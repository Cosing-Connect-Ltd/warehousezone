using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    
    public class TenantContact : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Contact Id")]
        public int TenantContactId { get; set; }
        [MaxLength(200)]
        [Display(Name = "Name")]
        public string TenantContactName { get; set; }
        [MaxLength(200)]
        [Display(Name = "Email")]
        public string TenantContactEmail { get; set; }
        [MaxLength(50)]
        [Display(Name = "Phone")]
        public string TenantContactPhone { get; set; }
        [Display(Name = "Pin Code")]
        public short? TenantContactPin { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
