using AbstractionBlocks.Common.Application.DI;
using AbstractionBlocks.Common.Validation;
using Category.Application.UseCases.Categories.Commands.CreateCategory;
using Microsoft.Extensions.DependencyInjection;
namespace Category.Application.DI;
public static class CategoryApplicationDIServices
{
    public static IServiceCollection AddCategoryApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCategoryCommand>());
        services.AddValidationInfrastructure(typeof(CreateCategoryCommand).Assembly);
        services.AddCommonApplicationServices();
        return services;
    }
}
