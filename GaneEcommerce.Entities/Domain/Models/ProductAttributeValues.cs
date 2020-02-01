using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductAttributeValues:PersistableEntity<int>
    {
        public ProductAttributeValues()
        {
            ProductAttributeValuesMap = new HashSet<ProductAttributeValuesMap>();
        }
        [Key]
        [Display(Name = "Value Id")]
        public int AttributeValueId { get; set; }
        [Display(Name = "Attribute")]
        public int AttributeId { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Value")]
        public string Value { get; set; }
        public virtual ProductAttributes ProductAttributes { get; set; }
        public virtual ICollection<ProductAttributeValuesMap> ProductAttributeValuesMap { get; set; }
    }
}
