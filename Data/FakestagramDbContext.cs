using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data
{
    public class FakestagramDbContext : DbContext
    {
        public FakestagramDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
