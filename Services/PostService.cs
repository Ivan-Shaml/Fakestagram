using AutoMapper;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using System.Security.Claims;

namespace Fakestagram.Services
{
    public class PostService : GenericService<Post, PostCreateDTO, PostEditDTO, PostReadDTO>, IPostService
    {
        private readonly ISaveImageService _saveImageService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public PostService(IPostsRepository repo, IMapper mapper,
                                ISaveImageService saveImageService,IWebHostEnvironment environment,
                                IConfiguration configuration, IUserService userService
                            ) : base(repo, mapper)
        {
            _saveImageService = saveImageService;
            _environment = environment;
            _configuration = configuration;
            _userService = userService;
        }

        private bool CheckFileType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".png" or ".jpeg" or ".jpg" or ".gif": return true;
                default: return false;
            }
        }

        public string UploadImage(IFormFile file)
        {
            string fileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();

            if (!this.CheckFileType(fileExtension))
            {
                throw new InvalidDataException("File type not allowed!");
            }

            string fileName = $"{Guid.NewGuid().ToString()}{Guid.NewGuid().ToString()}{fileExtension}";

            string userDirectory = _userService.GetCurrentUser()?.Id.ToString();

            string uploadDirectoryPath = Path.Combine(_environment.WebRootPath, _configuration.GetSection("ImagesUploadRoot").Value, userDirectory);

            string filePath = _saveImageService.BuildPath(uploadDirectoryPath, fileName);

            _saveImageService.SaveFile(file, filePath);

            return $"{_configuration.GetSection("ImagesUploadRoot").Value}/{userDirectory}/{fileName}";

        }

        public PostReadDTO SaveNewPost(string filePath)
        {
            Post p = new Post()
            {
                ImgUrl = filePath,
                UserCreatorId = _userService.GetCurrentUser().Id
            };

            _repo.Save(p);

            return new PostReadDTO()
            {
                ImgUrl = p.ImgUrl,
                PostId = p.Id,
                CommentsCount = 0,
                LikesCount = 0
            };
        }

        public void DeletePost(Guid id)
        {
            _repo.Delete(id);
        }
    }
}
