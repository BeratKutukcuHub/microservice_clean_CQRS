using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace AbstactionBlocks.DIEnjections
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
        public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<GlobalExceptionHandler>();
            app.UseMiddleware<ResponseWrapperMiddleware>();
            return app;
        }
    }
}
