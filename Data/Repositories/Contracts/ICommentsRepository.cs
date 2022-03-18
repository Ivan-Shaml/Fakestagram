﻿using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface ICommentsRepository : IGenericRepository<Comment>
    {
        IEnumerable<CommentReadDTO> GetCommentsByPostId(Guid postId);
        IEnumerable<CommentReadDTO> GetCommentsByUserId(Guid userId);
        IEnumerable<CommentReadDTO> GetAllCommentsToDto();
        CommentReadDTO GetCommentByIdToDto(Guid commentId);
    }
}
