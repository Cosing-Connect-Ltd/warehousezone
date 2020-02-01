using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PProperty
    {
        public PProperty()
        {
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }
        [Key]
        public int PPropertyId { get; set; }
        [Required(ErrorMessage = "Property Code is required")]
        [Display(Name = "Property Code")]
        public string PropertyCode { get; set; }
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }
        [Display(Name = "City/Town")]
        public string AddressLine4 { get; set; }
        [Display(Name = "County")]
        public string AddressLine5 { get; set; }
        [Required(ErrorMessage = "Address Post Code is required")]
        [Display(Name = "Address Post Code")]
        public string AddressPostcode { get; set; }
        public string PropertyStatus { get; set; }

        public bool? IsDeleted { get; set; }

        public bool IsLandlordManaged { get; set; }

        public bool IsVacant { get; set; }
        public DateTime? DateAvailable { get; set; }
        public DateTime? DateAdded { get; set; }
        [Display(Name = "Property Branch")]
        public string PropertyBranch { get; set; }
        public double? TenancyMonths { get; set; }
        public int SiteId { get; set; }
        public bool SyncRequiredFlag { get; set; }
        public DateTime? LetDate { get; set; }
        [Display(Name = "Landlord Code")]
        public string CurrentLandlordCode { get; set; }
        [Display(Name = "Tenant Code")]
        public string CurrentTenantCode { get; set; }
        [ForeignKey("CurrentLandlordId")]
        public virtual PLandlord PropertyLandlord { get; set; }
        [Display(Name = "Landlord")]
        public int? CurrentLandlordId { get; set; }
        public int? CurrentPTenentId { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime DateUpdated { get; set; }
        public int? UpdatedUserId { get; set; }

        public string FullAddress
        {
            get
            {
                var fullAddress = "";
                fullAddress += string.IsNullOrEmpty(AddressLine1) ? "" : AddressLine1;
                fullAddress += string.IsNullOrEmpty(AddressLine2) ? "" : ", " + AddressLine2;
                fullAddress += string.IsNullOrEmpty(AddressLine3) ? "" : ", " + AddressLine3;
                fullAddress += string.IsNullOrEmpty(AddressLine4) ? "" : ", " + AddressLine4;
                fullAddress += string.IsNullOrEmpty(AddressLine5) ? "" : ", " + AddressLine5;
                fullAddress += string.IsNullOrEmpty(AddressPostcode) ? "" : ", " + AddressPostcode;
                return fullAddress;
            }
        }

        public string PartAddress
        {
            get
            {
                var fullAddress = "";
                fullAddress += string.IsNullOrEmpty(PropertyCode) ? "" : PropertyCode;
                fullAddress += string.IsNullOrEmpty(AddressLine1) ? "" : " - " + AddressLine1;
                fullAddress += string.IsNullOrEmpty(AddressPostcode) ? "" : " - " + AddressPostcode + "";
                return fullAddress;
            }
        }

        public string LandlordDetails
        {
            get
            {
                if (PropertyLandlord != null)
                {
                    return PropertyLandlord.LandlordFullname + ", " + PropertyLandlord.FullAddress();
                }
                return string.Empty;
            }
        }
    }
}