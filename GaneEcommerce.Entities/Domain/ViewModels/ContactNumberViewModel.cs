using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class ContactNumbersViewModel
    {
        public int? Id { get; set; }
        [DisplayName("Work Phone")]
        public string WorkNumber { get; set; }
        [DisplayName("Mobile")]
        public string MobileNumber { get; set; }
        [DisplayName("Home Phone")]
        public string HomeNumber { get; set; }

        [DisplayName("Email"), EmailAddress(ErrorMessage = "Email Address is not valid.")]
        public string EmailAddress { get; set; }
        public string Fax { get; set; }
    }
    
}