using EventHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Infrastructure.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await EnsureRoleAsync(roleManager, "Admin");
            await EnsureRoleAsync(roleManager, "Organizer");

            await EnsureUserAsync(
                userManager,
                "admin@eventhub.com",
                "Admin123!",
                "Admin");

            await EnsureUserAsync(
                userManager,
                "organizer@eventhub.com",
                "Admin123!",
                "Organizer");

            await EnsureUserAsync(
                userManager,
                "user@eventhub.com",
                "User123!",
                null);
        }

        private static async Task EnsureRoleAsync(
            RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string? role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
            }

            if (role != null && !await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

}
