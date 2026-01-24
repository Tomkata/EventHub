using Microsoft.AspNetCore.Identity;

namespace EventHub.Infrastructure.Data.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin","Organizer","User" };

            foreach (var role in roles)
            {
                var exist = await roleManager.RoleExistsAsync(role);
                if (!exist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
