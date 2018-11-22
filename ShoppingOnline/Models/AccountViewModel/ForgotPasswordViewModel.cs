using System.ComponentModel.DataAnnotations;

namespace ShoppingOnline.WebApplication.Models.AccountViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}