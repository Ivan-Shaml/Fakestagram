using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Posts
{
    public class PostCreateDTO
    {
        [Required]
        [FromForm(Name = "image")]
        public IFormFile Image { get; set; }

        [Required]
        [FromForm(Name = "description")]
        public string Description { get; set; } = string.Empty;
    }
}
