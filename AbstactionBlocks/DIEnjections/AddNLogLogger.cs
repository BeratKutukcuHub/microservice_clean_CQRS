using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace DIEnjections
{
    public static class AddNLogLogger
    {
        public static IServiceCollection AddNLogLoggerService(this WebApplicationBuilder builder)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Host.UseNLog();
            return builder.Services;
        }
    }
}
