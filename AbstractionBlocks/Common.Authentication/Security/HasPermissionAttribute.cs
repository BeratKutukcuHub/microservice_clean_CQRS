using Microsoft.AspNetCore.Authorization;
namespace Shared.Authentication.Security
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission) : base(permission)
        {
        }
    }
}