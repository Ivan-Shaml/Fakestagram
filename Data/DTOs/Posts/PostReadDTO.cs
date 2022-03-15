namespace Fakestagram.Data.DTOs.Posts
{
    public class PostReadDTO
    {
        public Guid PostId { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
    }
}
