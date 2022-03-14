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
            return _dbSet.FirstOrDefault(x => x.Email == email && x.Password == password);
        }

        public User GetByUsername(string username)
        {
            return _dbSet.FirstOrDefault(x => x.UserName == username);
        }

        public User GetByUsernameAndPassword(string username, string password)
        {
            return _dbSet.FirstOrDefault(x => x.UserName == username && x.Password == password);
        }

        public List<User> GetUserFollowers(Guid userId)
        {
            User u = _dbSet.FirstOrDefault(x => x.Id == userId);

            if (u is null)
            {
                throw new InvalidDataException("User with the specified Id is not found.");
            }

            return _context.Follows
                .Include(x => x.UserFollowed)
                .Where(x => x.UserFollowedId == userId)
                .Select(x => x.UserFollowed).ToList();
        }

        public List<User> GetUserFollowings(Guid userId)
        {
            User u = _dbSet.FirstOrDefault(x => x.Id == userId);

            if (u is null)
            {
                throw new InvalidDataException("User with the specified Id is not found.");
            }

            return _context.Follows
                .Include(x => x.UserFollower)
                .Where(x => x.UserFollowerId == userId)
                .Select(x => x.UserFollower).ToList();
        }
    }
}
