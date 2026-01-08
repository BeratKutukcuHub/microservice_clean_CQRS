using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace IdentityService.Identity.Domain.Events;

public class RoleCreatedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid RoleId { get; set; }
    public string Name { get; set; }
    public List<string> Permissions { get; set; }

    public RoleCreatedIntegrationEvent(
        Guid roleId,
        string name,
        List<string> permissions) : base("IdentityService", "v1")
    {
        RoleId = roleId;
        Name = name;
        Permissions = permissions;
    }
}
