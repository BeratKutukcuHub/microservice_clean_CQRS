using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception;
using Category.Application.Interfaces;
using MediatR;
namespace Category.Application.UseCases.Categories.Commands.DeleteCategory;
public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDispatcher _dispatcher;
    public DeleteCategoryHandler(
        ICategoryRepository categoryRepository,
        ICurrentUser currentUser,
        IApplicationDispatcher dispatcher)
    {
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
        _dispatcher = dispatcher;
    }
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException($"Category with ID {request.Id} not found");
        }
        category.Delete();
        await _categoryRepository.DeleteAsync(request.Id);
        await _dispatcher.Dispatch(category.Events);
        var audit = AuditLog.Create(
            "Category",
            category.Id,
            "Deleted",
            _currentUser.UserId,
            _currentUser.CorrelationId,
            "DeleteCategoryHandler",
            new List<ChangeDetail>
            {
                new() { Field = "Status", OldValue = "Active", NewValue = "Deleted" }
            }
        );
        audit.AddAuditEvent();
        await _dispatcher.Dispatch(audit.Events);
        return Unit.Value;
    }
}
