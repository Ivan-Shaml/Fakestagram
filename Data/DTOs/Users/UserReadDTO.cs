using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Models;

namespace Fakestagram.Data.DTOs.Users
{
    public class UserReadDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostsCount { get; set; }
        public List<PostReadDTO> Posts { get; set; }
    }
}
