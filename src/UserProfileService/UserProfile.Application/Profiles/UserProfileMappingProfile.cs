using AutoMapper;
using UserProfileService.Application.DTOs;
using UserProfileService.Domain.Entities;
namespace UserProfileService.Application.Profiles
{
    public class UserProfileMappingProfile : Profile
    {
        public UserProfileMappingProfile()
        {
            CreateMap<UserProfileService.Domain.Entities.UserProfile, UserProfileDto>();
        }
    }
}
