using AbstractionBlocks.Common.Infrastructure.DI;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.SecretBase.DI;
using AbstractionBlocks.Common.Messaging.DI;
using UserProfileService.Application.DI;
using UserProfileService.Infrastructure.DI;
using NLog.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDIEnjectionsSecretBase();
builder.Services.AddDICommonInfrastructure();

builder.Services.AddUserProfileApplicationDIServices();
builder.Services.AddUserProfileInfrastructureDIServices();

// Add RabbitMQ
builder.Services.AddRabbitMQMessaging(builder.Configuration, "RabbitMQ");

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserProfile.Api", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI();

app.Run();
