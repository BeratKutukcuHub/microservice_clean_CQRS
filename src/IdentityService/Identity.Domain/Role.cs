using System.Text.Json.Serialization;
using AbstractionBlocks.Common.Domain;
using IdentityService.Identity.Domain.Events;
namespace IdentityService.Identity.Domain
{
    public class Role : Entity, IAggregateRoot
    {
        public List<string> Permissions { get; private set; } = new();
        private readonly List<IEventDomain> _events = new();
        [JsonIgnore]
        public IReadOnlyList<IEventDomain> Events => _events;
        public void ClearEvents()
        {
            _events.Clear();
        }

        private Role(Guid id, string name)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            Name = name;
        }
        public static Role Create(string name)
        {
            var role = new Role(Guid.NewGuid(), name);
            
            // Raise integration event
            role._events.Add(new RoleCreatedIntegrationEvent(
                role.Id,
                role.Name ?? string.Empty,
                role.Permissions));
            
            return role;
        }
        public void AddPermission(string permission)
        {
            if (!Permissions.Contains(permission))
                Permissions.Add(permission);
        }
        public void RemovePermission(string permission)
        {
            Permissions.Remove(permission);
        }
    }
}
