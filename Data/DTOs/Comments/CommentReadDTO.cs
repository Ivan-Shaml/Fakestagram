namespace Fakestagram.Data.DTOs.Comments
{
    public class CommentReadDTO
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid PostId { get; set; }
        public string Text { get; set; }
        public int LikesCount { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
