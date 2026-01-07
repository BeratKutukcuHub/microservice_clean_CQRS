using System.Security.Claims;
using AbstractionBlocks.Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
namespace ProductService.Product.Infrastructure.Concreate
{
    public class CurrentUser : ICurrentUser
    {
        public Guid UserId { get; }
        public string? Email { get; }
        public bool IsAuthenticated { get; }
        public Guid CorrelationId
        {
            get
            {
                var item = _context.HttpContext?.Items["CorrelationId"];
                if (item != null && Guid.TryParse(item.ToString(), out var correlationId))
                {
                    return correlationId;
                }
                return Guid.Empty;
            }
        }
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
            var userIdClaim = user!.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var guid))
            {
                UserId = guid;
            }
            Email = user.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
