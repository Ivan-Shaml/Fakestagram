using Fakestagram.Data.DTOs;

namespace Fakestagram.Services.Contracts
{
    public interface ILikesService
    {
        void LikeComment(Guid targetCommentId);
        void DislikeComment(Guid targetCommentId);
        void LikePost (Guid targetPostId);
        void DislikePost(Guid targetPostId);
        List<UserListLikesDTO> GetUsersLikedPostList(Guid targetPostId);
        List<UserListLikesDTO> GetUsersLikedCommentList(Guid targetCommentId);

    }
}
