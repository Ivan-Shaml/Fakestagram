using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface ICommentsRepository : IGenericRepository<Comment>
    {
        IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId, int? skip, int? take);
        IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId, int? skip, int? take);
        IEnumerable<CommentReadDTO> GetAllCommentsToDto(int? skip, int? take);
        CommentReadDTO GetCommentByIdToDto(Guid commentId);
    }
}
