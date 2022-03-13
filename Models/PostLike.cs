using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakestagram.Models
{
    public class PostLike : BaseModel
    {
        [Required]
        public Guid PostId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
