using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Messaging.Interfaces;
using MediatR;
using UserProfileService.Application.Interfaces;

namespace UserProfileService.Application.Commands.DeleteUserProfile;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, bool>
{
    private readonly IUserProfileRepository _repository;
    private readonly IApplicationDispatcher _dispatcher;
    private readonly IEventBus _eventBus;

    public DeleteUserProfileCommandHandler(
        IUserProfileRepository repository,
        IApplicationDispatcher dispatcher,
        IEventBus eventBus)
    {
        _repository = repository;
        _dispatcher = dispatcher;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _repository.GetByUserIdAsync(request.UserId);
        if (userProfile == null)
        {
            throw new InvalidOperationException($"User profile with UserId {request.UserId} not found.");
        }

        // Use domain method to mark as deleted
        userProfile.Delete();

        // Use DeleteAsync from repository (soft delete is handled there)
        var result = await _repository.DeleteAsync(userProfile.Id);

        if (result)
        {
            await _dispatcher.Dispatch(userProfile.Events);

            // Publish integration events from entity
            foreach (var domainEvent in userProfile.Events)
            {
                if (domainEvent is IntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent, cancellationToken);
                }
            }
            
            userProfile.ClearDomainEvents();
        }

        return result;
    }
}
