using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Models
{
    public class BaseModel
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
    }
}
