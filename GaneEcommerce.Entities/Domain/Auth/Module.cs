using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class Module
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
    }

    [Serializable]
    public class TenantModules
    {
        [Key]
        public int Id { get; set; }

        public int ModuleId { get; set; }

        public int TenantId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}