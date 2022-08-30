namespace BlogApp.Models.ViewModels
{
    public class CreateBlogPost
    {
        public BlogPost BlogPost { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

    }
}
