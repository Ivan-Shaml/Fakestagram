using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Users
{
    public class UserLoginDTO
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
