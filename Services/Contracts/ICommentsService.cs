using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface ICommentsService : IGenericService<Comment, CommentCreateDTO, CommentEditDTO, CommentReadDTO>
    {
        IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId, PaginationParameters @params);
        IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId, PaginationParameters @params);
        IEnumerable<CommentReadDTO> GetAllCommentsToDto(PaginationParameters @params);
    }
}
