using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace IdentityService.Identity.Domain.Events;

public class UserDeletedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public bool IsSoftDelete { get; set; }

    public UserDeletedIntegrationEvent(
        Guid userId,
        string email,
        bool isSoftDelete) : base("IdentityService", "v1")
    {
        UserId = userId;
        Email = email;
        IsSoftDelete = isSoftDelete;
    }
}
