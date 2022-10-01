using Microsoft.AspNetCore.Mvc;
using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Services;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;


namespace BlogApp.Controllers
{
    public class EmailDataController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IEmailService _emailService;
        public EmailDataController(ILogger<HomeController> logger,
                                ApplicationDbContext context,
                                IBlogPostService blogPostService,
                                UserManager<BlogUser> userManager,
                                IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            EmailData emailData = new();
            emailData.UserEmail = (await _userManager.GetUserAsync(User))?.Email;

            return View(emailData);
        }



        //POST
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendEmailAdmin(EmailData emailData)
        {
            BlogUser blogUser = await _userManager.GetUserAsync(User);

            //get user email address

            ModelState.Remove("BlogUserId");
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(blogUser.Email, emailData.EmailSubject, emailData.EmailMessage);
                    return RedirectToAction("AuthorPage", "Home", new { swalMessage = "Success: Email Sent!" });
                }
                catch
                {
                    return RedirectToAction("Index", "EmailData", new { swalMessage = "Error, Email Not Sent!" });
                    throw;
                }
            }
            return View(emailData);

        }
    }
}































//    [Authorize]
//    public async Task<IActionResult> SendEmail([Bind("UserEmail, EmailSubject, EmailMessage")] EmailData emailData)
//    {


//        //Logged in users can send emails to admin
//        //1) Check if user is logged in: 
//        //  True: "your email address" = email address of logged in user
//        //  False: Take user to login page

//        //create email service - refer to address book
//        //

//        string blogUserId = _userManager.GetUserId(User);
//        Contact contact = await _context.Contact.Where(c => c.Id == id && c.AppUserId == appUserId)
//                                                .FirstOrDefaultAsync();

//        if (contact == null)
//        {
//            return NotFound();
//        }


//        emailData.BlogUserId = _userManager.GetUserId(User),


//        ;



//        return View(model);





//    }

//}
//}

