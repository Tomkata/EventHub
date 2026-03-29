


namespace EventHub.Tests
{
    using EventHub.Core.DTOs.Organizer;
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Models.Organizer;
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure;
    using EventHub.Infrastructure.Identity;
    using EventHub.Repositories.Interfaces.Organizer;
    using EventHub.Repositories.Interfaces.User;
    using EventHub.Services.Services.User;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;


    public class OrganizerServiceTests
    {
        private readonly Mock<IOrganizerRequestRepository> _requestRepoMock = new();
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IUserProfileRepository> _userProfileRepoMock = new();

        private const string UserId = "user-111";


        private readonly OrganizerService _sut;

        public OrganizerServiceTests()
        {
            _userManagerMock = CreateUserManagerMock();

            _sut = new OrganizerService(
                _requestRepoMock.Object,
                _userManagerMock.Object,
                mapper: null!,
                _userProfileRepoMock.Object);
        }

        private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());
        }

        private static OrganizerRequestFormDto MakeForm()
        => new OrganizerRequestFormDto { Email = "applicant@test.com", Note = "I want to organize!" };

        private static OrganizerRequest MakeRequest(Status status, DateTime? lastRejectedAt = null)
            => new OrganizerRequest
            {
                UserId = UserId,
                Status = status,
                Email = "applicant@test.com",
                LastRejectedAt = lastRejectedAt
            };


        private void SetupUserWithRoles(string userId, params string[] roles)
        {
            var user = new ApplicationUser { Id = userId };
            _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles.ToList());
            _userManagerMock
                .Setup(m => m.AddToRoleAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(m => m.RemoveFromRoleAsync(user, It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
        }

        private void SetupValidProfile()
        => _userProfileRepoMock
            .Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(new UserProfile { UserId = UserId });
    

    [Fact]
        public async Task Apply_NewUserWithProfile_CreatesNewPendingRequest()
        {
            _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
                .ReturnsAsync((OrganizerRequest?)null);
            SetupUserWithRoles(UserId, Roles.User);
            SetupValidProfile();

            await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

            _requestRepoMock.Verify(r => r.AddAsync(
                It.Is<OrganizerRequest>(req =>
                    req.UserId == UserId &&
                    req.Status == Status.Pending),
                default), Times.Once);
            _requestRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    } 
}


