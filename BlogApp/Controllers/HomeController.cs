using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;
        public HomeController(ILogger<HomeController> logger,
                                ApplicationDbContext context,
                                IBlogPostService blogPostService,
                                UserManager<BlogUser> userManager)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _userManager = userManager;
        }

        public async Task<IActionResult> AuthorPage()
        {
            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(b => b.IsPublished == true).ToList();
            return View(posts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       



    }
}