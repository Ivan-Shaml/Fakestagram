namespace Fakestagram.Data.DTOs.Posts
{
    public class PostReadDTO
    {
        public Guid PostId { get; set; }
        public string[] ImgUrls { get; set; }
        public string Description { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
