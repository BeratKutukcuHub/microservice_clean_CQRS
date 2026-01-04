using IdentityService.Application.Exceptions;
using IdentityService.Application.Interfaces;
using IdentityService.Identity.Domain.Events;

namespace IdentityService.Application.Auth.Identity.Events
{
    public interface IAuditLogEventHandler : IEventApplicationHandler<AuditLogEventDomain>
    {
    }
}