using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ProductAttributeValuesMap
    {
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Display(Name = "Attribute Value")]
        public int AttributeValueId { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime? DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual ProductAttributeValues ProductAttributeValues { get; set; }
    }
}
