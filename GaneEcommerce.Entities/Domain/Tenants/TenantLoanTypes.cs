namespace Ganedata.Core.Entities.Domain
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // This table will hold different loan types. eg. short term loan, long term loan and loan period will be defined in days. 
    public partial class TenantLoanTypes : PersistableEntity<int>
    {
        [Key]
        [Display(Name = "Loan Id")]
        public int LoanID { get; set; }
        [Display(Name = "Loan Name")]
        public string LoanName { get; set; }
        [Display(Name = "Loan Detail")]
        public string LoanDescription { get; set; }
        [Required]
        // Loan period is in days 
        [Display(Name = "Loan Period")]
        public int LoanDays { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
    }
}
