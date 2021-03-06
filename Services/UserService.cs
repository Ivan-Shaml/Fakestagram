using AutoMapper;
using Fakestagram.Data.DTOs;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using System.Security.Claims;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Data.DTOs.Tokens;

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
                           IAuthProvider authProvider, IPasswordProvider passwordProvider,
                           IPaginationHelper paginationHelper
                          )
                : base(repo, mapper, paginationHelper)
        {
            _postsRepository = postsRepository;
            _followRepository = followRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtProvider = authProvider;
            _hmacSha256Provider = passwordProvider;
        }

        public TokenAuthDTO RefreshToken(TokenAuthDTO authDto)
        {
            return _jwtProvider.Refresh(authDto);
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            User currentUser = GetCurrentUser();
            _jwtProvider.RevokeRefreshToken(refreshToken, currentUser);
        }

        public void Follow(Guid userId)
        {
            Dictionary<string, string> claims = _jwtProvider.GetClaims(_httpContextAccessor.HttpContext.User);

            User u = ((IUsersRepository)_repo).GetById(Guid.Parse(claims["userId"])) ?? throw new UserNotFoundException("The user is not found.");
            _followRepository.Follow(u.Id, userId);
        }

        private bool IsUserNameTaken(string username)
        {
            return ((IUsersRepository)_repo).GetByUsername(username) != null;
        }

        private bool IsEmailTaken(string email)
        {
            return ((IUsersRepository)_repo).GetByEmail(email) != null;
        }

        public TokenAuthDTO RegisterNewUser(UserRegisterDTO userRegisterDTO, out User user)
        {
            bool isEmailTaken = this.IsEmailTaken(userRegisterDTO.Email);
            bool isUserNameTaken = this.IsUserNameTaken(userRegisterDTO.UserName);

            if (isEmailTaken) throw new EmailIsAlreadyTakenException("The specified Email is already taken.");
            if (isUserNameTaken) throw new UserNameIsAlreadyTakenException("The specified UserName is already taken.");

            _hmacSha256Provider.CreatePasswordHash(userRegisterDTO.Password, out string passwordHash, out string passwordSalt);

            User userToRegister = new User()
            {
                UserName = userRegisterDTO.UserName,
                Email = userRegisterDTO.Email,
                FirstName = userRegisterDTO.FirstName,
                LastName = userRegisterDTO.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserRoles.Regular,
            };

            ((IUsersRepository)_repo).Create(userToRegister);

            user = ((IUsersRepository)_repo).GetByUsername(userToRegister.UserName);

            return _jwtProvider.IssueNewTokenPair(userToRegister);
        }

        public void Unfollow(Guid userId)
        {
            Dictionary<string, string> claims = _jwtProvider.GetClaims(_httpContextAccessor.HttpContext.User);

            User u = ((IUsersRepository)_repo).GetById(Guid.Parse(claims["userId"])) ?? throw new UserNotFoundException("The user is not found.");
            _followRepository.Unfollow(u.Id, userId);
        }

        public TokenAuthDTO UserLogin(UserLoginDTO userLoginDTO)
        {
            User u = ((IUsersRepository)_repo).GetByUsername(userLoginDTO.UserName) ?? throw new UserNotFoundException("The username or password is incorrect.");

            string passwordHash = u.PasswordHash;
            string passwordSalt = u.PasswordSalt;

            bool doesPasswordsMatch = _hmacSha256Provider.VerifyPasswordHash(userLoginDTO.Password, passwordHash, passwordSalt);

            if (doesPasswordsMatch)
            {
                return _jwtProvider.IssueNewTokenPair(u);
            }

            throw new UserNotFoundException("The username or password is incorrect.");
        }
        public override UserReadDTO GetById(Guid id)
        {
            User u = ((IUsersRepository)_repo).GetById(id);

            if (u is null)
            {
                throw new UserNotFoundException("User with the specified Id is not found.");
            }

            List<PostReadDTO> UserPosts = _postsRepository.GetAllByUserCreatorIdToReadDTO(id);

            UserReadDTO userReadDTO = new UserReadDTO
            {
                UserId = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FollowersCount = ((IUsersRepository)_repo).GetUserFollowers(id).Count,
                FollowingCount = ((IUsersRepository)_repo).GetUserFollowings(id).Count,
                Posts = UserPosts,
                PostsCount = UserPosts.Count
            };

            return userReadDTO;
        }

        public User GetCurrentUser()
        {
            Dictionary<string, string> claims = _jwtProvider.GetClaims(_httpContextAccessor.HttpContext.User);

            return ((IUsersRepository)_repo).GetById(Guid.Parse(claims["userId"])) ?? throw new UserNotFoundException("The user is not found.");
        }

        public override List<UserReadDTO> GetAll(PaginationParameters @params)
        {
            int? skip = (@params?.Page - 1) * @params?.ItemsPerPage;
            int? take = @params?.ItemsPerPage;

            IEnumerable<User> allUsers = ((IUsersRepository)_repo).GetAll(skip, take);

            List<UserReadDTO> allUsersToDto = new List<UserReadDTO>();

            foreach (var usr in allUsers)
            {
                List<PostReadDTO> userPosts = _postsRepository.GetAllByUserCreatorIdToReadDTO(usr.Id);

                UserReadDTO userReadDTO = new UserReadDTO
                {
                    UserId = usr.Id,
                    UserName = usr.UserName,
                    FirstName = usr.FirstName,
                    LastName = usr.LastName,
                    FollowersCount = ((IUsersRepository)_repo).GetUserFollowers(usr.Id).Count,
                    FollowingCount = ((IUsersRepository)_repo).GetUserFollowings(usr.Id).Count,
                    Posts = userPosts,
                    PostsCount = userPosts.Count
                };

                allUsersToDto.Add(userReadDTO);
            }

            SetPaginationHeader(@params, take);

            return allUsersToDto;
        }

        public List<UserListFollowsDTO> GetUserFollowers(Guid userId)
        {
            var users = ((IUsersRepository)_repo).GetUserFollowers(userId);

            return _mapper.Map<List<UserListFollowsDTO>>(users);
        }

        public List<UserListFollowsDTO> GetUserFollowings(Guid userId)
        {
            var users = ((IUsersRepository)_repo).GetUserFollowings(userId);

            return _mapper.Map<List<UserListFollowsDTO>>(users);
        }

        public bool IsCurrentUserAdmin()
        {
            return _jwtProvider.isUserAdmin(_httpContextAccessor.HttpContext.User);
        }

        public void ChangeUserRole(Guid userId)
        {
            var user = ((IUsersRepository)_repo).GetById(userId) ??
                       throw new UserNotFoundException("User with the specified Id is not found.");

            if (user.Role == UserRoles.Administrator)
            {
                user.Role = UserRoles.Regular;
            }
            else if (user.Role == UserRoles.Regular)
            {
                user.Role = UserRoles.Administrator;
            }

            ((IUsersRepository)_repo).Update(user);
        }

        public override UserReadDTO Update(Guid entityId, UserEditDTO updateDto)
        {
            var userFromDb = ((IUsersRepository)_repo).GetById(entityId) ??
                             throw new UserNotFoundException("User with the specified Id is not found.");

            if (userFromDb.Email != updateDto.Email && this.IsEmailTaken(updateDto.Email))
            {
                throw new EmailIsAlreadyTakenException("The specified Email is already taken.");
            }

            if (userFromDb.UserName != updateDto.UserName && this.IsUserNameTaken(updateDto.UserName))
            {
                throw new UserNameIsAlreadyTakenException("The specified UserName is already taken.");
            }

            userFromDb.Email = updateDto.Email;
            userFromDb.UserName = updateDto.UserName;
            userFromDb.FirstName = updateDto.FirstName;
            userFromDb.LastName = updateDto.LastName;

            ((IUsersRepository)_repo).Update(userFromDb);

            List<PostReadDTO> userPosts = _postsRepository.GetAllByUserCreatorIdToReadDTO(userFromDb.Id);

            return new UserReadDTO
            {
                UserId = userFromDb.Id,
                UserName = userFromDb.UserName,
                FirstName = userFromDb.FirstName,
                LastName = userFromDb.LastName,
                FollowersCount = ((IUsersRepository)_repo).GetUserFollowers(userFromDb.Id).Count,
                FollowingCount = ((IUsersRepository)_repo).GetUserFollowings(userFromDb.Id).Count,
                Posts = userPosts,
                PostsCount = userPosts.Count
            };
        }
    }
}
