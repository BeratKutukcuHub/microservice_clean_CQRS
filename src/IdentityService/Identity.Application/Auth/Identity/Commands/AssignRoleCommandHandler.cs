using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Exception;
using IdentityService.Application.Interfaces;
using IdentityService.Identity.Application.Repository;
using MediatR;
using IdentityService.Identity.Domain;

namespace IdentityService.Application.Auth.Identity.Commands
{
    public record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<bool>;
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, bool>
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditRepository _auditService;
        private readonly ILoggerService<AssignRoleCommandHandler> _logger;
        private readonly ICurrentUser _currentUser;
        public AssignRoleCommandHandler(
            IIdentityRepository identityRepository,
            IRoleRepository roleRepository,
            IAuditRepository auditService,
            ILoggerService<AssignRoleCommandHandler> logger,
            ICurrentUser currentUser)
        {
            _identityRepository = identityRepository;
            _roleRepository = roleRepository;
            _auditService = auditService;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityRepository.GetByIdAsync(request.UserId)
                ?? throw new NotFoundException($"User with ID {request.UserId} not found.");
            var role = await _roleRepository.GetByIdAsync(request.RoleId)
                ?? throw new NotFoundException($"Role with ID {request.RoleId} not found.");

            var alreadyHasRole = user.RoleIds.Contains(role.Id);
            if (!alreadyHasRole)
            {
                user.AddRole(role.Id);
                await _identityRepository.UpdateAsync(user);
            }

            await _auditService.AddAuditLogAsync(
                AuditLog.Create(
                    "IdentityUser",
                    user.Id,
                    "AssignRole",
                    _currentUser.UserId,
                    Guid.NewGuid(),
                    "AssignRoleCommandHandler",
                    new List<ChangeDetail>
                    {
                        new ChangeDetail
                        {
                        Field = "Role",
                        OldValue = alreadyHasRole ? role.Name : null,
                        NewValue = role.Name
                        }
                    }));            
            _logger.Information("Role {RoleName} assigned to user {UserEmail}", Guid.NewGuid(), default);

            return true;
        }
    }
}
