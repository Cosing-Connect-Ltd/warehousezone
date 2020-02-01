using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class ProductFiles
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "File Path")]
        public string FilePath { get; set; }
        public short SortOrder { get; set; }
        public bool DefaultImage { get; set; }
        public bool HoverImage { get; set; }

        [Display(Name = "Date Created")]
        public System.DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Is Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
    }
}
