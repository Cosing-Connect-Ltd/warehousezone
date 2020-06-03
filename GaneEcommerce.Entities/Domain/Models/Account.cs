using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using System.Globalization;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("Account")]
    public class Account
    {
        public Account()
        {
            AccountAddresses = new HashSet<AccountAddresses>();
            AccountContacts = new HashSet<AccountContacts>();
            Orders = new HashSet<Order>();
            ProductAccountCodes = new HashSet<ProductAccountCodes>();
            AccountTransactions = new HashSet<AccountTransaction>();
            AccountStatusAudits = new HashSet<AccountStatusAudit>();
        }

        [Key]
        //TODO: GdDisplayName attribute (below) was throwing exception in pre compiling EF views, so commented out for time being.
        // Would need to be implemented when translation are implemented.
        //[GdDisplayName("Account ID")]
        [Display(Name = "Account ID")]
        public int AccountID { get; set; }
        [Display(Name = "Account Code")]
        [Remote("IsUnique", "Account", AdditionalFields = "AccountID", ErrorMessage = "Account Code already exists.")]
        [Required]
        public string AccountCode { get; set; }
        [Display(Name = "Account Name")]
        [Required]
        public string CompanyName { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }
        [Display(Name = "Currency")]
        public int CurrencyID { get; set; }
        [Display(Name = "Account Tax Status")]
        public int TaxID { get; set; }
        [Display(Name = "Account Status")]
        public AccountStatusEnum AccountStatusID { get; set; }
        [Display(Name = "Price Group")]
        public int PriceGroupID { get; set; }
        [StringLength(50)]
        [Display(Name = "VAT No")]
        public string VATNo { get; set; }
        [StringLength(50)]
        [Display(Name = "Reg No")]
        public string RegNo { get; set; }
        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Display(Name = "Primary Email")]
        public string AccountEmail { get; set; }
        //[GdPhone]
        [Display(Name = "Phone")]
        public string Telephone { get; set; }
        //[GdPhone]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        //[GdPhone]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }
        [RegularExpression("(http(s)?://([\\w-]+\\.)|([\\w-]+\\.))+[\\w-]*(/[\\w- ./?%=]*)?", ErrorMessage = "Website address is not valid")]
        [Display(Name = "Website")]
        public string website { get; set; }
        [Display(Name = "Credit Limit")]
        public double? CreditLimit { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }

        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Client")]
        public int TenantId { get; set; }
        [Display(Name = "Customer")]
        public bool AccountTypeCustomer { get; set; }
        [Display(Name = "Supplier")]
        public bool AccountTypeSupplier { get; set; }
        [Display(Name = "End User")]
        public bool AccountTypeEndUser { get; set; }

        [Display(Name = "Account Owner")]
        public int OwnerUserId { get; set; }

        [Display(Name = "Accepted Shelf Life")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Value should be greater than or equal to 1")]
        public int? AcceptedShelfLife { get; set; }
        public virtual TenantPriceGroups TenantPriceGroups { get; set; }
        public virtual GlobalCountry GlobalCountry { get; set; }
        public virtual GlobalCurrency GlobalCurrency { get; set; }
        public virtual ICollection<AccountAddresses> AccountAddresses { get; set; }
        public virtual ICollection<AccountContacts> AccountContacts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductAccountCodes> ProductAccountCodes { get; set; }
        public virtual ICollection<AccountTransaction> AccountTransactions { get; set; }
        public virtual ICollection<AccountStatusAudit> AccountStatusAudits { get; set; }
        public virtual GlobalTax GlobalTax { get; set; }
        [ForeignKey("OwnerUserId")]
        public virtual AuthUser AccountOwner { get; set; }

        [Display(Name = "Account Name")]
        public string AccountNameCode
        {
            get { return CompanyName + " (" + AccountCode + ")"; }
        }

        public string FullAddress
        {
            get
            {
                var a = AccountAddresses.Where(x => x.IsDeleted != true).FirstOrDefault();
                if (a == null) return string.Empty;

                var fullAddress = "";
                fullAddress += string.IsNullOrEmpty(a.AddressLine1) ? "" : a.AddressLine1;
                fullAddress += string.IsNullOrEmpty(a.AddressLine2) ? "" : ", " + a.AddressLine2;
                fullAddress += string.IsNullOrEmpty(a.AddressLine3) ? "" : ", " + a.AddressLine3;
                fullAddress += string.IsNullOrEmpty(a.Town) ? "" : ", " + a.Town;
                fullAddress += string.IsNullOrEmpty(a.PostCode) ? "" : ", " + a.PostCode;
                return fullAddress;
            }
        }
        public string FullAddressWithNameHtml
        {
            get
            {
                var a = AccountAddresses.Where(x => x.IsDeleted != true).FirstOrDefault();
                if (a == null) return string.Empty;

                var fullAddress = "";
                fullAddress += string.IsNullOrEmpty(CompanyName) ? "" : CompanyName;
                fullAddress += string.IsNullOrEmpty(a.AddressLine1) ? "" : "<br/>" + a.AddressLine1;
                fullAddress += string.IsNullOrEmpty(a.AddressLine2) ? "" : "<br/>" + a.AddressLine2;
                fullAddress += string.IsNullOrEmpty(a.AddressLine3) ? "" : "<br/>" + a.AddressLine3;
                fullAddress += string.IsNullOrEmpty(a.Town) ? "" : "<br/>" + a.Town;
                fullAddress += string.IsNullOrEmpty(a.PostCode) ? "" : "<br/>" + a.PostCode;
                return fullAddress;
            }
        }

        [Display(Name = "Account Balance")]
        public decimal? FinalBalance { get; set; }
        public DateTime? DateBalanceUpdated { get; set; }
        public bool CashOnlyAccount { get; set; }
        [Display(Name = "Credit Terms (days)")]
        public short? CreditTerms { get; set; }

    }

    [Serializable]
    [Table("AccountTransaction")]
    public class AccountTransaction : PersistableEntity<int>
    {
        public int AccountTransactionId { get; set; }
        public int? AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public string Notes { get; set; }
        public decimal Amount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal FinalBalance { get; set; }
        public int? OrderId { get; set; }
        public int? OrderProcessId { get; set; }
        public int? InvoiceMasterId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Memo { get; set; }
        public string Status { get; set; }
        public string IPNData { get; set; }
        public AccountPaymentModeEnum? AccountPaymentModeId { get; set; }
        public AccountTransactionTypeEnum AccountTransactionTypeId { get; set; }
        public virtual ICollection<AccountTransactionFile> AccountTransactionFiles { get; set; }
        [ForeignKey("InvoiceMasterId")]
        public virtual InvoiceMaster InvoiceMaster { get; set; }

    }

    [Serializable]
    [Table("AccountStatusAudit")]
    public class AccountStatusAudit : PersistableEntity<int>
    {
        public int AccountStatusAuditId { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        public string Reason { get; set; }

        public AccountStatusEnum LastStatusId { get; set; }

        public AccountStatusEnum NewStatusId { get; set; }
        public AccountStatusEnum LastStatusEnum => LastStatusId == 0 ? AccountStatusEnum.InActive : LastStatusId;

        public AccountStatusEnum NewStatusEnum => NewStatusId == 0 ? AccountStatusEnum.InActive : NewStatusId;
    }

    [Serializable]
    [Table("AccountTransactionFile")]
    public class AccountTransactionFile : PersistableEntity<int>
    {
        public int AccountTransactionFileID { get; set; }

        public int AccountTransactionID { get; set; }

        [ForeignKey("AccountTransactionID")]
        public virtual AccountTransaction AccountTransaction { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public byte[] FileContent { get; set; }

        public decimal ChequeAmount { get; set; }

        public int? AccountID { get; set; }

        public int? OrderID { get; set; }
    }
}