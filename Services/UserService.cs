using AutoMapper;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using System.Security.Claims;

namespace Fakestagram.Services
{
    public class UserService : GenericService<User, UserRegisterDTO, UserEditDTO, UserReadDTO>, IUserService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly IFollowRepository _followRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthProvider _jwtProvider;
        private readonly IPasswordProvider _hmacSha256Provider;

        public UserService(IUsersRepository repo, IMapper mapper, IPostsRepository postsRepository,
                           IFollowRepository followRepository, IHttpContextAccessor httpContextAccessor,
                           IAuthProvider authProvider, IPasswordProvider passwordProvider
                          )
                : base(repo, mapper)
        {
            _postsRepository = postsRepository;
            _followRepository = followRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtProvider = authProvider;
            _hmacSha256Provider = passwordProvider;
        }

        public void Follow(Guid userId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                User u = _repo.GetById(Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).ToString())) ?? throw new UserNotFoundException("The user is not found.");
                _followRepository.Follow(userId, u.Id);
            }
        }

        private bool IsUserNameTaken(string username)
        {
            return ((IUsersRepository)_repo).GetByUsername(username) != null;
        }

        private bool IsEmailTaken(string email)
        {
            return ((IUsersRepository)_repo).GetByEmail(email) != null;
        }

        public string RegisterNewUser(UserRegisterDTO userRegisterDTO, out User user)
        {
            bool isEmailTaken = this.IsEmailTaken(userRegisterDTO.Email);
            bool isUserNameTaken = this.IsUserNameTaken(userRegisterDTO.UserName);

            if(isEmailTaken) throw new EmailIsAlreadyTakenException("The specified Email is already taken.");
            if(isUserNameTaken) throw new UserNameIsAlreadyTakenException("The specified UserName is already taken.");

            _hmacSha256Provider.CreatePasswordHash(userRegisterDTO.Password, out string passwordHash, out string passwordSalt);

            User userToRegister = new User()
            {
                UserName = userRegisterDTO.UserName,
                Email = userRegisterDTO.Email,
                FirstName = userRegisterDTO.FirstName,
                LastName = userRegisterDTO.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            ((IUsersRepository)_repo).Create(userToRegister);

            string jwt = _jwtProvider.CreateToken(userToRegister);
            
            user = ((IUsersRepository)_repo).GetByUsername(userToRegister.UserName);

            return jwt;
        }

        public void Unfollow(Guid userId)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                User u = _repo.GetById(Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).ToString())) ?? throw new UserNotFoundException("The user is not found.");
                _followRepository.Unfollow(userId, u.Id);
            }
        }

        public string UserLogin(UserLoginDTO userLoginDTO)
        {
            User u = ((IUsersRepository)_repo).GetByUsername(userLoginDTO.UserName) ?? throw new UserNotFoundException("The username or password is incorrect.");

            string passwordHash = u.PasswordHash;
            string passwordSalt = u.PasswordSalt;

            bool doesPasswordsMatch = _hmacSha256Provider.VerifyPasswordHash(userLoginDTO.Password, passwordHash, passwordSalt);

            if (doesPasswordsMatch)
            {
                return _jwtProvider.CreateToken(u);
            }

            return null;
        }

        public UserReadDTO GetUserReadDTOByUserId(Guid userId)
        {
            User u = _repo.GetById(userId);

            if (u is null)
            {
                throw new InvalidDataException("User with the specified Id is not found.");
            }

            List<PostReadDTO> UserPosts = _postsRepository.GetAllByUserCreatorIdToReadDTO(userId);

            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FollowersCount = ((IUsersRepository)_repo).GetUserFollowers(userId).Count,
                FollowingCount = ((IUsersRepository)_repo).GetUserFollowings(userId).Count,
                Posts = UserPosts,
                PostsCount = UserPosts.Count
            };

            return userReadDTO;
        }
    }
}
