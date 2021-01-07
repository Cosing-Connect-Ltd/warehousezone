using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Ganedata.Core.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
        public string ReturnToken { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AccountTransactionCollectionViewModel
    {
        public AccountTransactionCollectionViewModel()
        {
            AccountTransactions = new List<AccountTransactionViewModel>();
            AllAccounts = new List<SelectListItem>();
            AllPaymentModes = new List<SelectListItem>();
        }
        public List<AccountTransactionViewModel> AccountTransactions { get; set; }
        public List<SelectListItem> AllAccounts { get; set; }
        public List<SelectListItem> AllPaymentModes { get; set; }
    }

    public class AccountTransactionViewModel
    {
        public int AccountTransactionId { get; set; }
        public int? AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal FinalBalance { get; set; }
        public string Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public AccountPaymentModeEnum? AccountPaymentModeId { get; set; }
        public string AccountPaymentMode { get; set; }
        public AccountTransactionTypeEnum AccountTransactionTypeId { get; set; }
        public string AccountTransactionType { get; set; }
        public string AccountCode { get; set; }
    }

    public class AccountStatusAuditViewModel
    {
        public int AccountStatusAuditId { get; set; }

        public int AccountId { get; set; }

        public string LastStatus { get; set; }

        public string NewStatus { get; set; }

        public string LastModified { get; set; }

        public string Reason { get; set; }

        public int? UpdatedUserId { get; set; }
        public string UserName { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    [Serializable]
    public class ProductAccountCodesViewModel : PersistableEntity<int>
    {
        [Display(Name = "Product Code Id")]
        public int ProdAccCodeID { get; set; }
        [Required]
        [Display(Name = "Account")]
        public int AccountID { get; set; }
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Product Code")]
        public string ProdAccCode { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

}
