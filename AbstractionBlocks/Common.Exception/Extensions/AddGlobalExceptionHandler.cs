using System.Security.Claims;
using System.Text.Json;
using AbstractionBlocks.Common.Exception;
using AbstractionBlocks.Common.Exception.Logger;
using FluentValidation;
using Microsoft.AspNetCore.Http;
namespace AbstractionBlocks.DIEnjections
{
    public record ResponseExceptions(
        string message,
        int statusCode,
        string errorCode,
        Dictionary<string, string[]>? errors,
        string? correlationId);
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILoggerService<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(
            ILoggerService<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var mapped = MapException(ex);
                Guid? userId = GetUserId(context);
                LogException(ex, mapped, userId);
                var responseJson = BuildResponse(context, mapped);
                await context.Response.WriteAsync(responseJson);
            }
        }
        private static Guid? GetUserId(HttpContext context)
        {
            var value = context.User?
                .Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
        private void LogException(
            Exception ex,
            ResponseExceptions mapped,
            Guid? userId)
        {
            if (ex is BaseDomainException bde)
            {
                _logger.Error(ex, bde.Message);
            }
            else
            {
                _logger.Error(
                    exception: ex,
                    message: "Unhandled exception occurred");
            }
        }
        private static ResponseExceptions MapException(Exception ex)
        {
            if (ex is BaseDomainException bde)
            {
                return new ResponseExceptions(
                    bde.Message,
                    bde.StatusCode,
                    bde.ErrorCode,
                    bde.Errors,
                    bde.CorrelationId);
            }
            if (ex is FluentValidation.ValidationException ve)
            {
                var errors = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return new ResponseExceptions(
                    "Validation failed",
                    StatusCodes.Status400BadRequest,
                    "VALIDATION_ERROR",
                    errors,
                    null); 
            }
            return new ResponseExceptions(
                ex.Message,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                null,
                null);
        }
        private static string BuildResponse(
            HttpContext context,
            ResponseExceptions exMap)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exMap.statusCode;
            var errors = exMap.errors?
                .SelectMany(kv => kv.Value.Select(v => $"{kv.Key}: {v}"))
                .ToList()
                ?? new List<string>();
            var response = ApiResponse<object>.Error(
                errors,
                exMap.statusCode,
                exMap.correlationId);
            return JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
        }
    }
}
