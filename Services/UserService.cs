using AutoMapper;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;

namespace Fakestagram.Services
{
    public class UserService : GenericService<User, UserRegisterDTO, UserEditDTO, UserReadDTO>, IUserService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly IFollowRepository _followRepository;

        public UserService(IUsersRepository repo, IMapper mapper, IPostsRepository postsRepository, IFollowRepository followRepository) : base(repo, mapper)
        {
            _postsRepository = postsRepository;
            _followRepository = followRepository;
        }

        public void Follow(Guid userId)
        {
            _followRepository.Follow(userId, userId);
        }

        public string RegisterNewUser(UserRegisterDTO userRegisterDTO)
        {
            throw new NotImplementedException();
        }

        public void Unfollow(Guid userId)
        {
            _followRepository.Unfollow(userId, userId);
        }

        public string UserLogin(UserLoginDTO userLoginDTO)
        {
            throw new NotImplementedException();
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
