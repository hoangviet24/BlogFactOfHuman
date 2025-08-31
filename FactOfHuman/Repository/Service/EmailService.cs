using FactOfHuman.Repository.IService;
using Sprache;
using System.Net;
using System.Net.Mail;

namespace FactOfHuman.Repository.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var smtp = new SmtpClient
            {
                Host = _configuration["Smtp:Host"]!,
                Port = int.Parse(_configuration["Smtp:Port"]!),
                EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"]!),
                Credentials = new NetworkCredential(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"])
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:UserName"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtp.SendMailAsync(mailMessage);
        }
    }
}
