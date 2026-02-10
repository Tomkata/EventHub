
namespace EventHub.Infrastructure.Data.Seed
{
    using EventHub.Infrastructure;
    using Microsoft.AspNetCore.Identity;

    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { Roles.Admin, Roles.Organizer, Roles.User };

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
