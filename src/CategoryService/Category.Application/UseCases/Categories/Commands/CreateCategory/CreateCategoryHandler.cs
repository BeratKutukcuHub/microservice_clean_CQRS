using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception.Logger;
using Category.Application.Interfaces;
using MediatR;
namespace Category.Application.UseCases.Categories.Commands.CreateCategory;
public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDispatcher _dispatcher;
    private readonly ILoggerService<CreateCategoryHandler> _logger;
    public CreateCategoryHandler(
        ICategoryRepository repository,
        ICurrentUser currentUser,
        IApplicationDispatcher dispatcher,
        ILoggerService<CreateCategoryHandler> logger)
    {
        _repository = repository;
        _currentUser = currentUser;
        _dispatcher = dispatcher;
        _logger = logger;
    }
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _repository.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists");
        }
        var category = Domain.Category.Create(
            request.Name,
            request.Description,
            request.ImageUrl,
            request.ParentCategoryId,
            _currentUser.UserId
        );
        var categoryId = await _repository.AddAsync(category);
        await _dispatcher.Dispatch(category.Events);
        var audit = AuditLog.Create(
            "Category",
            categoryId,
            "Created",
            _currentUser.UserId,
            _currentUser.CorrelationId,
            "CreateCategoryHandler",
            new List<ChangeDetail>
            {
                new ChangeDetail { Field = "Name", NewValue = request.Name, OldValue = null },
                new ChangeDetail { Field = "Description", NewValue = request.Description, OldValue = null }
            }
        );
        audit.AddAuditEvent();
        await _dispatcher.Dispatch(audit.Events);
        _logger.Information("Category created", new { CategoryId = categoryId, Name = request.Name });
        return new CreateCategoryResponse(
            categoryId,
            category.Name!,
            category.Description,
            category.ImageUrl,
            category.ParentCategoryId,
            category.CreatedAt
        );
    }
}
