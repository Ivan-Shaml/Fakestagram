using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class CommentLikesRepository : GenericRepository<CommentLike>, ICommentLikesRepository
    {
        public CommentLikesRepository(FakestagramDbContext dbContext) : base(dbContext)
        {
        }

        public CommentLike GetByCommentId(Guid commentId, Guid likedUserId)
        {
            return _dbSet.FirstOrDefault(x => x.CommentId == commentId && x.UserId == likedUserId);
        }

        public List<User> GetUserListLikes(Guid commentId)
        {
            return _context.CommentLikes
                .Where(c => c.CommentId == commentId)
                .Include(us => us.User)
                .Select(us => us.User)
                .ToList();
        }
    }
}
