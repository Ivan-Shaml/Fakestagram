using Fakestagram.Models;
using System.Security.Claims;

namespace Fakestagram.Services.Contracts
{
    public interface IAuthProvider
    {
        string CreateToken(User user);
        bool VerifyToken(string token);
        List<Claim> GetClaims(string token);
        bool isUserAdmin(string token);
    }
}
