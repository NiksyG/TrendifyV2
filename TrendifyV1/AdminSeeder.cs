using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TrendifyV1.Data.Entities; // namespace за ApplicationUser

namespace TrendifyV1.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Administrator", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

            string adminUsername = "TheWeekend";
            string adminEmail = "TheWeekndXO@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            var user = await userManager.FindByNameAsync(adminUsername);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail
                };

                await userManager.CreateAsync(user, "Admin1234*");
            }

            if (!await userManager.IsInRoleAsync(user, "Administrator"))
            {
                await userManager.AddToRoleAsync(user, "Administrator");
            }
        }
    }
}