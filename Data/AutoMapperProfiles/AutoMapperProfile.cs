using AutoMapper;
using Fakestagram.Data.DTOs;
using Fakestagram.Models;

namespace Fakestagram.Data.AutoMapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //TODO: Implement Automapper at a later stage...

            CreateMap<User, UserListFollowsDTO>();
            CreateMap<List<User>, List<UserListFollowsDTO>>();
            CreateMap<User, UserListLikesDTO>();
            CreateMap<List<User>, List<UserListLikesDTO>>();
        }
    }
}
