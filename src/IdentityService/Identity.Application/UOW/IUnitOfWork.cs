using IdentityService.Application.Interfaces;
using IdentityService.Identity.Application.Repository;

namespace IdentityService.Application.UOW
{
    public interface IUnitOfWork
    {
        ICurrentUser CurrentUser { get; }
        IIdentityRepository IdentityRepository { get; }
        IRoleRepository RoleRepository { get; }
    }
}
