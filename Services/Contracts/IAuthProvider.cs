using Fakestagram.Models;
using System.Security.Claims;
using Fakestagram.Data.DTOs.Tokens;

namespace Fakestagram.Services.Contracts
{
    public interface IAuthProvider
    {
        string CreateAccessToken(User user);
        Dictionary<string, string> GetClaims(ClaimsPrincipal userHttpContext);
        bool isUserAdmin(ClaimsPrincipal userHttpContext);
        string CreateRefreshToken(string accessToken);
        TokenAuthDTO IssueTokenPair(TokenAuthDTO authDto);
    }
}
