
namespace EventHub.Infrastructure.Data.Seed
{
    using EventHub.Core.Models.Events;
    using EventHub.Infrastructure.Data;
    using EventHub.Infrastructure.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public static class EventSeeder
    {   
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Events.AnyAsync())
                return;

            var organizer = await context.UserProfiles.FirstOrDefaultAsync();
            if (organizer == null)
                return;


            async Task<Guid> GetCityId(string city)
                => await context.Locations
                    .Where(l => l.City == city)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync();

            var plovdivId = await GetCityId("Plovdiv");
            var ruseId = await GetCityId("Ruse");
            var sofiaId = await GetCityId("Sofia");

            if (plovdivId == Guid.Empty || ruseId == Guid.Empty || sofiaId == Guid.Empty)
                throw new Exception("Required locations not found. Ensure DataSeeder runs before EventSeeder.");

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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
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
            LocationId = ruseId,
            Address = "Tech Hub Ruse",
            OrganizerId = organizer.UserId
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
            LocationId = sofiaId,
            Address = "Sofia Coworking Space",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
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
            LocationId = plovdivId,
            Address = "Rowing Canal, Plovdiv",
            OrganizerId = organizer.UserId
        },
    };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
        }

    }
}
