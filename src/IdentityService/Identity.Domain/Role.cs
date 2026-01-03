using AbstractionBlocks.Common.Domain;

namespace IdentityService.Identity.Domain
{
    public class Role : Entity, IAggregateRoot
    {
        public List<string> Permissions { get; private set; } = new();

        private Role(Guid id, string name)
        {
            Id = id;
            CreatedAt = DateTime.UtcNow;
            Name = name;
        }

        public static Role Create(string name) => new Role(Guid.NewGuid(), name);

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
