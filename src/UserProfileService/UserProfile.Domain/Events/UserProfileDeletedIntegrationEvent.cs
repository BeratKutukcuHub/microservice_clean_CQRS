using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace UserProfileService.Domain.Events;

public class UserProfileDeletedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid ProfileId { get; set; }
    public Guid UserId { get; set; }

    public UserProfileDeletedIntegrationEvent(
        Guid profileId,
        Guid userId) : base("UserProfileService", "v1")
    {
        ProfileId = profileId;
        UserId = userId;
    }
}
