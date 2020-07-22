using System.ComponentModel.DataAnnotations;

namespace WarehouseEcommerce.Models
{
    public class ContactUs
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required, DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}