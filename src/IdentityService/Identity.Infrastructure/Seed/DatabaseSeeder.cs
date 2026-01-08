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

            // Seed Admin Role
            var adminRole = (await uow.RoleRepository.FindAsync(x => x.Name == "Admin")).FirstOrDefault();
            if (adminRole == null)
            {
                adminRole = Domain.Role.Create("Admin");
                
                // User Permissions
                adminRole.AddPermission("User.Create");
                adminRole.AddPermission("User.Update");
                adminRole.AddPermission("User.Delete");
                adminRole.AddPermission("User.ViewAll");
                adminRole.AddPermission("User.View");
                
                // Role Permissions
                adminRole.AddPermission("Role.Create");
                adminRole.AddPermission("Role.Update");
                adminRole.AddPermission("Role.Delete");
                adminRole.AddPermission("Role.ViewAll");
                adminRole.AddPermission("Role.View");
                
                // Product Permissions
                adminRole.AddPermission("Product.Create");
                adminRole.AddPermission("Product.Update");
                adminRole.AddPermission("Product.Delete");
                adminRole.AddPermission("Product.ViewAll");
                adminRole.AddPermission("Product.View");
                
                // Category Permissions
                adminRole.AddPermission("Category.Create");
                adminRole.AddPermission("Category.Update");
                adminRole.AddPermission("Category.Delete");
                adminRole.AddPermission("Category.ViewAll");
                adminRole.AddPermission("Category.View");
                
                // Order Permissions
                adminRole.AddPermission("Order.Create");
                adminRole.AddPermission("Order.Update");
                adminRole.AddPermission("Order.Delete");
                adminRole.AddPermission("Order.ViewAll");
                adminRole.AddPermission("Order.View");
                
                await uow.RoleRepository.AddAsync(adminRole);
            }

            // Seed Moderator Role
            var moderatorRole = (await uow.RoleRepository.FindAsync(x => x.Name == "Moderator")).FirstOrDefault();
            if (moderatorRole == null)
            {
                moderatorRole = Domain.Role.Create("Moderator");
                
                // User Permissions (Limited)
                moderatorRole.AddPermission("User.ViewAll");
                moderatorRole.AddPermission("User.View");
                moderatorRole.AddPermission("User.Update");
                
                // Product Permissions
                moderatorRole.AddPermission("Product.Create");
                moderatorRole.AddPermission("Product.Update");
                moderatorRole.AddPermission("Product.ViewAll");
                moderatorRole.AddPermission("Product.View");
                
                // Category Permissions
                moderatorRole.AddPermission("Category.Create");
                moderatorRole.AddPermission("Category.Update");
                moderatorRole.AddPermission("Category.ViewAll");
                moderatorRole.AddPermission("Category.View");
                
                // Order Permissions (View Only)
                moderatorRole.AddPermission("Order.ViewAll");
                moderatorRole.AddPermission("Order.View");
                moderatorRole.AddPermission("Order.Update");
                
                await uow.RoleRepository.AddAsync(moderatorRole);
            }

            // Seed User Role
            var userRole = (await uow.RoleRepository.FindAsync(x => x.Name == "User")).FirstOrDefault();
            if (userRole == null)
            {
                userRole = Domain.Role.Create("User");
                
                // Product Permissions (View Only)
                userRole.AddPermission("Product.View");
                userRole.AddPermission("Product.ViewAll");
                
                // Category Permissions (View Only)
                userRole.AddPermission("Category.View");
                userRole.AddPermission("Category.ViewAll");
                
                // Order Permissions (Own Orders)
                userRole.AddPermission("Order.Create");
                userRole.AddPermission("Order.View");
                userRole.AddPermission("Order.ViewOwn");
                
                // Profile Permissions
                userRole.AddPermission("Profile.View");
                userRole.AddPermission("Profile.Update");
                
                await uow.RoleRepository.AddAsync(userRole);
            }

            // Seed Admin User
            var adminEmail = "admin@example.com";
            var adminUser = (await uow.IdentityRepository.FindAsync(x => x.Email == adminEmail)).FirstOrDefault();
            if (adminUser == null)
            {
                var newAdmin = Domain.IdentityUser.Create("Admin", adminEmail, "Admin123!");
                newAdmin.AddRole(adminRole.Id);
                await uow.IdentityRepository.AddAsync(newAdmin);
            }

            // Seed Moderator User
            var moderatorEmail = "moderator@example.com";
            var moderatorUser = (await uow.IdentityRepository.FindAsync(x => x.Email == moderatorEmail)).FirstOrDefault();
            if (moderatorUser == null)
            {
                var newModerator = Domain.IdentityUser.Create("Moderator", moderatorEmail, "Moderator123!");
                newModerator.AddRole(moderatorRole.Id);
                await uow.IdentityRepository.AddAsync(newModerator);
            }

            // Seed Regular User
            var userEmail = "user@example.com";
            var regularUser = (await uow.IdentityRepository.FindAsync(x => x.Email == userEmail)).FirstOrDefault();
            if (regularUser == null)
            {
                var newUser = Domain.IdentityUser.Create("User", userEmail, "User123!");
                newUser.AddRole(userRole.Id);
                await uow.IdentityRepository.AddAsync(newUser);
            }
        }
    }
}
