using AutoMapper;
using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;

namespace Fakestagram.Services
{
    public class CommentsService : GenericService<Comment, CommentCreateDTO, CommentEditDTO, CommentReadDTO>, ICommentsService
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public CommentsService(ICommentsRepository repo, IMapper mapper, IUserService userService, IPostService postService)
            : base(repo, mapper)
        {
            _userService = userService;
            _postService = postService;
        }

        public IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId)
        {
            return ((ICommentsRepository) _repo).GetCommentsByPostId(postId);
        }

        public IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId)
        {
            return ((ICommentsRepository)_repo).GetCommentsByUserId(userId);
        }

        public override CommentReadDTO Insert(CommentCreateDTO createDto)
        {
            var currentUser = _userService.GetCurrentUser();
            var post = _postService.GetById(createDto.PostId);

            if (post is null)
                throw new InvalidDataException("Post with the specified Id was not found.");

            var comment = new Comment()
            {
                PostId = createDto.PostId,
                UserId = currentUser.Id,
                PostedAt = DateTime.UtcNow,
                Text = createDto.Text
            };

            ((ICommentsRepository)_repo).Create(comment);

            CommentReadDTO commentReadDTO = new CommentReadDTO()
            {
                CommentId = comment.Id,
                PostId = createDto.PostId,
                PostedAt = comment.PostedAt,
                Text = comment.Text,
                LikesCount = 0,
                UserId = currentUser.Id,
                UserName = currentUser.UserName
            };

            return commentReadDTO;
        }

        public override CommentReadDTO GetById(Guid id)
        {

            return ((ICommentsRepository)_repo).GetCommentByIdToDto(id);
        }

        public override CommentReadDTO Update(Guid entityId, CommentEditDTO updateDto)
        {
            var comment = ((ICommentsRepository)_repo).GetById(entityId);

            if (comment is null)
                throw new InvalidDataException("Comment with the specified Id was not found.");

            comment.Text = updateDto.Text;
            
            _repo.Update(comment);

            return ((ICommentsRepository)_repo).GetCommentByIdToDto(entityId);
        }

        public IEnumerable<CommentReadDTO> GetAllCommentsToDto()
        {
            return ((ICommentsRepository)_repo).GetAllCommentsToDto();
        }
    }
}
