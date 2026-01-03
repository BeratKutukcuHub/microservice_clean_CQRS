using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Identity.Application.Repository;
using MediatR;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<bool>;

    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, bool>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILoggerService<AssignRoleCommandHandler> _logger;

        public AssignRoleCommandHandler(IIdentityRepository identityRepository, IRoleRepository roleRepository, ILoggerService<AssignRoleCommandHandler> logger)
        {
            _identityRepository = identityRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException($"User with ID {request.UserId} not found.");

            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException($"Role with ID {request.RoleId} not found.");

            user.AddRole(role.Id);
            await _identityRepository.UpdateAsync(user);

            _logger.Information($"Role {role.Name} assigned to user {user.Email}", user.Id, default);
            return true;
        }
    }
}

