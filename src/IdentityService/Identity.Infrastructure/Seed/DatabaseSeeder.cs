using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.Application.UOW;
namespace IdentityService.Identity.Infrastructure.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task EnsureSeedDataAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;
            var uow = provider.GetRequiredService<IUnitOfWork>();
            var adminEmail = "admin@example.com";
            var adminUser = (await uow.IdentityRepository.FindAsync(x => x.Email == adminEmail)).FirstOrDefault();
            if (adminUser == null)
            {
                var adminRole = (await uow.RoleRepository.FindAsync(x => x.Name == "Admin")).FirstOrDefault();
                if (adminRole == null)
                {
                    adminRole = Domain.Role.Create("Admin");
                    adminRole.AddPermission("User.Create");
                    adminRole.AddPermission("User.Delete");
                    adminRole.AddPermission("User.ViewAll");
                    adminRole.AddPermission("Role.Create");
                    adminRole.AddPermission("Role.Delete");
                    adminRole.AddPermission("Role.ViewAll");
                    await uow.RoleRepository.AddAsync(adminRole);
                }
                var newAdmin = Domain.IdentityUser.Create("Admin", adminEmail, "Admin123!");
                newAdmin.AddRole(adminRole.Id);
                await uow.IdentityRepository.AddAsync(newAdmin);
            }
        }
    }
}