using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("Tooltips")]
    public class Tooltip : PersistableEntity<int>
    {
        [Key]
        public int TooltipId { get; set; }
        public string Localization { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? TenantId { get; set; }
    }
}