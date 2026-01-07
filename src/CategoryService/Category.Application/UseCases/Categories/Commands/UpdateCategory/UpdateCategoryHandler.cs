using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception;
using Category.Application.Interfaces;
using MediatR;
namespace Category.Application.UseCases.Categories.Commands.UpdateCategory;
public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDispatcher _dispatcher;
    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository,
        ICurrentUser currentUser,
        IApplicationDispatcher dispatcher)
    {
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
        _dispatcher = dispatcher;
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
        category.Update(request.Name, request.Description, request.IsActive);
        await _categoryRepository.UpdateAsync(category);
        await _dispatcher.Dispatch(category.Events);
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
        return new UpdateCategoryResponse(
            category.Id,
            category.Name!,
            category.Description,
            category.IsActive,
            category.UpdatedAt ?? DateTime.UtcNow
        );
    }
}
