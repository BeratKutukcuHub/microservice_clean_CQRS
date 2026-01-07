using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Identity.Application.Repository;
using MediatR;
using IdentityService.Application.UOW;
namespace IdentityService.Application.Auth.Role.Commands
{
    public record RemovePermissionFromRoleCommand(Guid RoleId, string Permission) : IRequest<bool>;
    public class RemovePermissionFromRoleCommandHandler : IRequestHandler<RemovePermissionFromRoleCommand, bool>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<RemovePermissionFromRoleCommandHandler> _logger;
        public RemovePermissionFromRoleCommandHandler(ILoggerService<RemovePermissionFromRoleCommandHandler> logger, IdentityService.Application.UOW.IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public async Task<bool> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _uow.RoleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.RoleId} not found.");
            role.RemovePermission(request.Permission);
            await _uow.RoleRepository.UpdateAsync(role);
            _logger.Information("Role.PermissionRemoved", new
            {
                Action = "Remove",
                ActorId = _uow.CurrentUser.UserId,
                TargetId = role.Id
            });
            return true;
        }
    }
}
