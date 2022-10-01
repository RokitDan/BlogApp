using BlogApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogApp.Services.Interfaces
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailAsync(BlogUser blogUser, EmailData emailData);
    }
}
