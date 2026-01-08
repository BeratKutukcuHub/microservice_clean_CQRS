using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace IdentityService.Identity.Domain.Events;

public class UserUpdatedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Guid> RoleIds { get; set; }

    public UserUpdatedIntegrationEvent(
        Guid userId,
        string name,
        string email,
        List<Guid> roleIds) : base("IdentityService", "v1")
    {
        UserId = userId;
        Name = name;
        Email = email;
        RoleIds = roleIds;
    }
}
