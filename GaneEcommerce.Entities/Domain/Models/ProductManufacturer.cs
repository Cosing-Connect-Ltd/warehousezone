using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductManufacturer : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Image Path")]
        public string ImagePath { get; set; }
        public string Note { get; set; }
        [Display(Name = "Manufacturer Code")]
        public string MFGCode { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        [Display(Name = "Show In Our Brands List")]
        public bool ShowInOurBrands { get; set; }
    }
}