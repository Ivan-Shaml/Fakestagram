using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Posts
{
    public class PostEditDTO
    {
        [Required]
        public string Description { get; set; }
    }
}
