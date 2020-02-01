using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("AccountContacts")]
    public class AccountContacts
    {
        [Key]
        [Display(Name = "Contact Id")]
        public int AccountContactId { get; set; }
        [Display(Name = "Account")]
        public int AccountID { get; set; }
        [Required]
        [MaxLength(200)]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [MaxLength(200)]
        [Display(Name = "Contact Job Title")]
        public string ContactJobTitle { get; set; }
        [MaxLength(200)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }
        //[GdPhone]
        [Display(Name = "Contact Phone")]
        public string TenantContactPhone { get; set; }
        // following is a secret pin code to authenticate the person on phone.
        [Display(Name = "Security Pin")]
        public short? TenantContactPin { get; set; }
        [Display(Name = "Remittances")]
        public Boolean ConTypeRemittance { get; set; }
        [Display(Name = "Statements")]
        public Boolean ConTypeStatment { get; set; }
        [Display(Name = "Invoices")]
        public Boolean ConTypeInvoices { get; set; }
        [Display(Name = "Marketing")]
        public Boolean ConTypeMarketing { get; set; }

        [Display(Name = "Purchasing")]
        public Boolean ConTypePurchasing { get; set; }
        [Display(Name = "Date Created")]
        public System.DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public System.DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
    }
}
