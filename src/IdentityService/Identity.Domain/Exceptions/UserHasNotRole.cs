namespace IdentityService.Identity.Domain.Exceptions
{
        public class UserHasNotRole : Exception
        {
        public UserHasNotRole(Guid roleId) : base($"The user has not the role {roleId}")
        {
        }
        }
}