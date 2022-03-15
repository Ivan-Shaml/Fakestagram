using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Posts
{
    public class PostUpdateDTO
    {
        [Required]
        public string Description { get; set; }
    }
}
