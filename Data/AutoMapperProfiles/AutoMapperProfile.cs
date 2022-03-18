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

            CreateMap<User, UserListFollowsDTO>().ForMember(dto => dto.UserId, m => m.MapFrom(m => m.Id));
            //CreateMap<List<User>, List<UserListFollowsDTO>>();
            CreateMap<User, UserListLikesDTO>().ForMember(dto => dto.UserId, m => m.MapFrom(m => m.Id));
            //CreateMap<List<User>, List<UserListLikesDTO>>();
        }
    }
}
