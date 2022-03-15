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
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private string baseUrl;

        public PostService(IPostsRepository repo, IMapper mapper,
                                IImageService imageService,IWebHostEnvironment environment,
                                IConfiguration configuration, IUserService userService
                            ) : base(repo, mapper)
        {
            _imageService = imageService;
            _environment = environment;
            _configuration = configuration;
            _userService = userService;

            this.baseUrl = _configuration.GetSection("AppBaseUrl").Value;
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

            string filePath = _imageService.BuildPath(uploadDirectoryPath, fileName);

            _imageService.SaveFile(file, filePath);

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
                ImgUrl = $"{baseUrl}{p.ImgUrl}",
                PostId = p.Id,
                CommentsCount = 0,
                LikesCount = 0
            };
        }

        public override void Delete(Guid id)
        {
            var post = _repo.GetById(id);

            string fullPath = Path.Combine(_environment.WebRootPath, post.ImgUrl);

            _imageService.DeleteFile(fullPath);

            _repo.Delete(id);
        }

        public List<Post> GetAllByUserCreatorId(Guid userId)
        {
            return ((IPostsRepository)_repo).GetAllByUserCreatorId(userId);
        }

        public override PostReadDTO GetById(Guid id)
        {
            var post = _repo.GetById(id);

            var postReadDTO = new PostReadDTO()
            {
                ImgUrl = $"{baseUrl}{post.ImgUrl}",
                PostId = post.Id,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count
            };

            return postReadDTO;
        }

        public override List<PostReadDTO> GetAll()
        {
            var allPosts = ((IPostsRepository)_repo).GetAll();

            List<PostReadDTO> postReadDTOs = new List<PostReadDTO>();

            foreach (var post in allPosts)
            {
                PostReadDTO pDto = new PostReadDTO()
                {
                    ImgUrl = $"{baseUrl}{post.ImgUrl}",
                    PostId = post.Id,
                    CommentsCount = post.Comments.Count,
                    LikesCount = post.Likes.Count
                };

                postReadDTOs.Add(pDto);
            }

            return postReadDTOs;
        }
    }
}
