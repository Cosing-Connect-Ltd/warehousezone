using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    
    public class TenantProfile : PersistableEntity<int>
    {
        [Key]
        [Display(Name = " Profile Id")]
        public int TenantProfileId { get; set; }
        [MaxLength(200)]
        [Display(Name = "Name")]
        public string TenantProfileKey { get; set; }
        [Display(Name = "Value")]
        public string TenantProfileKeyValue { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("TenantId")] 
        public virtual Tenant Tenant { get; set; }
    }
}
