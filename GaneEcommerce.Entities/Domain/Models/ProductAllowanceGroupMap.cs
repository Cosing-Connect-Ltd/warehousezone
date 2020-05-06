using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductAllowanceGroupMap : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductwebsiteMapId { get; set; }
        [Display(Name = "Attribute Value")]
        public int AllowanceGroupId { get; set; }
        [ForeignKey("ProductwebsiteMapId")]
        public virtual ProductsWebsitesMap ProductsWebsitesMap { get; set; }
        [ForeignKey("AllowanceGroupId")]
        public virtual ProductAllowanceGroup ProductAllowanceGroup { get; set; }
    }
}
