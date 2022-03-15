using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories
{
    public class CommentsRepository : GenericRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(FakestagramDbContext dbContext) : base(dbContext)
        {
        }
    }
}
