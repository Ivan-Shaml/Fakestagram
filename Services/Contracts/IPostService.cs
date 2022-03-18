using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IPostService : IGenericService<Post, PostCreateDTO, PostEditDTO, PostReadDTO>
    {
        string UploadImage(IFormFile file);
        PostReadDTO SaveNewPost(PostCreateDTO imgPath);
        List<Post> GetAllByUserCreatorId(Guid userId);
        Post GetByIdToModel(Guid id);
    }
}
