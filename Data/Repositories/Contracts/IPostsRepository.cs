using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Models;

namespace Fakestagram.Data.Repositories.Contracts
{
    public interface IPostsRepository : IGenericRepository<Post>
    {
        List<Post> GetAllByUserCreatorId(Guid id);
        List<PostReadDTO> GetAllByUserCreatorIdToReadDTO(Guid id);
        Post GetByUserCreatorId(Guid id);
        PostReadDTO GetByUserCreatorIdToReadDTO(Guid id);
    }
}
