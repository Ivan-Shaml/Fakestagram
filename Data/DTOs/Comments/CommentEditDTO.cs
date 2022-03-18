using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Comments
{
    public class CommentEditDTO
    {
        [Required]
        public string Text { get; set; }
    }
}
