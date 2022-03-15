using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface ICommentLikesRepository : IGenericRepository<CommentLike>
    {
        CommentLike GetByCommentId(Guid commentId, Guid likedUserId);
        List<User> GetUserListLikes(Guid commentId);
    }
}
