using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class PostsRepository : GenericRepository<Post>, IPostsRepository
    {
        public PostsRepository(FakestagramDbContext context)
            :base(context)
        {
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
                        ImgUrl = item.ImgUrl,
                        PostId = item.Id,
                        CommentsCount = item.Comments.Count,
                        LikesCount = item.Likes.Count,
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
                ImgUrl = post.ImgUrl,
                PostId = post.Id,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count,
            };

            return postReadDTO;
        }
    }
}
