using System.Text.Json;
using AbstractionBlocks.CommonExceptionBase;
using Microsoft.AspNetCore.Http;

public class ResponseWrapperMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
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

            if (context.Response.StatusCode >= 400)
                return;

            if (!context.Response.ContentType?.Contains("application/json") == true)
                return;

            memoryStream.Position = 0;
            var bodyText = await new StreamReader(memoryStream).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(bodyText))
                return;

            object? data;
            try
            {
                data = JsonSerializer.Deserialize<object>(bodyText);
            }
            catch
            {
                return;
            }

            var wrapped = ApiResponse<object>.Success(
                data,
                context.TraceIdentifier,
                200
            );

            var json = JsonSerializer.Serialize(
                wrapped,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

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

