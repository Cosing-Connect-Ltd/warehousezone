using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductTagMap : PersistableEntity<int>
    {

        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("ProductId")]
        public virtual ProductMaster ProductMaster { get; set; }

        [ForeignKey("TagId")]
        public virtual ProductTag ProductTag { get; set; }


    }

}
