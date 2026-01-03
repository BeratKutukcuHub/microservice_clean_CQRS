using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Identity.Application.Repository;
using MediatR;

namespace IdentityService.Application.Auth.Role.Commands
{
    public record AddPermissionToRoleCommand(Guid RoleId, string Permission) : IRequest<bool>;

    public class AddPermissionToRoleCommandHandler : IRequestHandler<AddPermissionToRoleCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<AddPermissionToRoleCommandHandler> _logger;

        public AddPermissionToRoleCommandHandler(IRoleRepository roleRepository, ILoggerService<AddPermissionToRoleCommandHandler> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(AddPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.RoleId} not found.");

            role.AddPermission(request.Permission);
            await _roleRepository.UpdateAsync(role);

            _logger.Information($"Permission {request.Permission} added to role {role.Name}", role.Id);
            return true;
        }
    }
}

