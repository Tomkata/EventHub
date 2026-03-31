
namespace EventHub.Tests.Services;

using Xunit;
using AutoMapper;
using EventHub.Core.DTOs.Event;
using EventHub.Core.Exceptions.Event.ForJoin;
using EventHub.Core.Exceptions.Event.ForLeft;
using EventHub.Core.Exceptions.User;
using EventHub.Core.Models.Events;
using EventHub.Core.Models.Users;
using EventHub.Infrastructure;
using EventHub.Infrastructure.Data;
using EventHub.Infrastructure.Identity;
using EventHub.Repositories.Interfaces.Events;
using EventHub.Repositories.Interfaces.User;
using EventHub.Services.Services.Event;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

public class ParticipantServiceTests : IDisposable
{
    private readonly Mock<IEventRepository> _eventRepoMock = new();
    private readonly Mock<IEventParticipantsRepository> _participantsRepoMock = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserProfileRepository> _userProfileRepoMock = new();
    private readonly ApplicationDbContext _dbContext;

    private static readonly Guid EventId = Guid.NewGuid();
    private const string UserId = "user-111";
    private const string OrganizerId = "organizer-222";

    private readonly ParticipantService _sut;

    public ParticipantServiceTests()
    {
        _userManagerMock = CreateUserManagerMock();

        var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new ApplicationDbContext(opts);

        _sut = new ParticipantService(
            _participantsRepoMock.Object,
            _eventRepoMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object,
            _userProfileRepoMock.Object,
            _dbContext);
    }

    public void Dispose() => _dbContext.Dispose();


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

    private static EventJoinInfo BuildJoinInfo(
        string? organizerId = null,
        DateTime? endDate = null,
        int maxParticipants = 100)
        => new EventJoinInfo
        {
            Id = EventId,
            OrganizerId = organizerId ?? OrganizerId,
            EndDate = endDate ?? DateTime.UtcNow.AddDays(7),
            MaxParticipantsCount = maxParticipants,
            ParticipantsCount = 0
        };

    private void SetupUserWithRoles(string userId, params string[] roles)
    {
        var user = new ApplicationUser { Id = userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles.ToList());
    }

    private void SetupValidProfile(string userId)
        => _userProfileRepoMock
            .Setup(r => r.GetByUserIdAsync(userId, default))
            .ReturnsAsync(new UserProfile { UserId = userId });

    // JoinEventAsync
    [Fact]
    public async Task JoinEventAsync_ValidRegularUser_JoinsSuccessfully()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        SetupUserWithRoles(UserId, Roles.User);
        SetupValidProfile(UserId);
        _eventRepoMock.Setup(r => r.TryJoinAsync(EventId, UserId, default)).ReturnsAsync(true);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().NotThrowAsync();
        _eventRepoMock.Verify(r => r.TryJoinAsync(EventId, UserId, default), Times.Once);
    }

    [Fact]
    public async Task JoinEventAsync_OrganizerJoinsAnotherOrganizersEvent_JoinsSuccessfully()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default))
            .ReturnsAsync(BuildJoinInfo(organizerId: "different-organizer-999"));
        SetupUserWithRoles(UserId, Roles.Organizer);
        SetupValidProfile(UserId);
        _eventRepoMock.Setup(r => r.TryJoinAsync(EventId, UserId, default)).ReturnsAsync(true);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task JoinEventAsync_EventNotFound_ThrowsEventNotFoundException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default))
            .ReturnsAsync((EventJoinInfo?)null);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<EventNotFoundException>();
    }

    [Fact]
    public async Task JoinEventAsync_UserNotFound_ThrowsUserNotFoundException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        _userManagerMock.Setup(m => m.FindByIdAsync(UserId)).ReturnsAsync((ApplicationUser?)null);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task JoinEventAsync_AdminUser_ThrowsAdminCannotJoinEventException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        SetupUserWithRoles(UserId, Roles.Admin);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<AdminCannotJoinEventException>();
    }

    [Fact]
    public async Task JoinEventAsync_OrganizerJoinsOwnEvent_ThrowsOrganizerJoinOwnEventException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default))
            .ReturnsAsync(BuildJoinInfo(organizerId: UserId));
        SetupUserWithRoles(UserId, Roles.Organizer);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<OrganizerJoinOwnEventException>();
    }

    [Fact]
    public async Task JoinEventAsync_UserHasNoProfile_ThrowsUserDontHavePrfileException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        SetupUserWithRoles(UserId, Roles.User);
        _userProfileRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((UserProfile?)null);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<UserDontHavePrfileException>();
    }

    [Fact]
    public async Task JoinEventAsync_EventExpired_ThrowsEventExpiredException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default))
            .ReturnsAsync(BuildJoinInfo(endDate: DateTime.UtcNow.AddDays(-1)));
        SetupUserWithRoles(UserId, Roles.User);
        SetupValidProfile(UserId);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<EventExpiredException>();
    }

    [Fact]
    public async Task JoinEventAsync_EventAtCapacity_ThrowsEventFilledException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        SetupUserWithRoles(UserId, Roles.User);
        SetupValidProfile(UserId);
        _eventRepoMock.Setup(r => r.TryJoinAsync(EventId, UserId, default)).ReturnsAsync(false);

        var act = async () => await _sut.JoinEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<EventFilledException>();
    }

    // LeftEventAsync

    [Fact]
    public async Task LeftEventAsync_UserIsParticipant_LeavesAndSavesChanges()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        _participantsRepoMock.Setup(r => r.ExistsAsync(UserId, EventId, default)).ReturnsAsync(true);

        await _sut.LeftEventAsync(UserId, EventId, default);

        _participantsRepoMock.Verify(r => r.RemoveParticipantFromEventAsync(UserId, EventId, default), Times.Once);
        _participantsRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task LeftEventAsync_EventNotFound_ThrowsEventNotFoundException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default))
            .ReturnsAsync((EventJoinInfo?)null);

        var act = async () => await _sut.LeftEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<EventNotFoundException>();
    }

    [Fact]
    public async Task LeftEventAsync_UserNotParticipant_ThrowsUserNotParticipantException()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        _participantsRepoMock.Setup(r => r.ExistsAsync(UserId, EventId, default)).ReturnsAsync(false);

        var act = async () => await _sut.LeftEventAsync(UserId, EventId, default);

        await act.Should().ThrowAsync<UserNotParticipantException>();
    }

    [Fact]
    public async Task LeftEventAsync_UserIsNotParticipant_NeverCallsRemove()
    {
        _eventRepoMock.Setup(r => r.GetEventJoinInfoAsync(EventId, default)).ReturnsAsync(BuildJoinInfo());
        _participantsRepoMock.Setup(r => r.ExistsAsync(UserId, EventId, default)).ReturnsAsync(false);

        try { await _sut.LeftEventAsync(UserId, EventId, default); } catch { }

        _participantsRepoMock.Verify(
            r => r.RemoveParticipantFromEventAsync(It.IsAny<string>(), It.IsAny<Guid>(), default),
            Times.Never);
    }
}
