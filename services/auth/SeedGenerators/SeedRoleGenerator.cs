using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Roza.AuthService.SeedGenerators
{
    public  class SeedRoleGenerator
    {
        public static async Task Do(IServiceProvider serviceProvider)
        {
            // Ensure the roles are created at application startup
            using var scope = serviceProvider.CreateScope();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User", "Manager" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
