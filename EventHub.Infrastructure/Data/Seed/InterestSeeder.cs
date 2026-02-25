using EventHub.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Infrastructure.Data.Seed
{
    public static class InterestSeeder 
    {
            public static async  Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Interests.AnyAsync())
                return;

            var interests = new List<Interest>
{
    new Interest { Id = Guid.NewGuid(), InterestName = "Technology" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Programming" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Artificial Intelligence" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Startups" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Entrepreneurship" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Gaming" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Board Games" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Fitness" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Running" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Yoga" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Hiking" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Travel" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Photography" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Music" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Live Concerts" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Movies" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Reading" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Cooking" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Food & Dining" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Art & Design" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Volunteering" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Networking" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Public Speaking" },
    new Interest { Id = Guid.NewGuid(), InterestName = "Investing" }
};

            context.Interests.AddRange(interests);
            await context.SaveChangesAsync();

        }
    }
}
