using MediatR;
namespace Category.Application.UseCases.Categories.Commands.DeleteCategory;
public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;
