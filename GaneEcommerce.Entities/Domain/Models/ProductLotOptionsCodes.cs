using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductLotOptionsCodes
    {
        public ProductLotOptionsCodes()
        {
            ProductMaster = new HashSet<ProductMaster>();
        }

        [Key]
        [Display(Name = "Code Id")]
        public int LotOptionCodeId { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
    }
}
