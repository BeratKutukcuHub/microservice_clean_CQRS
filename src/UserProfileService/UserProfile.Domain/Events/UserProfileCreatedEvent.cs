using AbstractionBlocks.Common.Domain;
using UserProfileService.Domain.Entities;
namespace UserProfileService.Domain.Events
{
    public record UserProfileCreatedEvent(UserProfile UserProfile) : IEventDomain;
}
