using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        void DeleteAllChildTokens(Guid parentTokenId);
        void DeleteTokenSequence (Guid parentTokenId);
        void DeleteAllUserTokens(Guid userId);
        bool IsTokenAlreadyRotated(Guid tokenId);
        void PopTokenSequence(Guid tokenId);
    }
}
