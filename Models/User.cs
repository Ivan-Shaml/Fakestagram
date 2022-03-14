using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Models
{
    public class User : BaseModel
    {
        [Required]
        [MaxLength(15, ErrorMessage = "Username can't exceed 15 characters.")]
        public string UserName { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        [Required]
        public string PasswordSalt { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<PostLike> PostLikes { get; set; }
        public virtual List<CommentLike> CommentLikes { get; set; }
    }
}
