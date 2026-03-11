using EventHub.Core.Models.Users;
using EventHub.Infrastructure.Data;
using EventHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class UserProfileSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        var adminId = await context.Users
         .Where(u => u.Email == "admin@eventhub.com")
         .Select(u => u.Id)
         .FirstOrDefaultAsync();


        var viaManager = await userManager.FindByEmailAsync("admin@eventhub.com");

        var profilesBefore = await context.UserProfiles.CountAsync();
    }
}
