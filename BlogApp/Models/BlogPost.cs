using BlogApp.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogPost
    {
        //Primary key
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        //A Foreign Key
        public int CategoryId { get; set; }


        public string? Slug { get; set; }

        public string? Abstract { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Published")]
        public bool IsPublished { get; set; }

        //Propertires for storing image
        public byte[]? ImageData { get; set; } //= Array.Empty<byte>();
        public string? ImageType { get; set; } //= "";

        //Property for passing file information from the form(html) to the post.
        //Also not saved in teh database via [NotMapped] attribute
        [NotMapped]
        public virtual IFormFile? BlogPostImage { get; set; }

        private Comment _newComment = new();
        [NotMapped]
        public Comment NewComment
        {
            get => _newComment;
            set
            {
                _newComment = value;
                _newComment.BlogPostId = Id;
            }
        }


        //Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();



    }
}
