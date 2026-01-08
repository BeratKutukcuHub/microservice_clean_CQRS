using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Messaging.Interfaces;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.UOW;
using MediatR;
namespace IdentityService.Application.Auth.Role.Commands
{
    public record CreateRoleCommand(string Name) : IRequest<RoleDto>;
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<CreateRoleCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public CreateRoleCommandHandler(
            IdentityService.Application.UOW.IUnitOfWork uow,
            ILoggerService<CreateRoleCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = IdentityService.Identity.Domain.Role.Create(request.Name);
            await _uow.RoleRepository.AddAsync(role);
            
            _logger.Information("Role.Created", new { ActorId = _uow.CurrentUser.UserId, TargetId = role.Id });

            // Publish integration events from entity
            foreach (var domainEvent in role.Events)
            {
                if (domainEvent is IntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent, cancellationToken);
                }
            }
            
            role.ClearEvents();

            return new RoleDto(role.Id, role.Name, role.Permissions);
        }
    }
}
