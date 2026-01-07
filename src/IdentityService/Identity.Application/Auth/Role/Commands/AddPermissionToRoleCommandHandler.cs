using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Identity.Application.Repository;
using MediatR;
using IdentityService.Application.UOW;
namespace IdentityService.Application.Auth.Role.Commands
{
    public record AddPermissionToRoleCommand(Guid RoleId, string Permission) : IRequest<bool>;
    public class AddPermissionToRoleCommandHandler : IRequestHandler<AddPermissionToRoleCommand, bool>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<AddPermissionToRoleCommandHandler> _logger;
        public AddPermissionToRoleCommandHandler(ILoggerService<AddPermissionToRoleCommandHandler> logger, IdentityService.Application.UOW.IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }
        public async Task<bool> Handle(AddPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _uow.RoleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.RoleId} not found.");
            role.AddPermission(request.Permission);
            await _uow.RoleRepository.UpdateAsync(role);
            _logger.Information("Role.PermissionAdded", new
            {
                Action = "Add",
                ActorId = _uow.CurrentUser.UserId,
                TargetId = role.Id
            });
            return true;
        }
    }
}
