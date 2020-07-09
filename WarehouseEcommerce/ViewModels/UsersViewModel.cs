using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace WarehouseEcommerce.ViewModels
{
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
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters long and contain upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string CompanyName { get; set; }

        public string VATNumber { get; set; }
        public string RegNumber { get; set; }

        public bool? PlaceOrder { get; set; }
    }
}