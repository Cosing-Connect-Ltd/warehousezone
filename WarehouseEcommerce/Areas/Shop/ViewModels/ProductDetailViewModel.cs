using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public List<ProductMaster> productMasterList { get; set; }
        public ProductMaster ProductMaster { get; set; }

        public List<ProductFiles> ProductFilesList { get; set; }
        public ProductFiles ProductFiles { get; set; }

    }

    public class UsersViewModel
    {
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string ContactNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        public string VATNumber { get; set; }
        public string RegNumber { get; set; }

        public bool? PlaceOrder { get; set; }
    }
}