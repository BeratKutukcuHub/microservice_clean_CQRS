using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace AbstractionBlocks.Common.Exception.Logger
{
    public class LoggerService<TLogCategory> :
        ILoggerService<TLogCategory>
        where TLogCategory : class
    {
        private readonly ILogger<TLogCategory> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggerService(
            ILogger<TLogCategory> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Information(string message, object? context = null)
        {
            using (CreateScope(context))
            {
                _logger.LogInformation(message);
            }
        }

        public void Warning(string message, object? context = null)
        {
            using (CreateScope(context))
            {
                _logger.LogWarning(message);
            }
        }

        public void Warning(System.Exception exception, string message, object? context = null)
        {
            using (CreateScope(context))
            {
                _logger.LogWarning(exception, message);
            }
        }

        public void Error(System.Exception exception, string message, object? context = null)
        {
            using (CreateScope(context))
            {
                _logger.LogError(exception, message);
            }
        }

        private IDisposable CreateScope(object? context)
        {
            return _logger.BeginScope(new
            {
                context,
                correlationId = GetCorrelationId()
            });
        }

        private Guid? GetCorrelationId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.Items.TryGetValue("CorrelationId", out var value) == true
                && value is string correlationIdStr
                && Guid.TryParse(correlationIdStr, out var correlationId))
            {
                return correlationId;
            }

            return null;
        }
    }
}
