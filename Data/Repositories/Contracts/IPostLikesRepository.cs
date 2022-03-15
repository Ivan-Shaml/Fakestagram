using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IPostLikesRepository : IGenericRepository<PostLike>
    {
        PostLike GetByPostId(Guid postId, Guid likedUserId);
        List<User> GetUserListLikes(Guid postId);
    }
}
