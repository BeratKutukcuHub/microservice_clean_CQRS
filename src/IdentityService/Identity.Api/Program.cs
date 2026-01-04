

using IdentityService.Application.DI;
using IdentityService.Identity.Infrastructure.DI;
using Microsoft.OpenApi.Models;
using AbstractionBlocks.DIEnjections;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.SecretBase.DI;
using Shared.Authentication;
using IdentityService.Identity.Infrastructure.Seed;
using NLog.Web;
using AbstractionBlocks.Common.Authentication.Security;
using Microsoft.AspNetCore.Authorization;

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
builder.Services.AddIdentityApplicationDIServices();
builder.Services.AddIdentityInfrastructureDIServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.Api", Version = "v1" });
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

    await app.Services.EnsureSeedDataAsync();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI();
app.Run();

