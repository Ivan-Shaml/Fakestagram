using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class PostsRepository : GenericRepository<Post>, IPostsRepository
    {
        private readonly IConfiguration _configuration;
        private string baseUrl;

        public PostsRepository(FakestagramDbContext context, IConfiguration configuration)
            :base(context)
        {
            _configuration = configuration;

            this.baseUrl = _configuration.GetSection("AppBaseUrl").Value;
        }
        private string[] DeserializeImgUrls(string imgUrlString)
        {
            string[] imgUrls = imgUrlString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < imgUrls.Length; i++)
            {
                imgUrls[i] = $"{baseUrl}{imgUrls[i]}";
            }

            return imgUrls;
        }
        public List<Post> GetAllByUserCreatorId(Guid id)
        {
            return _dbSet.Include(comments => comments.Comments)
                    .Include(postLike => postLike.Likes)
                    .Where(x => x.UserCreatorId == id)
                    .ToList();
        }

        public List<PostReadDTO> GetAllByUserCreatorIdToReadDTO(Guid id)
        {
            List<Post> posts = _dbSet.Include(comments => comments.Comments)
                    .Include(postLike => postLike.Likes)
                    .Where(x => x.UserCreatorId == id)
                    .ToList();

            //TODO: Find a way to map the data, using Automapper...

            List<PostReadDTO> postReadDTOs = new List<PostReadDTO>(posts.Count);

            foreach (var item in posts)
            {
                postReadDTOs.Add(
                    new PostReadDTO()
                    {
                        ImgUrls = DeserializeImgUrls(item.ImgUrl),
                        PostId = item.Id,
                        CommentsCount = item.Comments.Count,
                        LikesCount = item.Likes.Count,
                        Description = item.Description,
                        PostedAt = item.CreationDate
                    }
                );
            }

            return postReadDTOs;
        }

        public Post GetByUserCreatorId(Guid id)
        {
            return _dbSet.Include(comments => comments.Comments)
                    .Include(postLike => postLike.Likes)
                    .FirstOrDefault(x => x.UserCreatorId == id);
        }

        public PostReadDTO GetByUserCreatorIdToReadDTO(Guid id)
        {
            var post = _dbSet.Include(comments => comments.Comments)
                    .Include(postLike => postLike.Likes)
                    .FirstOrDefault(x => x.UserCreatorId == id);

            var postReadDTO = new PostReadDTO()
            {
                ImgUrls = DeserializeImgUrls(post.ImgUrl),
                PostId = post.Id,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count,
                Description = post.Description,
                PostedAt = post.CreationDate
            };

            return postReadDTO;
        }

        public override Post GetById(Guid id)
        {
            var post = _context.Posts.Include(c => c.Comments).Include(l => l.Likes).FirstOrDefault(x => x.Id == id);

            if (post is null)
            {
                throw new InvalidDataException("The spectified Id is not found");
            }

            return post;
        }

        public override IEnumerable<Post> GetAll(int? skip, int? take)
        {
            skip ??= 0;
            take ??= 0;

            if (skip <= 0 && take <= 0)
            {
                return _context.Posts.Include(c => c.Comments).Include(l => l.Likes).ToList();
            }

            return _context.Posts.Include(c => c.Comments).Include(l => l.Likes)
                .OrderBy(c => c.CreationDate)
                .Skip(skip.Value)
                .Take(take.Value)
                .ToList();
        }
    }
}
