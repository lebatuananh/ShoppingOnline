using System.Threading.Tasks;

namespace ShoppingOnline.WebApplication.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}