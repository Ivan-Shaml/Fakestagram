using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class PostLikesRepository : GenericRepository<PostLike>, IPostLikesRepository
    {
        public PostLikesRepository(FakestagramDbContext dbContext) : base(dbContext)
        {
        }

        public PostLike GetByPostId(Guid postId, Guid likedUserId)
        {
            return _dbSet.FirstOrDefault(x => x.PostId == postId && x.UserId == likedUserId);
        }

        public List<User> GetUserListLikes(Guid postId)
        {
            base.GetById(postId);

            return _context.PostLikes.
                Include(us => us.User)
                .Select(us => us.User)
                .ToList();
        }
    }
}
