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

        Console.WriteLine($"[DIAG] Admin ID от context.Users: '{adminId}'");

        var viaManager = await userManager.FindByEmailAsync("admin@eventhub.com");
        Console.WriteLine($"[DIAG] Admin от UserManager: '{viaManager?.Id ?? "NULL"}'");

        var profilesBefore = await context.UserProfiles.CountAsync();
        Console.WriteLine($"[DIAG] Профили преди: {profilesBefore}");
    }
}
