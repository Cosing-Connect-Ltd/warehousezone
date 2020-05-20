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
        [Display(Name = "Tooltip Id")]
        public int TooltipId { get; set; }
        [Display(Name = "Localization")]
        public string Localization { get; set; }
        [Display(Name = "Key")]
        [Required]
        [StringLength(512)]
        public string Key { get; set; }
        [Display(Name = "Title")]
        [StringLength(1024)]
        public string Title { get; set; }
        [Display(Name = "Description")]
        [Required]
        [StringLength(4086)]
        public string Description { get; set; }
        [Display(Name = "Client")]
        public int? TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}