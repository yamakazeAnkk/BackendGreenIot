using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace GreenIotApi.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _emailFrom = "tuongankk2003@gmail.com"; 
        private readonly string _emailPassword = "";     

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Your App Name", _emailFrom));
            email.To.Add(new MailboxAddress(to, to));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_emailFrom, _emailPassword);
                    await smtpClient.SendAsync(email);
                }
                finally
                {
                    await smtpClient.DisconnectAsync(true);
                }
            }
        }
    }
}