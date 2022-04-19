using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories
{
    public class TokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public TokenRepository(FakestagramDbContext dbContext)
            : base(dbContext)
        {
        }

        public void DeleteAllChildTokens(Guid parentTokenId)
        {
            var parentToken = GetById(parentTokenId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesn't exist.");
            }

            _dbSet.RemoveRange(_dbSet.Where(t => t.ParentRefreshTokenId == parentTokenId).ToList());
        }

        public void DeleteTokenSequence(Guid parentTokenId)
        {
            var parentToken = GetById(parentTokenId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesn't exist.");
            }

            DeleteAllChildTokens(parentTokenId);
            _dbSet.Remove(parentToken);
        }

        public void DeleteAllUserTokens(Guid userId)
        {
            var parentToken = _dbSet.FirstOrDefault(u => u.UserId == userId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("This user haven't got tokens yet.");
            }

            _dbSet.RemoveRange(_dbSet.Where(t => t.UserId == userId).ToList());
        }

        public bool IsTokenAlreadyRotated(Guid tokenId)
        {
            var allTokensFromSequence = _dbSet.Where(t => t.ParentRefreshTokenId == tokenId || t.Id == tokenId).OrderByDescending(t => t.IAT).ToList();

            var tokenFromDb = _dbSet.FirstOrDefault(t => t.Id == tokenId);

            if (allTokensFromSequence.IndexOf(tokenFromDb) == 0)
            {
                return true;
            }

            return false;
        }

        public void PopTokenSequence(Guid tokenId)
        {
            var parentToken = GetById(tokenId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesn't exist.");
            }

            var firstChildTokenInSequence = _dbSet.Where(t => t.ParentRefreshTokenId == tokenId)?.OrderBy(t => t.IAT)?.First();

            if (firstChildTokenInSequence != null)
            {
                _dbSet.Remove(firstChildTokenInSequence);
            }
        }
    }
}
