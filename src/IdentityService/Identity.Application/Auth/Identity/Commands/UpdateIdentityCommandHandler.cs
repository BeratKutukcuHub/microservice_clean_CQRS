using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Messaging.Interfaces;
using IdentityService.Application.Auth.Identity.Profile;
using IdentityService.Application.Exceptions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
namespace IdentityService.Application.Auth.Identity.Commands
{
    public class UpdateIdentityCommand : IRequest<UpdateIdentityResponse>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
    public class UpdateIdentityCommandHandler : IRequestHandler<UpdateIdentityCommand, UpdateIdentityResponse>
    {
        private readonly IdentityService.Application.UOW.IUnitOfWork _uow;
        private readonly ILoggerService<UpdateIdentityCommandHandler> _logger;
        private readonly IApplicationDispatcher _dispatcher;
        private readonly IEventBus _eventBus;

        public UpdateIdentityCommandHandler(
            IdentityService.Application.UOW.IUnitOfWork uow,
            ILoggerService<UpdateIdentityCommandHandler> logger,
            IApplicationDispatcher dispatcher,
            IEventBus eventBus)
        {
            _uow = uow;
            _logger = logger;
            _dispatcher = dispatcher;
            _eventBus = eventBus;
        }
        public async Task<UpdateIdentityResponse> Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
        {
            var result = await _uow.IdentityRepository.GetByIdAsync(request.Id);
            if (result is null)
            {
                _logger.Warning("Identtiy.UpdateUser Id not found", new
                {
                    Action = "Update",
                    ActorId = _uow.CurrentUser.UserId,
                    TargetId = request.Id
                });
                throw new NotFoundExceptionApp(request.Id.ToString());
            }
            var updated = result.UpdateIdentity(request.Name, request.Email, request.Password);
            _logger.Information("IdentityUserUpdated", new
            {
                Action = "Update",
                ActorId = _uow.CurrentUser.UserId,
                TargetId = request.Id
            });
            var response = await _uow.IdentityRepository.UpdateAsync(updated);
            var changes = new List<ChangeDetail>();
            if (request.Name is not null)
                changes.Add(new ChangeDetail { Field = "Name", NewValue = request.Name, OldValue = result.Name });
            if (request.Email is not null)
                changes.Add(new ChangeDetail { Field = "Email", NewValue = request.Email, OldValue = result.Email });
            var audit = AuditLog.Create("IdentityUser", request.Id, "Update", _uow.CurrentUser.UserId, _uow.CurrentUser.CorrelationId,
            "UpdateIdentityCommandHandler", changes);
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);

            // Publish integration events from entity
            foreach (var domainEvent in updated.Events)
            {
                if (domainEvent is IntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent, cancellationToken);
                }
            }
            
            updated.ClearEvents();

            return new UpdateIdentityResponse(response.Id, response.Name, response.Email);
        }
    }
}
