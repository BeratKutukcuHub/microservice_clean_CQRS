using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AbstactionBlocks.DIEnjections
{
    public class CorrelationIdMiddleware : IMiddleware
    {
        private const string CorrelationIdHeaderKey = "X-Correlation-Id";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out StringValues correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Response.Headers[CorrelationIdHeaderKey] = correlationId;
            context.Items["CorrelationId"] = correlationId.ToString();
            context.TraceIdentifier = correlationId.ToString();

            await next(context);
        }
    }
}

