using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("GlobalUOM")]
    public class GlobalUOM
    {
        public GlobalUOM()
        {
            ProductMaster = new HashSet<ProductMaster>();
        }

        [Key]
        [Display(Name = "UOM Id")]
        public int UOMId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "UOM")]
        public string UOM { get; set; }
        [Display(Name = "UOM Type")]
        public int UOMTypeId { get; set; }
        public virtual GlobalUOMTypes GlobalUOM_Types { get; set; }
        public virtual ICollection<Locations> Locations { get; set; }
        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
    }
}
