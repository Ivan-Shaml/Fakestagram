using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakestagram.Models
{
    public class CommentLike : BaseModel
    {
        [Required]
        public Guid CommentId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
