
namespace EventHub.Infrastructure.Data.Seed
{
    using EventHub.Core.Models;
    using System.Text.Json;

    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Locations.Any())
                return;

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                "Seed",
                "cities.json");

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
