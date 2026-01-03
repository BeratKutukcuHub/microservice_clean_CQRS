using System.ComponentModel;
using System.Text;
using IdentityService.Identity.Domain.Exceptions;
using IdentityService.Identity.Domain.Helper;
using AbstractionBlocks.Common.Domain;
namespace IdentityService.Identity.Domain
{

    public class IdentityUser : Entity, IAggregateRoot
    {
        public IdentityUser() { }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string PasswordSalt { get; private set; } = string.Empty;
        public List<Guid> RoleIds { get; private set; } = new List<Guid>();
        public List<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
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
                return new IdentityUser(Guid.NewGuid(), name, email, hash, Convert.ToBase64String(salt));
            }
            throw new UserIsNotValid();
        }

        public IdentityUser UpdateIdentity(string? name, string? email, string? password)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;

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
            return this;
        }

        public void SoftDelete() => IsDeleted = true;
        public void AddRole(Guid RoleId)
        {
            if (!RoleIds.Contains(RoleId))
            {
                RoleIds.Add(RoleId);
            }
            else throw new UserHasRole(RoleId.ToString());
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
