using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Messaging.Interfaces;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;
namespace IdentityService.Application.Auth.Identity.Commands
{
    public record CreateIdentityCommand(string? name, string email, string password) : IRequest<Guid>;
    public class CreateIdentityCommandHandler : IRequestHandler<CreateIdentityCommand, Guid>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<CreateIdentityCommandHandler> _logger;
        private readonly IEventBus _eventBus;

        public CreateIdentityCommandHandler(
            IdentityService.Application.UOW.IUnitOfWork uow,
            ILoggerService<CreateIdentityCommandHandler> logger,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task<Guid> Handle(CreateIdentityCommand request, CancellationToken cancellationToken)
        {
            var user = IdentityUser.Create(request.name ?? string.Empty, request.email, request.password);
            
            var id = await _uow.IdentityRepository.AddAsync(user);
            
            _logger.Information("IdentityUser.Created", new
            {
                ActorId = _uow.CurrentUser.UserId,
                TargetId = id
            });

            // Publish integration events from entity
            foreach (var domainEvent in user.Events)
            {
                if (domainEvent is IntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent, cancellationToken);
                }
            }
            
            user.ClearEvents();

            return id;
        }
    }
}
