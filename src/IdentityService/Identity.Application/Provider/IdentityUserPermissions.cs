using IdentityService.Identity.Domain;
namespace IdentityService.Identity.Application.Provider
{
    public class IdentityUserPermissions
    {
        public IdentityUser User { get; private set; }
        public List<string> Permissions { get; private set; }
        private IdentityUserPermissions(IdentityUser user, List<string> permissions)
        {
            User = user;
            Permissions = permissions;
        }
        public static IdentityUserPermissions Create(IdentityUser user, List<string> permissions) 
        => new IdentityUserPermissions(user, permissions);
    }
}