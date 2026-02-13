
namespace EventHub.Infrastructure.Data.Seed
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public static class EventSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            // Ако вече има events – не правим нищо
            if (await context.Events.AnyAsync())
                return;

            var organizer = await userManager.FindByEmailAsync("organizer@eventhub.com");

            if (organizer == null)
                throw new Exception("Organizer user not found. Identity seeding must run first.");

            var events = new List<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Rock the Night Festival",
                    Description = "Open-air rock concert featuring Bulgarian bands.",
                    ImagePath = "images/events/concert.jpg",
                    CreatedAt = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddMonths(3),
                    EndDate = DateTime.UtcNow.AddMonths(3).AddHours(5),
                    MaxParticipants = 500,
                    CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    LocationId = Guid.Parse("46EE85A7-5DA3-42B3-96D2-B58FD2B8CFC9"),
                    Address = "Rowing Canal, Plovdiv",
                    OrganizerId = organizer.Id
                },

                new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "ASP.NET Core Hands-on Workshop",
                    Description = "Practical workshop covering EF Core and Web APIs.",
                    ImagePath = "images/events/workshop.jpg",
                    CreatedAt = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddMonths(1),
                    EndDate = DateTime.UtcNow.AddMonths(1).AddHours(6),
                    MaxParticipants = 40,
                    CategoryId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    LocationId = Guid.Parse("D80720A5-6E69-44A5-87DD-997DF1E4DDC8"),
                    Address = "Tech Hub Ruse",
                    OrganizerId = organizer.Id
                },

                new Event
                {
                    Id = Guid.NewGuid(),
                    Title = "Past Tech Meetup",
                    Description = "Expired demo event.",
                    ImagePath = "images/events/meet-up.jpg",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    StartDate = DateTime.UtcNow.AddMonths(-3),
                    EndDate = DateTime.UtcNow.AddMonths(-3).AddHours(3),
                    MaxParticipants = 100,
                    CategoryId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    LocationId = Guid.Parse("B285237B-5DDC-449F-BF3A-C9CF5E805910"),
                    Address = "Sofia Coworking Space",
                    OrganizerId = organizer.Id
                }
            };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
        }
    }
}
