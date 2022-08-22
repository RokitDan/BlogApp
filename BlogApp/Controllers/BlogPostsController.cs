using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Services;
using BlogApp.Services.Interfaces;
using BlogApp.Extensions;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService, IBlogPostService blogPostService)
        {
            _context = context;
            _imageService = imageService;
            _blogPostService = blogPostService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            //To do: Use service

            var applicationDbContext = _context.BlogPosts.Where(b => b.IsDeleted == false).Include(b => b.Category).Include(b => b.Tags);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string searchTerm, int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            ViewData["SearchTerm"] = searchTerm;

            IPagedList<BlogPost> blogPosts = await _blogPostService.Search(searchTerm).ToPagedListAsync(page, pageSize);


            List<string> firstNameList = new();

            firstNameList.Add("Danny");
            firstNameList.Add("Vince");
            firstNameList.Add("Bethany");
            firstNameList.Add("Dorothy");

            //List<string> namesThatStartWithD = new();

            //foreach (string firstName in firstNameList)
            //{
            //    if (firstName.StartsWith("D"))
            //    {
            //        namesThatStartWithD.Add(firstName);
            //    }
            //}

            //namesThatStartWithD = firstNameList.Where(x => x.StartsWith("D")).ToList();

            return View(blogPosts);
        }




        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            BlogPost? blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["BlogPostTags"] = new MultiSelectList(_context.Tags, "Id", "Name");

            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,LastUpdated,CategoryId,Abstract,IsPublished,BlogPostImage")] BlogPost blogPost, List<int> selectedTags)
        {
            if (ModelState.IsValid)
            {
                //Dates
                blogPost.Created = DataUtility.GetPostGresDate(DateTime.Now);

                //Slug
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title has already been used!");

                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                    ViewData["BlogPostTags"] = new MultiSelectList(_context.Tags, "Id", "Name");

                    return View(blogPost);
                }

                blogPost.Slug = blogPost.Title!.Slugify();

                //Image
                if (blogPost.BlogPostImage != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                    blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                }

                try
                {
                    _context.Add(blogPost);
                    await _context.SaveChangesAsync();

                    foreach (int tagId in selectedTags)
                    {
                        Tag tag = _context.Tags.Find(tagId)!;
                        blogPost.Tags.Add(tag);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["BlogPostTags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags.Select(t => t.Id));

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Created,LastUpdated,CategoryId,Slug,Abstract,IsDeleted,IsPublished,ImageData,ImageType,BlogPostImage")] BlogPost blogPost, List<int> selectedTags)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.LastUpdated = DataUtility.GetPostGresDate(DateTime.Now);
                    _context.Update(blogPost);

                    if (blogPost.BlogPostImage != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                        blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                    }


                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title orSlug has already been used!");

                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
                        ViewData["BlogPostTags"] = new MultiSelectList(_context.Tags, "Id", "Name");

                        return View(blogPost);
                    }
                    blogPost.Slug = blogPost.Title!.Slugify();

                    _context.Update(blogPost);

                    //List<Tag> oldTags = (await _addressBookService.GetContactCategoriesAsync(contact.Id)).ToList();

                    List<int> oldTags = (await _blogPostService.GetBlogTagIdsAsync(blogPost.Id)).ToList();


                    // clear out old tags
                    foreach (int tag1 in oldTags)
                    {
                        await _blogPostService.RemoveTagFromBlogPostAsync(tag1, blogPost.Id)!;
                    }



                    //Add new tags to blog
                    foreach (int tagId in selectedTags)
                    {
                        await _blogPostService.AddTagToBlogPostAsync(tagId, blogPost.Id)!;

                    }


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);

            blogPost!.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
