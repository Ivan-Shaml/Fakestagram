using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Microsoft.EntityFrameworkCore;

namespace Fakestagram.Data.Repositories
{
    public class CommentsRepository : GenericRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(FakestagramDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<CommentReadDTO> GetAllCommentsToDto(int? skip, int? take)
        {
            skip ??= 0;
            take ??= 0;

            IEnumerable<dynamic> comments = new List<dynamic>();

            if (skip <= 0 && take <= 0)
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Include(u => u.User)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    }).ToList();
            }
            else
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Skip(skip.Value)
                    .Take(take.Value)
                    .Include(u => u.User)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    }).ToList();
            }

            List<CommentReadDTO> commentReadDtos = MapDynamicToCommentReadDto(comments);

            return commentReadDtos;
        }

        private List<CommentReadDTO> MapDynamicToCommentReadDto(IEnumerable<dynamic> comments)
        {
            List<CommentReadDTO> commentReadDtos = new List<CommentReadDTO>(comments.Count());

            foreach (var c in comments)
            {
                commentReadDtos.Add(new CommentReadDTO()
                {
                    PostId = c.PostId,
                    Text = c.Text,
                    UserId = c.UserId,
                    UserName = c.UserName,
                    PostedAt = c.PostedAt,
                    LikesCount = c.LikesCount,
                    CommentId = c.CommentId
                });
            }

            return commentReadDtos;
        }

        public CommentReadDTO GetCommentByIdToDto(Guid commentId)
        {
            if (GetById(commentId) is null)
                throw new InvalidDataException("Comment with the specified Id was not found.");

            var comment = _context.Comments
             .Include(u => u.User)
             .Where(c => c.Id == commentId)
             .Select(comment => new
             {
                 PostId = comment.PostId,
                 CommentId = comment.Id,
                 UserId = comment.UserId,
                 UserName = comment.User.UserName,
                 Text = comment.Text,
                 PostedAt = comment.PostedAt,
                 LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
             }).First();

            return new CommentReadDTO()
            {
                PostedAt = comment.PostedAt,
                CommentId = comment.CommentId,
                LikesCount = comment.LikesCount,
                PostId = comment.PostId,
                Text = comment.Text,
                UserId = comment.UserId,
                UserName = comment.UserName
            };
        }

        public IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId, int? skip, int? take)
        {
            if (_context.Posts.FirstOrDefault(x => x.Id == postId) is null)
                throw new PostNotFoundException("Post with the specified Id was not found.");

            skip ??= 0;
            take ??= 0;

            IEnumerable<dynamic> comments = new List<dynamic>();

            if (skip <= 0 && take <= 0)
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Include(u => u.User)
                    .Where(c => c.PostId == postId)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    })
                    .ToList();

            }
            else
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Skip(skip.Value)
                    .Take(take.Value)
                    .Include(u => u.User)
                    .Where(c => c.PostId == postId)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    })
                    .ToList();
            }

            List<CommentReadDTO> commentReadDtos = MapDynamicToCommentReadDto(comments);

            return commentReadDtos;
        }

        public IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId, int? skip, int? take)
        {
            if (_context.Users.FirstOrDefault(x => x.Id == userId) is null)
                throw new UserNotFoundException("User with the specified Id was not found.");

            skip ??= 0;
            take ??= 0;

            IEnumerable<dynamic> comments = new List<dynamic>();

            if (skip <= 0 && take <= 0)
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Include(u => u.User)
                    .Where(c => c.UserId == userId)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    })
                    .ToList();
            }
            else
            {
                comments = _context.Comments
                    .OrderBy(x => x.PostedAt)
                    .Skip(skip.Value)
                    .Take(take.Value)
                    .Include(u => u.User)
                    .Where(c => c.UserId == userId)
                    .Select(comment => new
                    {
                        PostId = comment.PostId,
                        CommentId = comment.Id,
                        UserId = comment.UserId,
                        UserName = comment.User.UserName,
                        Text = comment.Text,
                        PostedAt = comment.PostedAt,
                        LikesCount = _context.CommentLikes.Where(lc => lc.CommentId == comment.Id).Count()
                    })
                    .ToList();
            }

            List<CommentReadDTO> commentReadDtos = MapDynamicToCommentReadDto(comments);

            return commentReadDtos;
        }

    }
}
