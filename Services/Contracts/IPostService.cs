using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IPostService : IGenericService<Post, PostCreateDTO, PostEditDTO, PostReadDTO>
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<PostReadDTO> SaveNewPostAsync(PostCreateDTO imgPath);
        List<Post> GetAllByUserCreatorId(Guid userId);
        Post GetByIdToModel(Guid id);
    }
}
