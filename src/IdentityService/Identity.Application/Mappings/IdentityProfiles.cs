using AutoMapper;
using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Domain;
namespace IdentityService.Application.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<IdentityUser, CreateIdentityCommand>().ReverseMap();
            CreateMap<UpdateIdentityCommand, IdentityUser>().ReverseMap();
            CreateMap<IdentityUser, LoginCommand>().ReverseMap();
            CreateMap<IdentityUser, GetIdentityUser>().ReverseMap();
        }
    }
}
