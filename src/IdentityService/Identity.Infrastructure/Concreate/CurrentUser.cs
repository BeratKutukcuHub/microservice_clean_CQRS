using System.Security.Claims;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IdentityService.Infrastructure.Concreate
{
    public class CurrentUser : ICurrentUser
    {
        public Guid UserId { get; }
        public string? Email { get; }
        public bool IsAuthenticated { get; }
        private readonly IHttpContextAccessor _context;
        public CurrentUser(IHttpContextAccessor context)
        {
            _context = context;
            var httpContext = _context.HttpContext;

            if (httpContext == null)
            {
                IsAuthenticated = false;
                return;
            }

            var user = httpContext.User;
            IsAuthenticated = user?.Identity?.IsAuthenticated == true;
            if (!IsAuthenticated)
                return;

            UserId = Guid.Parse(
            user!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            Email = user.FindFirst(ClaimTypes.Email)?.Value;
        }

    }
}
