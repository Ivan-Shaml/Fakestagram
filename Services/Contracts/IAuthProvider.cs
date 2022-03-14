using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IAuthProvider
    {
        string CreateToken(User user);
    }
}
