using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace IdentityService.Identity.Domain.Events;

public class RoleAssignedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public RoleAssignedIntegrationEvent(
        Guid userId,
        Guid roleId) : base("IdentityService", "v1")
    {
        UserId = userId;
        RoleId = roleId;
    }
}
