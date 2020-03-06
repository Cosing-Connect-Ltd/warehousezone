using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductManufacturer : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Manufacturer Name")]
        public string Name { get; set; }
        [Display(Name = "Image Path")]
        public string ImagePath { get; set; }
        public string Note { get; set; }
    }
}