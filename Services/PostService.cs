using AutoMapper;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using System.Security.Claims;
using System.Text;
using Fakestagram.Data.DTOs.Pagination;

namespace Fakestagram.Services
{
    public class PostService : GenericService<Post, PostCreateDTO, PostEditDTO, PostReadDTO>, IPostService
    {
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly string baseUrl;

        public PostService(IPostsRepository repo, IMapper mapper,
                                IImageService imageService, IWebHostEnvironment environment,
                                IConfiguration configuration, IUserService userService, IPaginationHelper paginationHelper
                            ) : base(repo, mapper, paginationHelper)
        {
            _imageService = imageService;
            _environment = environment;
            _configuration = configuration;
            _userService = userService;

            this.baseUrl = _configuration.GetSection("AppBaseUrl").Value;
        }

        private string[] DeserializeImgUrls(string imgUrlString)
        {
            string[] imgUrls = imgUrlString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < imgUrls.Length; i++)
            {
                imgUrls[i] = $"{baseUrl}{imgUrls[i]}";
            }

            return imgUrls;
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

        public PostReadDTO SaveNewPost(PostCreateDTO postCreateDTO)
        {
            Post p = new Post();

            if (postCreateDTO.Image.Count == 1)
            {
                p = new Post()
                {
                    ImgUrl = this.UploadImage(postCreateDTO.Image[0]),
                    Description = postCreateDTO.Description,
                    UserCreatorId = _userService.GetCurrentUser().Id
                };
            }
            else if (postCreateDTO.Image.Count > 1)
            {
                StringBuilder imagesUrls = new StringBuilder();
                foreach (var img in postCreateDTO.Image)
                {
                    imagesUrls.Append($"{this.UploadImage(img)};");
                }

                p = new Post()
                {
                    ImgUrl = imagesUrls.ToString(),
                    Description = postCreateDTO.Description,
                    UserCreatorId = _userService.GetCurrentUser().Id
                };
            }

            ((IPostsRepository)_repo).Save(p);

            string[] imgUrls = p.ImgUrl.Split(';');

            for (int i = 0; i < imgUrls.Length; i++)
            {
                imgUrls[i] = $"{baseUrl}{imgUrls[i]}";
            }

            return new PostReadDTO()
            {
                ImgUrls = DeserializeImgUrls(p.ImgUrl),
                PostId = p.Id,
                CommentsCount = 0,
                LikesCount = 0,
                Description = p.Description,
                PostedAt = p.CreationDate
            };
        }

        public override void Delete(Guid id)
        {
            var post = ((IPostsRepository)_repo).GetById(id);

            if (post is null)
                throw new InvalidDataException("Post with the specified Id was not found.");

            foreach (var url in post.ImgUrl.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string fullPath = Path.Combine(_environment.WebRootPath, url);

                _imageService.DeleteFile(fullPath);
            }

            ((IPostsRepository)_repo).Delete(id);
        }

        public List<Post> GetAllByUserCreatorId(Guid userId)
        {
            return ((IPostsRepository)_repo).GetAllByUserCreatorId(userId);
        }

        public override PostReadDTO GetById(Guid id)
        {
            var post = ((IPostsRepository)_repo).GetById(id);

            if (post is null)
                throw new InvalidDataException("Post with the specified Id was not found.");

            var postReadDTO = new PostReadDTO()
            {
                ImgUrls = DeserializeImgUrls(post.ImgUrl),
                PostId = post.Id,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count,
                Description = post.Description,
                PostedAt = post.CreationDate
            };

            return postReadDTO;
        }

        public Post GetByIdToModel(Guid id)
        {
            var post = ((IPostsRepository)_repo).GetById(id);

            if (post is null)
                throw new InvalidDataException("Post with the specified Id was not found.");

            return post;
        }

        public override List<PostReadDTO> GetAll(PaginationParameters @params)
        {
            int? skip = (@params?.Page - 1) * @params?.ItemsPerPage;
            int? take = @params?.ItemsPerPage;

            var allPosts = ((IPostsRepository)_repo).GetAll(skip, take);

            List<PostReadDTO> postReadDTOs = new List<PostReadDTO>();

            foreach (var post in allPosts)
            {
                PostReadDTO pDto = new PostReadDTO()
                {
                    ImgUrls = DeserializeImgUrls(post.ImgUrl),
                    PostId = post.Id,
                    CommentsCount = post.Comments.Count,
                    LikesCount = post.Likes.Count,
                    Description = post.Description,
                    PostedAt = post.CreationDate
                };

                postReadDTOs.Add(pDto);
            }

            SetPaginationHeader(@params, take);

            return postReadDTOs;
        }

        public override PostReadDTO Update(Guid entityId, PostEditDTO updateDto)
        {
            var post = ((IPostsRepository)_repo).GetById(entityId);

            if (post is null)
                throw new InvalidDataException("Post with the specified Id was not found.");

            post.Description = updateDto.Description;

            ((IPostsRepository)_repo).Update(post);

            PostReadDTO pDto = new PostReadDTO()
            {
                ImgUrls = DeserializeImgUrls(post.ImgUrl),
                PostId = post.Id,
                CommentsCount = post.Comments.Count,
                LikesCount = post.Likes.Count,
                Description = post.Description,
                PostedAt = post.CreationDate
            };

            return pDto;
        }
    }
}
