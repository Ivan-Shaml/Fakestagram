using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class UsersRepository : GenericRepository<User>, IUsersRepository
    {
        public UsersRepository(FakestagramDbContext context)
            : base(context)
        {
        }

        public User GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(x => x.Email == email);
        }

        public User GetByEmailAndPassword(string email, string password)
        {
            return _dbSet.FirstOrDefault(x => x.Email == email && x.PasswordHash == password);
        }

        public User GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(x => x.UserName == username);
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            return _dbSet.FirstOrDefault(x => x.UserName == username && x.PasswordHash == password);
        }

        public List<User> GetUserFollowers(Guid userId)
        {
            var user = base.GetById(userId) ?? throw new InvalidDataException("User with the specified Id is not found.");

            return _context.Follows
                .Include(x => x.UserFollower)
                .Where(x => x.UserFollowedId == userId)
                .Select(x => x.UserFollower).ToList();
        }

        public List<User> GetUserFollowings(Guid userId)
        {
            var user = base.GetById(userId) ?? throw new InvalidDataException("User with the specified Id is not found.");

            return _context.Follows
                .Include(x => x.UserFollowed)
                .Where(x => x.UserFollowerId == userId)
                .Select(x => x.UserFollowed).ToList();
        }
    }
}
