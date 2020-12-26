using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;

namespace Ganedata.Core.Models
{
    [Serializable]
    public class AddressViewModel
    {
        public int? Id { get; set; }
        [DisplayName("Address 1")]
        [Required]
        public string AddressLine1 { get; set; }
        [DisplayName("Address 2")]
        public string AddressLine2 { get; set; }
        [DisplayName("Address 3")]
        public string AddressLine3 { get; set; }
        public string HouseNumber { get; set; }
        [DisplayName("Post Code")]
        public string PostCode { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        [Display(Name = "Country")]
        public int CountryID { get; set; }

        public int? AccountID { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Telephone { get; set; }

        public int? AddressID { get; set; }

        public virtual CountryViewModel GlobalCountry { get; set; }

        public bool? AddTypeShipping { get; set; }

        public bool? AddTypeBilling { get; set; }        
    }
}