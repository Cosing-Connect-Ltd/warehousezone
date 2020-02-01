using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    public class GlobalUOMTypes
    {
        public GlobalUOMTypes()
        {
            GlobalUOM = new HashSet<GlobalUOM>();
        }

        [Key]
        [Display(Name = "UOM Type Id")]
        public int UOMTypeId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "UOM Type")]
        public string UOMType { get; set; }

        public virtual ICollection<GlobalUOM> GlobalUOM { get; set; }
    }
}
