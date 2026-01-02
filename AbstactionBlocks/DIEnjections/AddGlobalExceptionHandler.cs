
using System.Security.Claims;
using System.Text.Json;
using AbstractBlocks.CommonDomain.Logger;
using AbstractionBlocks.CommonExceptionBase;
using Microsoft.AspNetCore.Http;
namespace AbstactionBlocks.DIEnjections
{
    public record ResponseExceptions(string message,
    int statusCode,
    string errorCode,
    Dictionary<string, string[]>? errors,
    string? correlationId);

    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILoggerService<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILoggerService<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        private ResponseExceptions MapException(Exception ex)
        {
            if (ex is BaseDomainException bde)
            {
                return new ResponseExceptions(bde.Message, bde.StatusCode, bde.ErrorCode, bde.Errors, bde.CorrelationId);
            }

            return new ResponseExceptions(ex.Message, 500, "INTERNAL_SERVER_ERROR", null, null);
        }
        private string ExceptionResponser(HttpContext context, ResponseExceptions exMap)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exMap.statusCode;
            var errors = exMap.errors?.SelectMany(kv => kv.Value.Select(v => $"{kv.Key}: {v}")).ToList()
             ?? new List<string>();
            var response = ApiResponse<object>.Error(errors, exMap.statusCode, exMap.correlationId);
            return JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var mapEx = MapException(ex);
                Guid? id = context?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.
                Value is string val && Guid.TryParse(val, out var guidVal) ? guidVal : null;

                var correlationIdFromItems = context.Items["CorrelationId"] as string;
                Guid? correlationId = Guid.TryParse(mapEx.correlationId ?? correlationIdFromItems, out var cId) ? cId : (Guid?)null;

                _logger.Error("Unhandled exception: {Message}", ex, id ?? default, correlationId ?? default);
                var result = ExceptionResponser(context, mapEx);
                await context.Response.WriteAsync(result);
            }
        }
    }
}
