using Category.Application.DI;
using Category.Infrastructure.DI;
using Microsoft.OpenApi.Models;
using AbstractionBlocks.DIEnjections;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.SecretBase.DI;
using AbstractionBlocks.Common.Infrastructure.DI;
using NLog.Web;
using Microsoft.AspNetCore.Authorization;
using Shared.Authentication;
using AbstractionBlocks.Common.Authentication.Security;
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
builder.Services.AddCategoryApplicationServices();
builder.Services.AddCategoryInfrastructureServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Category.Api", Version = "v1" });
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