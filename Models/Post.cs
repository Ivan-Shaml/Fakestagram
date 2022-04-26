using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakestagram.Models
{
    public class Post : BaseModel
    {
        [Required]
        public Guid UserCreatorId { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Description { get; set; } = string.Empty;
        public virtual List<Comment> Comments { get; set; }
        public virtual List<PostLike> Likes { get; set; }

        [ForeignKey("UserCreatorId")]
        public virtual User User { get; set; }
    }
}
