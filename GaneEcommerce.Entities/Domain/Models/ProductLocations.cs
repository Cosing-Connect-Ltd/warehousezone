using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductLocations : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Location")]
        public int LocationId { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual Locations Locations { get; set; }

    }

}
