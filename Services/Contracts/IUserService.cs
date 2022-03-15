﻿using Fakestagram.Data.DTOs;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Models;

namespace Fakestagram.Services.Contracts
{
    public interface IUserService : IGenericService<User, UserRegisterDTO, UserEditDTO, UserReadDTO>
    {
        string RegisterNewUser(UserRegisterDTO userRegisterDTO, out User user);
        string UserLogin(UserLoginDTO userLoginDTO);
        void Follow(Guid userId);
        void Unfollow(Guid userId);
        List<UserListFollowsDTO> GetUserFollowers(Guid userId);
        List<UserListFollowsDTO> GetUserFollowings(Guid userId);
        User GetCurrentUser();
    }
}
