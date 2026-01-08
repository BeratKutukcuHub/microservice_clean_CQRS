using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.Events;
using Category.Application.Interfaces;
using MediatR;

namespace Category.Application.UseCases.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDispatcher _dispatcher;
    private readonly IEventBus _eventBus;

    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository,
        ICurrentUser currentUser,
        IApplicationDispatcher dispatcher,
        IEventBus eventBus)
    {
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
        _dispatcher = dispatcher;
        _eventBus = eventBus;
    }

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException($"Category with ID {request.Id} not found");
        }

        var oldState = new
        {
            category.Name,
            category.Description,
            category.IsActive
        };

        category.Update(request.Name, request.Description, request.IsActive, _currentUser.UserId);

        await _categoryRepository.UpdateAsync(category);

        await _dispatcher.Dispatch(category.Events);

        // Publish integration events from domain
        foreach (var domainEvent in category.Events)
        {
            if (domainEvent is IntegrationEvent integrationEvent)
            {
                await _eventBus.PublishAsync(integrationEvent, cancellationToken);
            }
        }

        var audit = AuditLog.Create(
            "Category",
            category.Id,
            "Updated",
            _currentUser.UserId,
            _currentUser.CorrelationId,
            "UpdateCategoryHandler",
            new List<ChangeDetail>
            {
                new() { Field = "Name", OldValue = oldState.Name, NewValue = category.Name },
                new() { Field = "Description", OldValue = oldState.Description, NewValue = category.Description },
                new() { Field = "IsActive", OldValue = oldState.IsActive.ToString(), NewValue = category.IsActive.ToString() }
            }
        );

        audit.AddAuditEvent();
        await _dispatcher.Dispatch(audit.Events);

        category.ClearEvents();

        return new UpdateCategoryResponse(
            category.Id,
            category.Name!,
            category.Description,
            category.IsActive,
            category.UpdatedAt ?? DateTime.UtcNow
        );
    }
}
