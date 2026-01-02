using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Api.Security
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(permission)
        {
        }
    }
}

