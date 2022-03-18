using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakestagram.Models
{
    public class Comment : BaseModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Text { get; set; }

        public DateTime PostedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual List<CommentLike> Likes { get; set; }
    }
}
