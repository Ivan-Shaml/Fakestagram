using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface ICommentsService : IGenericService<Comment, CommentCreateDTO, CommentEditDTO, CommentReadDTO>
    {
        IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId);
        IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId);
        IEnumerable<CommentReadDTO> GetAllCommentsToDto();
    }
}
