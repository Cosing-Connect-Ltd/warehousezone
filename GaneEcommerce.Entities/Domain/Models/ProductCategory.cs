using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductCategory : PersistableEntity<int>
    {

        [Key]
        [Display(Name = "Product Category Id")]
        public int ProductCategoryId { get; set; }


        //[StringLength(20)]
        [Remote("IsProductCategoryAvailable", "ProductCategories", AdditionalFields = "ProductCategoryId", ErrorMessage = "Product category name already exists.")]
        [Required(ErrorMessage = "Product Category Name is required")]
        [Display(Name = "Product Category")]
        public string ProductCategoryName { get; set; }
        public int SortOrder { get; set; }
        public int? ProductGroupId { get; set; }
        public virtual ProductGroups ProductGroups { get; set; }

    }
}