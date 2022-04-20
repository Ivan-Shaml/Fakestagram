using Fakestagram.Data.DTOs;
using Fakestagram.Data.DTOs.Tokens;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IUserService : IGenericService<User, UserRegisterDTO, UserEditDTO, UserReadDTO>
    {
        TokenAuthDTO RegisterNewUser(UserRegisterDTO userRegisterDTO, out User user);
        TokenAuthDTO UserLogin(UserLoginDTO userLoginDTO);
        TokenAuthDTO RefreshToken(TokenAuthDTO authDto);
        void RevokeRefreshToken(string refreshToken);
        void Follow(Guid userId);
        void Unfollow(Guid userId);
        List<UserListFollowsDTO> GetUserFollowers(Guid userId);
        List<UserListFollowsDTO> GetUserFollowings(Guid userId);
        User GetCurrentUser();
        bool IsCurrentUserAdmin();
        void ChangeUserRole(Guid userId);
    }
}
