using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Fakestagram.Models
{
    public class RefreshToken : BaseModel
    {
        [Required]
        public string RefreshJwtToken { get; set; }
        [Required]
        public string IPAddress { get; set; } = String.Empty;
        [Required]
        public DateTime IAT { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime ExpirationDateTime { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Guid? ParentRefreshTokenId { get; set; } = null;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
