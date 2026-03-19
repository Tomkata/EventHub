

namespace EventHub.Tests
{
    using AutoMapper;
    using EventHub.Core.DTOs;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Models.Common;
    using EventHub.Core.Models.Events;
    using EventHub.Core.Models.Users;
    using EventHub.Repositories.Interfaces.Common;
    using EventHub.Repositories.Interfaces.Events;
    using EventHub.Repositories.Interfaces.User;
    using EventHub.Services.Services.Event;
    using Moq;

    public class EventServiceTests
    {
        [Fact]
        public async Task CreateAsync_WhenCategoryIsInvalid_ShouldThrowInvalidCategoryException()
        {
            var eventRepo = new Mock<IEventRepository>();
            var participantsRepo = new Mock<IEventParticipantsRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var userProfileRepo = new Mock<IUserProfileRepository>();
            var mapper = new Mock<IMapper>();


            categoryRepo
                .Setup( r =>  r.GetByIdAsync(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);

            locationRepo
     .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
     .ReturnsAsync(new Location());

            participantsRepo
                .Setup(r => r.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserBasicInfo());

            userProfileRepo
                .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

          

            var service = new EventService(
                eventRepo.Object,
                participantsRepo.Object,
                categoryRepo.Object,
                locationRepo.Object,
                mapper.Object,
                userProfileRepo.Object
                );
           

            var dto = new CreateEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<InvalidCategoryException>(()=>
            service.CreateAsync(dto, "userId",CancellationToken.None));

            eventRepo.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
