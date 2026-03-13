

namespace EventHub.Infrastructure.Extensions
{
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Data.Seed;
    using EventHub.Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class SeedExtensions
    {
        public static async Task SeedDatabaseAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<ApplicationDbContext>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeed");

            try
            {
                await context.Database.MigrateAsync();
                await IdentitySeeder.SeedAsync(userManager, roleManager);
                context.ChangeTracker.Clear();
                await DataSeeder.SeedAsync(context);

                var adminUser = await userManager.FindByEmailAsync("admin@eventhub.com");
                var orgUser = await userManager.FindByEmailAsync("organizer@eventhub.com");

                if (adminUser != null && !await context.UserProfiles.AnyAsync(p => p.UserId == adminUser.Id))
                {
                    context.UserProfiles.Add(new UserProfile { UserId = adminUser.Id, FirstName = "Admin", LastName = "User", CreatedAt = DateTime.UtcNow });
                    await context.SaveChangesAsync();
                }

                if (orgUser != null && !await context.UserProfiles.AnyAsync(p => p.UserId == orgUser.Id))
                {
                    context.UserProfiles.Add(new UserProfile { UserId = orgUser.Id, FirstName = "Event", LastName = "Organizer", CreatedAt = DateTime.UtcNow });
                    await context.SaveChangesAsync();
                }

                await EventSeeder.SeedAsync(context, userManager);
                await InterestSeeder.SeedAsync(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SEED ERROR]: {Message}", ex.Message);
                throw;
            }
        }
    }
}
