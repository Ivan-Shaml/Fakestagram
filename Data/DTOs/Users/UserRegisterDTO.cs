using System.ComponentModel.DataAnnotations;

namespace Fakestagram.Data.DTOs.Users
{
    public class UserRegisterDTO
    {
        [Required]
        [MaxLength(15, ErrorMessage = "Username can't exceed 15 characters.")]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "The minimum password length is 6 symbols.")]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
