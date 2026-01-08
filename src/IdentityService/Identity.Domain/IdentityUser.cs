using System.ComponentModel;
using System.Text;
using IdentityService.Identity.Domain.Exceptions;
using IdentityService.Identity.Domain.Helper;
using IdentityService.Identity.Domain.Events;
using AbstractionBlocks.Common.Domain;
using System.Text.Json.Serialization;
namespace IdentityService.Identity.Domain
{
    public class IdentityUser : Entity, IAggregateRoot
    {
        public IdentityUser() { }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PasswordSalt { get; private set; } = string.Empty;
        public List<Guid> RoleIds { get; private set; } = new List<Guid>();
        public bool IsBlocked { get; private set; } = false;
        public List<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
        private List<IEventDomain> _events = new List<IEventDomain>();
        [JsonIgnore]
        public IReadOnlyList<IEventDomain> Events => _events;
        private IdentityUser(Guid id, string name, string email, string passwordhash, string passwordsalt)
        {
            Id = id;
            this.CreatedAt = DateTime.UtcNow;
            this.CreateById = Id;
            Name = name;
            Email = email;
            PasswordHash = passwordhash;
            this.PasswordSalt = passwordsalt;
        }
        public static IdentityUser Create(string name, string email, string passwordhash)
        {
            if (Checkers.IsValidEmail(email) &&
            Checkers.IsValidPassword(passwordhash))
            {
                var salt = PasswordHasher.GenerateSalt(16);
                var hash = Convert.ToBase64String(PasswordHasher.HashPassword(passwordhash, salt
                ));
                var user = new IdentityUser(Guid.NewGuid(), name, email, hash, Convert.ToBase64String(salt));
                
                // Raise integration event
                user._events.Add(new UserCreatedIntegrationEvent(
                    user.Id,
                    user.Name ?? string.Empty,
                    user.Email,
                    user.RoleIds));
                
                return user;
            }
            throw new UserIsNotValid();
        }
        public Guid LastRefreshToken()
        {
            return RefreshTokens.LastOrDefault().Token;
        }
        public void RemoveRole(Guid roleId)
        {
            if (RoleIds.Contains(roleId))
                RoleIds.Remove(roleId);
                else throw new UserHasNotRole(roleId);
        }
        public void BlockUser() => IsBlocked = true;
        public void UnblockUser() => IsBlocked = false;
        public IdentityUser UpdateIdentity(string? name, string? email, string? password)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!Checkers.IsValidEmail(email)) throw new UserIsNotValid();
                Email = email;
            }
            if (!string.IsNullOrWhiteSpace(password))
            {
                if (!Checkers.IsValidPassword(password)) throw new UserIsNotValid();
                var salt = PasswordHasher.GenerateSalt();
                var hash = Convert.ToBase64String(PasswordHasher.HashPassword(password, salt));
                PasswordSalt = Convert.ToBase64String(salt);
                PasswordHash = hash;
            }
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = Id;
            
            // Raise integration event
            _events.Add(new UserUpdatedIntegrationEvent(
                Id,
                Name ?? string.Empty,
                Email,
                RoleIds));
            
            return this;
        }
        public void SoftDelete()
        {
            IsDeleted = true;
            
            // Raise integration event
            _events.Add(new UserDeletedIntegrationEvent(
                Id,
                Email,
                true));
        }
        public void AddRole(Guid RoleId)
        {
            if (!RoleIds.Contains(RoleId))
            {
                RoleIds.Add(RoleId);
                
                // Raise integration event
                _events.Add(new RoleAssignedIntegrationEvent(Id, RoleId));
            }
            else throw new UserHasRole(RoleId.ToString());
        }
        public void ClearEvents()
        {
            _events.Clear();
        }

        public Guid AddRefreshToken()
        {
            var newtoken = Guid.NewGuid();
            foreach (var token in RefreshTokens)
            {
                token.Revoke();
            }
            RefreshTokens.Add(RefreshToken.Create(DateTime.UtcNow));
            return newtoken;
        }
    }
}
