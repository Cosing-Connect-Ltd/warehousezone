using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductAttributeMap : PersistableEntity<int>
    {

        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }

        [Display(Name = "Attribute")]
        public int AttributeId { get; set; }
        [ForeignKey("AttributeId")]
        public virtual ProductAttributes ProductAttributes { get; set; }



    }

}
