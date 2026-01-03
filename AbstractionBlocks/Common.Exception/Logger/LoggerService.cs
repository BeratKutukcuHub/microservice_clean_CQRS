
using AbstractionBlocks.Common.Exception.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AbstractionBlocks.Common.Exception.Logger
{
    public class LoggerService<TLogCategory> :
    ILoggerService<TLogCategory> where TLogCategory : class
    {
        private readonly ILogger<TLogCategory> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggerService(ILogger<TLogCategory> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid? GetCorrelationId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Items.TryGetValue("CorrelationId", out var correlationIdObj))
            {
                if (correlationIdObj is string correlationIdStr && Guid.TryParse(correlationIdStr, out var correlationId))
                {
                    return correlationId;
                }
            }
            return null;
        }

        public void Error(string message, System.Exception ex, Guid id, Guid? correlationId = null)
        => _logger.LogError(ex, message, id, correlationId ?? GetCorrelationId());

        public void Information(string message, Guid id, Guid? correlationId = null)
        => _logger.LogInformation(message, id, correlationId ?? GetCorrelationId());

        public void Warning(string message, Guid id, string reason, Guid? correlationId = null)
        => _logger.LogWarning(message, id, reason, correlationId ?? GetCorrelationId());

        public void Warning(string message, string email, string reason, Guid? correlationId = null)
        => _logger.LogWarning(message, email, reason, correlationId ?? GetCorrelationId());
    }
}
