using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace AbstractionBlocks.DIEnjections
{
    public static class GlobalExceptionExtension
    {
        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection app)
        {
            app.AddTransient<GlobalExceptionHandler>();
            app.AddTransient<ResponseWrapperMiddleware>();
            app.AddTransient<CorrelationIdMiddleware>();
            return app;
        }

    }
}
