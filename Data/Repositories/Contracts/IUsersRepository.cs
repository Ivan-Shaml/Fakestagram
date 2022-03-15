using Fakestagram.Data.DTOs.Users;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IUsersRepository : IPostsRepository<User>
    {
        User GetByUsernameAndPassword(string username, string password);
        User GetByEmailAndPassword(string email, string password);
        User GetByEmail(string email);
        User GetByUsername(string username);
        List<User> GetUserFollowers(Guid userId);
        List<User> GetUserFollowings(Guid userId);

    }
}
