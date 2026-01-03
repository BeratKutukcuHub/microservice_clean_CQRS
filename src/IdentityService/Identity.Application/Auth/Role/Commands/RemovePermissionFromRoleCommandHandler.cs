using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Identity.Application.Repository;
using MediatR;

namespace IdentityService.Application.Auth.Role.Commands
{
    public record RemovePermissionFromRoleCommand(Guid RoleId, string Permission) : IRequest<bool>;

    public class RemovePermissionFromRoleCommandHandler : IRequestHandler<RemovePermissionFromRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<RemovePermissionFromRoleCommandHandler> _logger;

        public RemovePermissionFromRoleCommandHandler(IRoleRepository roleRepository, ILoggerService<RemovePermissionFromRoleCommandHandler> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.RoleId} not found.");

            role.RemovePermission(request.Permission);
            await _roleRepository.UpdateAsync(role);

            _logger.Information($"Permission {request.Permission} removed from role {role.Name}", role.Id);
            return true;
        }
    }
}

