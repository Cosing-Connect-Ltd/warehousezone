using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("UISettings")]
    public class UISetting : PersistableEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int? SiteId { get; set; }
        public virtual TenantWebsites Site { get; set; }
        [Required]
        public int UISettingItemId { get; set; }
        public virtual UISettingItem UISettingItem { get; set; }
        [Required]
        [MaxLength(32)]
        public string Value { get; set; }
    }
}