using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class AuthPermission : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Permission Id")]
        public int PermissionId { get; set; }
        [Display(Name = "User Id")]
        public int UserId { get; set; }
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [ForeignKey("ActivityId")]
        public virtual AuthActivity AuthActivity { get; set; }
        [ForeignKey("UserId")] 
        public virtual AuthUser AuthUser { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual TenantLocations TenantLocation { get; set; }
    }
}
