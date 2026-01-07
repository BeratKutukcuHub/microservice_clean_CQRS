using System;
using System.Security.Claims;
using AbstractionBlocks.Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
namespace AbstractionBlocks.Common.Infrastructure.Concreate
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
                if (_context.HttpContext?.Items.TryGetValue("CorrelationId", out var correlationId) == true && correlationId != null)
                {
                    if (Guid.TryParse(correlationId.ToString(), out var guid))
                    {
                        return guid;
                    }
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
