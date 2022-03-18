using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Comments
{
    public class CommentCreateDTO
    {
        [Required]
        public Guid PostId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
