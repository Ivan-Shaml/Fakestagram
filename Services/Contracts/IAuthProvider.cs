using Fakestagram.Models;
using System.Security.Claims;

namespace Fakestagram.Services.Contracts
{
    public interface IAuthProvider
    {
        string CreateToken(User user);
        Dictionary<string, string> GetClaims(ClaimsPrincipal userHttpContext);
        bool isUserAdmin(ClaimsPrincipal userHttpContext);
    }
}
