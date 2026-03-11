
namespace EventHub.Infrastructure.Data.Seed
{
    using EventHub.Core.Models.Common;
    using System.Text.Json;

    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {



            if (context.Locations.Any())
                return;

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "Seed",
                "cities.json");

            if (!File.Exists(filePath))
            {
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);

            var locations = JsonSerializer.Deserialize<List<Location>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            await context.Locations.AddRangeAsync(locations);
            await context.SaveChangesAsync();
        }
    }
}
