using AbstractionBlocks.CommonExceptionBase;

namespace IdentityService.Identity.Domain.Exceptions
{
    public class UserHasRole : ValidationException
    {
        public UserHasRole(string roleId): 
        base($"User has {roleId} role.",
        new Dictionary<string, string[]> { { "errors", new string[] { $"User has {roleId} role." } } })
        {
            
        }
    }
}
