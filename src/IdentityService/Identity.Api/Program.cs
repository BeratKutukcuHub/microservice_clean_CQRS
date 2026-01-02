

using AbstactionBlocks.DIEnjections;
using DIEnjections;
using IdentityService.Application.DI;
using IdentityService.Identity.Infrastructure.DI;
using IdentityService.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.AddNLogLoggerService();
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddJwtBearerTokenSolverAuthenticationService
(builder.Configuration.GetSection("Jwt:SecretKey").Value ?? string.Empty);
builder.Services.AddIdentityApplicationDIServices();
builder.Services.AddIdentityInfrastructureDIServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider, IdentityService.Api.Security.PermissionPolicyProvider>();
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, IdentityService.Api.Security.PermissionHandler>();

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

using (var scope = app.Services.CreateScope())
{
    var uow = scope.ServiceProvider.GetRequiredService<IdentityService.Application.UOW.IUnitOfWork>();
    var adminEmail = "admin@antigravity.com";
    var adminUser = (await uow.IdentityRepository.FindAsync(x => x.Email == adminEmail)).FirstOrDefault();
    if (adminUser == null)
    {
        var adminRole = (await uow.RoleRepository.FindAsync(x => x.Name == "Admin")).FirstOrDefault();
        if (adminRole == null)
        {
            adminRole = IdentityService.Identity.Domain.Role.Create("Admin");
            adminRole.AddPermission("User.Create");
            adminRole.AddPermission("User.Delete");
            adminRole.AddPermission("User.ViewAll");
            adminRole.AddPermission("Role.Create");
            adminRole.AddPermission("Role.Delete");
            adminRole.AddPermission("Role.ViewAll");
            await uow.RoleRepository.AddAsync(adminRole);
        }

        var newAdmin = IdentityService.Identity.Domain.IdentityUser.Create("Admin", adminEmail, "Admin123!");
        newAdmin.AddRole(adminRole.Id);
        await uow.IdentityRepository.AddAsync(newAdmin);
    }
}

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapSwagger();
app.UseSwaggerUI();
app.Run();


