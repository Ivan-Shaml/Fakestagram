using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fakestagram.Models
{
    public class Follow : BaseModel
    {
        [Required]
        public Guid UserFollowedId { get; set; }
        [Required]
        public Guid UserFollowerId { get; set; }
        [Required]
        public DateTime FollowerSince { get; set; }

        
        [ForeignKey("UserFollowedId")]
        public virtual User UserFollowed{ get; set; }
        
        [ForeignKey("UserFollowerId")]
        public virtual User UserFollower { get; set; }
    }
}
