using AbstractionBlocks.Common.Application.Caching;
using MediatR;
using UserProfileService.Application.DTOs;
namespace UserProfileService.Application.Queries.GetUserProfile
{
    [Cache("UserProfile", 5)] 
    public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto>;
}
