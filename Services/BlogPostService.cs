using BlogApp.Data;
using BlogApp.Extensions;
using BlogApp.Models;
using BlogApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;





namespace BlogApp.Services
{
    public class BlogPostService : IBlogPostService
    {

        ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = title.Slugify();

                if (blogId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if (!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                    }
                }

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddTagToBlogPostAsync(int tagId, int blogPostId)
        {

            try
            {
                if (!await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    //add the tag to the database
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                }


            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId)
        {

            try
            {
                if (await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    //add the tag to the database
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Remove(tag);
                        await _context.SaveChangesAsync();
                    }
                }


            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                Tag? tag = await _context.Tags.FindAsync(tagId);

                return (await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostId))!.Tags.Contains(tag!);
            }
            catch
            {
                throw;
            }
        }


        public async Task<IEnumerable<int>> GetBlogTagIdsAsync(int blogTagId)
        {
            try
            {




                BlogPost? blogPost = await _context.BlogPosts.Include(b => b.Tags)
                                              .FirstOrDefaultAsync(b => b.Id == blogTagId)!;


                List<int> tagIds = blogPost!.Tags.Select(t => t.Id).ToList();
                return tagIds;
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<Category>> GetCategoriesAsync()
        {

            try
            {
                return await _context.Categories.Include(c => c.BlogPosts).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts
                                                         .Where(b => b.IsDeleted == false)
                                                         .Include(b => b.Comments)
                                                            .ThenInclude(c => c.Author)
                                                         .Include(b => b.Category)
                                                         .Include(b => b.Tags)
                                                         .ToListAsync();
                return blogPosts;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetPopularBlogPostAsync(int count)
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).ThenInclude(c => c.Author).Include(b => b.Tags).ToListAsync();
                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogPost>> GetRecentBlogPostAsync(int count)
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).ThenInclude(c => c.Author).Include(b => b.Tags).ToListAsync();
                return blogPosts.OrderByDescending(b => b.LastUpdated).Take(count).ToList();
            }

            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<BlogPost> Search(string searchString)
        {

            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrWhiteSpace(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();

                    var popularBlogPosts = _context.BlogPosts.Where(x => x.Comments.Count > 20).ToList();

                    blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                              b.Abstract!.ToLower().Contains(searchString) ||
                                                              b.Content!.ToLower().Contains(searchString) ||
                                                              b.Category!.Name!.ToLower().Contains(searchString) ||
                                                              b.Comments.Any(
                                                                        c => c.Body!.ToLower().Contains(searchString) ||
                                                                             c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                             c.Author.LastName!.ToLower().Contains(searchString)) ||
                                                              b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)))
                                                    .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                    .Include(b => b.Category)
                                                    .Include(b => b.Tags)
                                                    .Where(b => b.IsDeleted == false && b.IsPublished == true)
                                                    .AsNoTracking()
                                                    .OrderByDescending(b => b.Created)
                                                    .AsEnumerable();

                    return blogPosts;

                }

            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}



