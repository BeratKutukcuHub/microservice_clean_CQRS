using AbstractionBlocks.Common.Infrastructure.DI;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.SecretBase.DI;
using UserProfileService.Application.DI;
using UserProfileService.Infrastructure.DI;
using NLog.Web;
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDIEnjectionsSecretBase();
builder.Services.AddDICommonInfrastructure();
builder.Services.AddUserProfileApplicationDIServices();
builder.Services.AddUserProfileInfrastructureDIServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
