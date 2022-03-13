namespace Fakestagram.Models
{
    public class User : BaseModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<PostLike> PostLikes { get; set; }
        public virtual List<CommentLike> CommentLikes { get; set; }
    }
}
