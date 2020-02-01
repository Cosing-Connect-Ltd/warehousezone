using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductLotProcessTypeCodes
    {
        public ProductLotProcessTypeCodes()
        {
            ProductMaster = new HashSet<ProductMaster>();
        }

        [Key]
        [Display(Name = "Type Code Id")]
        public int LotProcessTypeCodeId { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public virtual ICollection<ProductMaster> ProductMaster { get; set; }
    }
}
