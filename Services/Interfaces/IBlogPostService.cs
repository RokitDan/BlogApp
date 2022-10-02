using BlogApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogApp.Models;
using BlogApp.Data;

namespace BlogApp.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogId);
        public Task AddTagToBlogPostAsync(int tagId, int blogPostId);
        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);
        public Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId);
        public Task<IEnumerable<int>> GetBlogTagIdsAsync(int blogTagId);

        //New methods 08/12/22

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<BlogPost>> GetAllBlogPostsAsync(); // All Posts regardless of IsDeleted and/or IsPublished

        public Task<List<BlogPost>> GetPopularBlogPostAsync(int count); //Defined by the number of comments made 
        public Task<List<BlogPost>> GetRecentBlogPostAsync(int count); //Defined by date created

        //To Do: Add service method for search
        public IEnumerable<BlogPost> Search(string searchString);






    }
}
