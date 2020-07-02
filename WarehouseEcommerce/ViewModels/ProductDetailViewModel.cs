﻿using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseEcommerce.ViewModels
{
    public class ProductDetailViewModel
    {
        public List<ProductMaster> productMasterList { get; set; }
        public ProductMaster ProductMaster { get; set; }

        public List<ProductFiles> ProductFilesList { get; set; }
        public ProductFiles ProductFiles { get; set; }

        public string CurrencySign { get; set; }
    }

    public class UsersViewModel
    {
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string ContactNumber { get; set; }

        [Remote("IsUserAvailableForSite", "User", ErrorMessage = "User already exists. ")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required, MinLength(8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        public string VATNumber { get; set; }
        public string RegNumber { get; set; }

        public bool? PlaceOrder { get; set; }
    }

    public class AccountDetailViewModel
    {
        public AuthUser AuthUser { get; set; }

        public List<Order> OrderHistory { get; set; }

        public List<WebsiteWishListItem> WebsiteWishList { get; set; }
    }
}