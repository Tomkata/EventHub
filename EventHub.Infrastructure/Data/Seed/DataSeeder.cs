using EventHub.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Data.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Locations.Any())
            {
                var jsonPath = "C:\\Users\\HP\\Desktop\\EventHub\\EventHub\\EventHub.Infrastructure\\Data\\Seed\\cities.json";

                var json = await File.ReadAllTextAsync(jsonPath);
                var locations = JsonSerializer.Deserialize<List<Location>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                await context.Locations.AddRangeAsync(locations);

                await context.SaveChangesAsync();
            }
        }
    }
}
