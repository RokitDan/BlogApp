using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }


        //Propertires for storing image
        public byte[]? ImageData { get; set; } //= Array.Empty<byte>();
        public string? ImageType { get; set; } //= "";

        //Property for passing file information from the form(html) to the post.
        //Also not saved in teh database via [NotMapped] attribute
        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }


        //Navigation Properties

        //blog posts
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();


        //comments
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();


        //categories
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

    }
}
