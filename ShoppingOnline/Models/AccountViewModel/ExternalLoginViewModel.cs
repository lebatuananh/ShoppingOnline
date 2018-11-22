using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.WebApplication.Models.AccountViewModel
{
    public class ExternalLoginViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string FullName { get; set; }

        [Required] public string DOB { get; set; }

        [Required] public string PhoneNumber { get; set; }
    }
}