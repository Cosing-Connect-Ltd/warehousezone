using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("AccountAddresses")]
    public class AccountAddresses
    {

        [Key]
        [Display(Name = "Address Id")]
        public int AddressID { get; set; }
        [Display(Name = "Contact Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Telephone { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
        [Display(Name = "Town")]
        public string Town { get; set; }
        [Display(Name = "County")]
        public string County { get; set; }
        [DataType(DataType.PostalCode)]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }
        [Display(Name = "Country")]
        [Required]
        public int CountryID { get; set; }
        [Display(Name = "Account")]
        public int? AccountID { get; set; }
        public string SessionId { get; set; }
        [Display(Name = "Dafault")]
        public bool? AddTypeDefault { get; set; }
        [Display(Name = "Shipping")]
        public bool? AddTypeShipping { get; set; }
        [Display(Name = "Billing")]
        public bool? AddTypeBilling { get; set; }
        [Display(Name = "Marketing")]
        public bool AddTypeMarketing { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Deleted")]
        public bool? IsDeleted { get; set; }
        [Display(Name = "Prestashop Id")]
        public int? PrestaShopAddressId { get; set; }
        public virtual GlobalCountry GlobalCountry { get; set; }
        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        public string FullAddressValue
        {
            get
            {
                var fullAddress = "";
                fullAddress += string.IsNullOrEmpty(Name) ? ";" : Name;
                fullAddress += string.IsNullOrEmpty(AddressLine1) ? ";" : "; " + AddressLine1;
                fullAddress += string.IsNullOrEmpty(AddressLine2) ? ";" : "; " + AddressLine2;
                fullAddress += string.IsNullOrEmpty(AddressLine3) ? ";" : "; " + AddressLine3;
                fullAddress += string.IsNullOrEmpty(Town) ? ";" : "; " + Town;
                fullAddress += string.IsNullOrEmpty(PostCode) ? ";" : "; " + PostCode;
                return fullAddress;
            }
        }
    }
}