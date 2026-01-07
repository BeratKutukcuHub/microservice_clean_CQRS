using MailNotification.Application.DI;
using MailNotification.Infrastructure.DI;
using AbstractionBlocks.DIEnjections;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.SecretBase.DI;
using AbstractionBlocks.Common.Infrastructure.DI;
using NLog.Web;
NLog.LogManager.Setup().LoadConfigurationFromAppSettings();
var logger = NLog.LogManager.GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDIEnjectionsSecretBase();
builder.Services.AddDICommonInfrastructure();
builder.Services.AddResponseCaching();
builder.Services.AddMailNotificationApplicationDIServices();
builder.Services.AddMailNotificationInfrastructureDIServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<ResponseWrapperMiddleware>();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();
