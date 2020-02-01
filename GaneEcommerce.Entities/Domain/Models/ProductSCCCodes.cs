using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public partial class ProductSCCCodes
    {
        [Key]
        [Display(Name = "SCC Code Id")]
        public int ProductSCCCodeId { get; set; }
        [Display(Name = "Product")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
        [StringLength(50)]
        [Display(Name = "SCC Code")]
        public string SCC { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
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
