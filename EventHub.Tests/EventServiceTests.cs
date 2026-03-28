

namespace EventHub.Tests
{
    using AutoMapper;
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Exceptions.Category;
    using EventHub.Core.Exceptions.Event;
    using EventHub.Core.Exceptions.Location;
    using EventHub.Core.Exceptions.User;
    using EventHub.Core.Exceptions.UserProfile;
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
        //Create tests
        [Fact]
        public async Task CreateAsync_WhenCategoryIsInvalid_ShouldThrowInvalidCategoryException()
        {
            var categoryRepo = new Mock<ICategoryRepository>();
            categoryRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);

            var eventRepo = new Mock<IEventRepository>();
            var service = CreateService(categoryRepo: categoryRepo, eventRepo: eventRepo);

            await Assert.ThrowsAsync<InvalidCategoryException>(() =>
                service.CreateAsync(NewDto(), "userId", CancellationToken.None));

            VerifyNotSaved(eventRepo);
        }

        [Fact]
        public async Task CreateAsync_WhenLocationIsInvalid_ShouldThrowInvalidLocationException()
        {
            var locationRepo = new Mock<ILocationRepository>();
            locationRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Location)null);

            var eventRepo = new Mock<IEventRepository>();
            var service = CreateService(locationRepo: locationRepo, eventRepo: eventRepo);

            await Assert.ThrowsAsync<InvalidLocationException>(() =>
                service.CreateAsync(NewDto(), "userId", CancellationToken.None));

            VerifyNotSaved(eventRepo);
        }

        [Fact]
        public async Task CreateAsync_WhenUserIsNotFound_ShouldThrowUserNotFoundException()
        {
            var participantsRepo = new Mock<IEventParticipantsRepository>();
            participantsRepo
                .Setup(r => r.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserBasicInfo)null);

            var eventRepo = new Mock<IEventRepository>();
            var service = CreateService(participantsRepo: participantsRepo, eventRepo: eventRepo);

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                service.CreateAsync(NewDto(), "userId", CancellationToken.None));

            VerifyNotSaved(eventRepo);
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsValid_ShouldCreateEvent()
        {
            var eventRepo = new Mock<IEventRepository>();
            var service = CreateService(eventRepo: eventRepo);

            await service.CreateAsync(NewDto(), "userId", CancellationToken.None);

            eventRepo.Verify(r => r.AddAsync(
                It.Is<Event>(e => e.OrganizerId == "userId"),
                It.IsAny<CancellationToken>()), Times.Once);

            eventRepo.Verify(r => r.SaveChangesAsync(
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenProfileDoesNotExist_ShouldThrowProfileNotFoundException()
        {
            var userProfileRepo = new Mock<IUserProfileRepository>();
            userProfileRepo
                .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var eventRepo = new Mock<IEventRepository>();
            var service = CreateService(userProfileRepo: userProfileRepo, eventRepo: eventRepo);

            await Assert.ThrowsAsync<ProfileNotFoundException>(() =>
                service.CreateAsync(NewDto(), "userId", CancellationToken.None));

            VerifyNotSaved(eventRepo);
        }


        //Update tests

        [Fact]
        public async Task UpdateAsync_WhenEventDoesNotExist_ShouldThrowEventNotFoundException()
        {
            var eventRepo = new Mock<IEventRepository>();

            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var service = CreateService(eventRepo: eventRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<EventNotFoundException>(() =>
                service.UpdateAsync(Guid.Empty, dto, "userId", true,CancellationToken.None))  ;

            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.VerifyNoOtherCalls();
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsNorOrganizer_ShouldThrowForbiddenOperationException()
        {
            var eventRepo = new Mock<IEventRepository>();

            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event
                {
                    OrganizerId = "ownerId"
                });

            var service = CreateService(eventRepo: eventRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<ForbiddenOperationException>(() =>
                service.UpdateAsync(Guid.Empty, dto, "anotherUser", false, CancellationToken.None));

            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsOrganizer_ShouldUpdateEventSuccessfully()
        {
            var eventRepo = new Mock<IEventRepository>();

            var existingEvent = new Event { OrganizerId = "ownerId" };
            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            var service = CreateService(eventRepo: eventRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };

            
             await service.UpdateAsync(Guid.Empty, dto, "ownerId", false, CancellationToken.None);

            Assert.Equal(dto.CategoryId, existingEvent.CategoryId);

            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsAdmin_ShouldUpdateEventSuccessfully()
        {
            var eventRepo = new Mock<IEventRepository>();

            var existingEvent = new Event { OrganizerId = "realOwner" };
            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            var service = CreateService(eventRepo: eventRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };


            await service.UpdateAsync(Guid.Empty, dto, "anotherUser", true, CancellationToken.None);

            Assert.Equal(dto.CategoryId, existingEvent.CategoryId);

            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryIsInvalid_ShouldThrowInvalidCategoryException()
        {
            var eventRepo = new Mock<IEventRepository>();
            var categoryRepo = new Mock<ICategoryRepository>();

            var existingEvent = new Event { OrganizerId = "organizer" };
            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            categoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);



            var service = CreateService(eventRepo: eventRepo,categoryRepo:categoryRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };


           await Assert.ThrowsAsync<InvalidCategoryException>(()
                =>  service.UpdateAsync(Guid.Empty, dto, "organizer", false, CancellationToken.None));


            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task UpdateAsync_WhenLocationIsInvalid_ShouldThrowInvalidLocationException()
        {
            var eventRepo = new Mock<IEventRepository>();
            var locationRepo = new Mock<ILocationRepository>();

            var existingEvent = new Event { OrganizerId = "organizer" };
            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            locationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Location)null);



            var service = CreateService(eventRepo: eventRepo, locationRepo: locationRepo);

            var dto = new EditEventDto
            {
                CategoryId = Guid.NewGuid(),
                LocationId = Guid.NewGuid()
            };


            await Assert.ThrowsAsync<InvalidLocationException>(()
                 => service.UpdateAsync(Guid.Empty, dto, "organizer", false, CancellationToken.None));


            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenEventDoesNotExist_ShouldThrowEventNotFoundException()
        {
            var eventRepo = new Mock<IEventRepository>();


            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var service = CreateService(eventRepo:eventRepo);

            await Assert.ThrowsAsync<EventNotFoundException>(() => service.DeleteAsync(Guid.Empty,"user",false,CancellationToken.None));

            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            eventRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_WhenUserDoesNotHaveAuthorization_ShouldThrowForbiddenOperationException()
        {
            var eventRepo = new Mock<IEventRepository>();

            var existingEvent = new Event { OrganizerId = "organizer" };

            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            var service = CreateService(eventRepo: eventRepo);

            await Assert.ThrowsAsync<ForbiddenOperationException>(() => service.DeleteAsync(Guid.Empty, "user", false, CancellationToken.None));

            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            eventRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_WhenUserIsAdmin_ShouldBeSuccessful()
        {
            var eventRepo = new Mock<IEventRepository>();

            var existingEvent = new Event { OrganizerId = "owner" };

            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            var service = CreateService(eventRepo: eventRepo);

         await service.DeleteAsync(Guid.Empty, "user", true, CancellationToken.None);

            Assert.True(existingEvent.IsDeleted);
            Assert.NotNull(existingEvent.DeletedAt);

            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteAsync_WhenUserIsOwner_ShouldBeSuccessful()
        {
            var eventRepo = new Mock<IEventRepository>();

            var existingEvent = new Event { OrganizerId = "owner" };

            eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingEvent);

            var service = CreateService(eventRepo: eventRepo);

            await service.DeleteAsync(Guid.Empty, "owner", false, CancellationToken.None);

            Assert.True(existingEvent.IsDeleted);
            Assert.NotNull(existingEvent.DeletedAt);

            eventRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            eventRepo.VerifyNoOtherCalls();
        }




        //Helpers
        private static CreateEventDto NewDto() => new CreateEventDto
        {
            CategoryId = Guid.NewGuid(),
            LocationId = Guid.NewGuid()
        };

        private static void VerifyNotSaved(Mock<IEventRepository> eventRepo)
        {
            eventRepo.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
            eventRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private EventService CreateService(
    Mock<ICategoryRepository>? categoryRepo = null,
    Mock<ILocationRepository>? locationRepo = null,
    Mock<IEventParticipantsRepository>? participantsRepo = null,
    Mock<IUserProfileRepository>? userProfileRepo = null,
    Mock<IEventRepository>? eventRepo = null,
    Mock<IMapper>? mapper = null)
        {
            if (categoryRepo is null)
            {
                categoryRepo = new Mock<ICategoryRepository>();
                categoryRepo
                    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Category());
            }

            if (locationRepo is null)
            {
                locationRepo = new Mock<ILocationRepository>();
                locationRepo
                    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Location());
            }

            if (participantsRepo is null)
            {
                participantsRepo = new Mock<IEventParticipantsRepository>();
                participantsRepo
                    .Setup(r => r.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new UserBasicInfo());
            }

            if (userProfileRepo is null)
            {
                userProfileRepo = new Mock<IUserProfileRepository>();
                userProfileRepo
                    .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);
            }

            eventRepo ??= new Mock<IEventRepository>();

            if (mapper is null)
            {
                mapper = new Mock<IMapper>();
                mapper
                    .Setup(m => m.Map<Event>(It.IsAny<CreateEventDto>()))
                    .Returns(new Event());
            }

            return new EventService(
                eventRepo.Object,
                participantsRepo.Object,
                categoryRepo.Object,
                locationRepo.Object,
                mapper.Object,
                userProfileRepo.Object);
        }
    }
}

