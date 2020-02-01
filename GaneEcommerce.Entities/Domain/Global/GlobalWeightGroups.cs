using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class GlobalWeightGroups
    {
        public GlobalWeightGroups()
        {
            ProductMaster = new HashSet<ProductMaster>();
        }

        [Key]
        [Display(Name = "Weight Group Id")]
        public int WeightGroupId { get; set; }

        [StringLength(50)]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Weight")]
        public int Weight { get; set; }

        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
    }
}
