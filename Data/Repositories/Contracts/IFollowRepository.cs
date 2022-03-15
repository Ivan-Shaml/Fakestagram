using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IFollowRepository : IPostsRepository<Follow>
    {
        void Follow(Guid initiatorUserId, Guid targetUserId);
        void Unfollow(Guid initiatorUserId, Guid targetUserId);
        int GetFollowsCount(Guid userId);
    }
}
