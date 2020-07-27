using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TenantModules
    {
        [Key]
        public int Id { get; set; }
        public TenantModuleEnum ModuleId { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}