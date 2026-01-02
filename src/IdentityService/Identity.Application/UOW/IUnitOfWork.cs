using IdentityService.Application.Interfaces;
using IdentityService.Identity.Domain.Repository;

namespace IdentityService.Application.UOW
{
    public interface IUnitOfWork
    {
        ICurrentUser CurrentUser { get; }
        IIdentityRepository IdentityRepository { get; }
        IRoleRepository RoleRepository { get; }
    }
}
