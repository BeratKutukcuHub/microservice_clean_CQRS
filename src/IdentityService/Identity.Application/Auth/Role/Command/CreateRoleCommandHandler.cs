using AbstractionBlocks.Common.Exception.Logger;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using MediatR;

namespace IdentityService.Application.Auth.Role.Commands
{
    public record CreateRoleCommand(string Name) : IRequest<RoleDto>;

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILoggerService<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(IUnitOfWork uow, ILoggerService<CreateRoleCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = IdentityService.Identity.Domain.Role.Create(request.Name);
            await _uow.RoleRepository.AddAsync(role);

            _logger.Information("Role created. Name: {RoleName}, CreatedBy: {UserId}", role.Id, _uow.CurrentUser.UserId);

            return new RoleDto(role.Id, role.Name, role.Permissions);
        }
    }
}

