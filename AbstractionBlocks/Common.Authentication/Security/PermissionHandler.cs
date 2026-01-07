using Microsoft.AspNetCore.Authorization;
namespace AbstractionBlocks.Common.Authentication.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        public PermissionHandler()
        {
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissionClaims = context.User.FindAll("permission");
                    if (permissionClaims.Any(c => c.Value == requirement.Permission))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}