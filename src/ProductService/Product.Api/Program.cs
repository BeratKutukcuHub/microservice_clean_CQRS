using AbstractionBlocks.Common.Authentication.Security;
using Shared.Authentication;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Infrastructure.DI;
using AbstractionBlocks.Common.SecretBase.DI;
using AbstractionBlocks.DIEnjections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using NLog.Web;
using ProductService.Product.Application.DI;
using ProductService.Product.Infrastructure.DI;
NLog.LogManager.Setup().LoadConfigurationFromAppSettings();
var logger = NLog.LogManager.GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddDIEnjectionsSecretBase();
builder.Services.AddDICommonAuthentication();
builder.Services.AddDICommonInfrastructure();
builder.Services.AddResponseCaching();
builder.Services.AddProductApplicationDIServices();
builder.Services.AddProductInfrastructureDIServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});
var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI();
app.Run();
