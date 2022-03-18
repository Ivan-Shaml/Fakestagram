using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(FakestagramDbContext dbContext) : base(dbContext)
        {
        }

        public void Follow(Guid initiatorUserId, Guid targetUserId)
        {
            User userFollower = _context.Users.FirstOrDefault(x => x.Id == initiatorUserId) ?? throw new InvalidDataException("UserFollower not found.");
            User userFollowed = _context.Users.FirstOrDefault(x => x.Id == targetUserId) ?? throw new InvalidDataException("UserFollowed not found.");

            if (_dbSet.FirstOrDefault(x => x.UserFollowerId == initiatorUserId && x.UserFollowedId == targetUserId) == null)
            {
                var follow = new Models.Follow()
                {
                    UserFollowerId = initiatorUserId,
                    UserFollowedId = targetUserId,
                    FollowerSince = DateTime.UtcNow
                };

                this.Create(follow);
                return;
            }
            else
            {
                throw new EntityAlreadyLikedException("You already follow that user.");
            }
        }

        public int GetFollowsCount(Guid userId)
        {
            return _dbSet.Where(x => x.UserFollowedId == userId).Count();
        }

        public void Unfollow(Guid initiatorUserId, Guid targetUserId)
        {
            User userFollower = _context.Users.FirstOrDefault(x => x.Id == initiatorUserId) ?? throw new InvalidDataException("UserFollower not found.");
            User userFollowed = _context.Users.FirstOrDefault(x => x.Id == targetUserId) ?? throw new InvalidDataException("UserFollowed not found.");

            var follow = _dbSet.FirstOrDefault(x => x.UserFollowerId == initiatorUserId && x.UserFollowedId == targetUserId);

            if (follow != null)
            {
                this.Delete(follow.Id);
                return;
            }
            else
            {
                throw new EntityAlreadyDislikedException("You aren't currently following that user.");
            }
        }
    }
}
