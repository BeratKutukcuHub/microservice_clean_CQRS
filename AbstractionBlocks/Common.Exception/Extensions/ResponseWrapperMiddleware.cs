using System.Text.Json;
using AbstractionBlocks.Common.Exception;
using Microsoft.AspNetCore.Http;
public class ResponseWrapperMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/api") && 
            !context.Request.Path.StartsWithSegments("/gateway"))
        {
            await next(context);
            return;
        }
        var originalBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;
        try
        {
            await next(context);
            if (context.Response.StatusCode == 404 && memoryStream.Length == 0)
            {
                var wrapped404 = ApiResponse<object>.Error(new List<string> { "Matching endpoint not found." }, 404, context.TraceIdentifier);
                var json404 = JsonSerializer.Serialize(wrapped404, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                context.Response.Body = originalBody;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json404);
                return;
            }
            memoryStream.Position = 0;
            var bodyText = await new StreamReader(memoryStream).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(bodyText))
            {
                if (context.Response.StatusCode >= 400)
                {
                    var errorWrapped = ApiResponse<object>.Error(new List<string> { $"Request failed with status {context.Response.StatusCode}" }, context.Response.StatusCode, context.TraceIdentifier);
                    var errorJson = JsonSerializer.Serialize(errorWrapped, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    context.Response.Body = originalBody;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(errorJson);
                }
                return;
            }
            if (bodyText.Contains("\"isSuccess\":") || bodyText.Contains("\"IsSuccess\":"))
            {
                context.Response.Body = originalBody;
                await context.Response.WriteAsync(bodyText);
                return;
            }
            object? data;
            try
            {
                data = JsonSerializer.Deserialize<object>(bodyText);
            }
            catch
            {
                if (context.Response.StatusCode >= 400)
                {
                    data = bodyText;
                }
                else
                {
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsync(bodyText);
                    return;
                }
            }
            ApiResponse<object> wrapped;
            if (context.Response.StatusCode >= 400)
            {
                var errors = data is JsonElement je && je.ValueKind == JsonValueKind.Object
                    ? new List<string> { data.ToString()! }
                    : new List<string> { bodyText };
                wrapped = ApiResponse<object>.Error(errors, context.Response.StatusCode, context.TraceIdentifier);
            }
            else
            {
                wrapped = ApiResponse<object>.Success(data, context.TraceIdentifier, context.Response.StatusCode);
            }
            var json = JsonSerializer.Serialize(wrapped, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            context.Response.Body = originalBody;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}
