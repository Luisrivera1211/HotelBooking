using System.Net;
using System.Net.Mail;
using HotelBooking.Business.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HotelBooking.Business.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            try
            {
                var host = _configuration["Email:Host"];
                var user = _configuration["Email:User"];
                var password = _configuration["Email:Password"];
                var from = _configuration["Email:From"] ?? user;

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                    return false;

                var port = int.TryParse(_configuration["Email:Port"], out var p) ? p : 587;
                var enableSsl = !bool.TryParse(_configuration["Email:EnableSsl"], out var ssl) || ssl;

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(user, password)
                };
                using var message = new MailMessage(from, to, subject, body);

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
