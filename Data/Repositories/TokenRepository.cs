using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

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
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesnt exist.");
            }

            _dbSet.RemoveRange(_dbSet.Where(t => t.ParentRefreshTokenId == parentTokenId).ToList());
            
            _context.SaveChanges();
        }

        public void DeleteTokenSequence(Guid parentTokenId)
        {
            var parentToken = GetById(parentTokenId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesnt exist.");
            }

            DeleteAllChildTokens(parentTokenId);
            
            _dbSet.Remove(parentToken);

            _context.SaveChanges();
        }

        public void DeleteAllUserTokens(Guid userId)
        {
            var parentToken = _dbSet.FirstOrDefault(u => u.UserId == userId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("This user havent got tokens yet.");
            }

            _dbSet.RemoveRange(_dbSet.Where(t => t.UserId == userId).ToList());

            _context.SaveChanges();
        }

        public bool IsTokenAlreadyRotated(Guid refreshTokenId)
        {
            var latestToken = _dbSet.Where(t => t.ParentRefreshTokenId == refreshTokenId || t.Id == refreshTokenId).OrderByDescending(t => t.IAT).First();

            if (latestToken.Id != refreshTokenId)
            {
                return true;
            }

            return false;
        }

        public void PopTokenSequence(Guid refreshTokenId, int threshold)
        {
            var parentToken = GetById(refreshTokenId);

            if (parentToken == null)
            {
                throw new RefreshTokenNotFoundException("Refresh token with the specified Id doesnt exist.");
            }

            var firstChildTokenInSequence = _dbSet.Where(t => t.ParentRefreshTokenId == refreshTokenId)?.OrderBy(t => t.IAT)?.First();

            int sequenceCount = _dbSet.Count(t => t.ParentRefreshTokenId == refreshTokenId || t.Id == refreshTokenId);

            if (firstChildTokenInSequence != null && sequenceCount >= threshold)
            {
                _dbSet.Remove(firstChildTokenInSequence);

                _context.SaveChanges();
            }
        }

        public bool DoesTokenExist(string token)
        {
            return _dbSet.Any(t => t.RefreshJwtToken == token);
        }

        public RefreshToken GetToken(string token)
        {
            var tokenFromDb = _context.RefreshTokens.Where(t => t.RefreshJwtToken == token)
                                .Include(t => t.User)
                                .FirstOrDefault();

            if (tokenFromDb == null)
            {
                throw new RefreshTokenNotFoundException("The refresh token doesnt exist.");
            }

            return tokenFromDb;
        }
    }
}
