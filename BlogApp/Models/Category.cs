using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]

        public string? Description { get; set; }

        //Propertires for storing image
        public byte[]? ImageData { get; set; } //= Array.Empty<byte>();
        public string? ImageType { get; set; } //= "";

        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute
        [NotMapped]
        public virtual IFormFile? CategoryImage { get; set; }

        //Navigation Properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}
