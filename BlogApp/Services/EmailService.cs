using BlogApp.Models;
using BlogApp.Services.Interfaces;
using BlogApp.Models.ViewModels;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace BlogApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendEmailAsync(BlogUser blogUser, EmailData emailData)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");

            MimeMessage newEmail = new MimeMessage();

            //add all email addresses to the "TO" for the email
            newEmail.Sender = MailboxAddress.Parse(email);
            newEmail.To.Add(MailboxAddress.Parse(emailSender));


            //add the subject for the email 
            newEmail.Subject = subject;

            //add the body for the email
            BodyBuilder emailBody = new BodyBuilder();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //send the email 
            using SmtpClient smtpClient = new SmtpClient();
            try
            {
                var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Host");
                var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port")!);
                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, _mailSettings.Password ?? Environment.GetEnvironmentVariable("Password"));

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);

            }
            catch
            {
                throw;
            }

        }
    }
}
