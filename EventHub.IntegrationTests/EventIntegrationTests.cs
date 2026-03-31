using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.Models.Common;
using EventHub.Core.Models.Events;
using EventHub.Core.Models.Users;
using EventHub.Infrastructure.Data;
using EventHub.Infrastructure.Identity;
using EventHub.Repositories.Repositories.Common;
using EventHub.Repositories.Repositories.Events;
using EventHub.Repositories.Repositories.User;
using EventHub.Services.Mapping;
using EventHub.Services.Services.Event;
using Microsoft.EntityFrameworkCore;

namespace EventHub.IntegrationTests
{
    public class EventIntegrationTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var option = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(option);
        }

        [Fact]
        public async Task CreateEvent_ShouldPersistInDatabase()
        {
            var db = CreateDbContext();

            var eventRepo = new EventRepository(db);
            var categoryRepo = new CategoryRepository(db);
            var locationRepo = new LocationRepository(db);
            var participantsRepo = new EventParticipantsRepository(db);
            var userProfileRepo = new UserProfileRepository(db);

            var mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ServiceMappingProfile>(); 
            }));

            var service = new EventService(
                eventRepo,
                participantsRepo,
                categoryRepo,
                locationRepo,
                mapper,
                userProfileRepo
            );



            var category = new Category { Id = Guid.NewGuid(), Name = "Test" };
            var location = new Location { Id = Guid.NewGuid(), City = "TestCity" };
            var user = new ApplicationUser { Id = "userId" };
            var userProfile = new UserProfile { UserId = user.Id};

            db.Categories.Add(category);
            db.Locations.Add(location);
            db.Users.Add(user);
            db.UserProfiles.Add(userProfile);

            await db.SaveChangesAsync();

            var dto = new CreateEventDto
            {
                CategoryId = category.Id,
                LocationId = location.Id,
                Title = "Test Event",
                Description = "Test Description",
                Address = "Test Address",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                MaxParticipants = 10,
                ImagePath = "test.jpg"
            };

            await service.CreateAsync(dto, "userId", CancellationToken.None);

            var savedEvent = await db.Events.FirstOrDefaultAsync();

            Assert.NotNull(savedEvent);
            Assert.Equal("userId", savedEvent.OrganizerId);
        }
    }
}
