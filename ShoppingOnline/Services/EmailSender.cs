using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ShoppingOnline.WebApplication.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var domain = string.Empty;
            var port = string.Empty;

            if (email.Contains("@gmail.com"))
            {
                domain = _configuration["MailSettings:PrimaryDomain"];
                port = _configuration["MailSettings:PrimaryPort"];
            }
            else
            {
                domain = _configuration["MailSettings:SecondayDomain"];
                port = _configuration["MailSettings:SecondaryPort"];

            }

            SmtpClient client = new SmtpClient(domain)
            {
                UseDefaultCredentials = false,
                Port = int.Parse(port),
                EnableSsl = bool.Parse(_configuration["MailSettings:EnableSsl"]),
                Credentials = new NetworkCredential(_configuration["MailSettings:UserName"], _configuration["MailSettings:Password"])
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["MailSettings:FromEmail"], _configuration["MailSettings:FromName"]),
            };
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);
            return Task.CompletedTask;
        }
    }
}