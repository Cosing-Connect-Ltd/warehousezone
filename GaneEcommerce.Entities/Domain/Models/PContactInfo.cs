using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class PContactInfo
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Display(Name = "City/Town")]
        public string AddressLine3 { get; set; }
        [Display(Name = "City/Town")]
        public string AddressTown { get; set; }
        [Display(Name = "Post code")]
        public string AddressPostcode { get; set; }
        [Display(Name = "Home Telephone")]
        public string HomeTelephone { get; set; }
        [Display(Name = "Work Telephone 1")]
        public string WorkTelephone1 { get; set; }
        [Display(Name = "Work Telephone 2")]
        public string WorkTelephone2 { get; set; }
        [Display(Name = "Fax")]
        public string WorkTelephoneFax { get; set; }
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        public string FullAddress()
        {
            var fullAddress = "";
            fullAddress += string.IsNullOrEmpty(AddressLine1) ? "" : AddressLine1;
            fullAddress += string.IsNullOrEmpty(AddressLine2) ? "" : ", " + AddressLine2;
            fullAddress += string.IsNullOrEmpty(AddressLine3) ? "" : ", " + AddressLine3;
            fullAddress += string.IsNullOrEmpty(AddressTown) ? "" : ", " + AddressTown;
            fullAddress += string.IsNullOrEmpty(AddressPostcode) ? "" : ", " + AddressPostcode;
            return fullAddress;
        }
    }
}