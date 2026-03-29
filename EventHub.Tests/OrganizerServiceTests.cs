namespace EventHub.Tests.Services;

using Xunit;
using EventHub.Core.DTOs.Organizer;
using EventHub.Core.Enums.Organizer;
using EventHub.Core.Exceptions.Event.ForJoin;
using EventHub.Core.Exceptions.Oranizer.ForApply;
using EventHub.Core.Exceptions.Oranizer.ForApprove;
using EventHub.Core.Exceptions.Oranizer.ForDemote;
using EventHub.Core.Exceptions.Oranizer.ForReject;
using EventHub.Core.Exceptions.User;
using EventHub.Core.Models.Organizer;
using EventHub.Core.Models.Users;
using EventHub.Infrastructure;
using EventHub.Infrastructure.Identity;
using EventHub.Repositories.Interfaces.Organizer;
using EventHub.Repositories.Interfaces.User;
using EventHub.Services.Services.User;
using FluentAssertions;
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

    [Fact]
    public async Task Apply_AlreadyApproved_ThrowsUserAlreadyOrganizerException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Approved));

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<UserAlreadyOrganizerException>();
    }

    [Fact]
    public async Task Apply_PendingRequestExists_ThrowsOrganizerRequestPendingException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Pending));

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestPendingException>();
    }

    [Fact]
    public async Task Apply_RejectedWithinCooldown_ThrowsOrganizerCooldownNotExpiredException()
    {
        var rejectedAt = DateTime.UtcNow.AddDays(-3);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Rejected, lastRejectedAt: rejectedAt));

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<OrganizerCooldownNotExpiredException>();
    }

    [Fact]
    public async Task Apply_RejectedCooldownExpired_ReusesExistingRequestSetsToPending()
    {
        var rejectedAt = DateTime.UtcNow.AddDays(-8);
        var existingRequest = MakeRequest(Status.Rejected, lastRejectedAt: rejectedAt);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(existingRequest);

        await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        existingRequest.Status.Should().Be(Status.Pending);
        _requestRepoMock.Verify(r => r.AddAsync(It.IsAny<OrganizerRequest>(), default), Times.Never);
        _requestRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Apply_CooldownExactlyOnBoundary_StillBlocked()
    {
        var rejectedAt = DateTime.UtcNow.AddDays(-7).AddHours(1);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Rejected, lastRejectedAt: rejectedAt));

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<OrganizerCooldownNotExpiredException>();
    }

    [Fact]
    public async Task Apply_AdminUser_ThrowsAdminCannotApplyException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);
        SetupUserWithRoles(UserId, Roles.Admin);

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<AdminCannotApplyException>();
    }

    [Fact]
    public async Task Apply_UserHasNoProfile_ThrowsCreateProfileApplyException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);
        SetupUserWithRoles(UserId, Roles.User);
        _userProfileRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((UserProfile?)null);

        var act = async () => await _sut.ApplyForOrganizerAsync(MakeForm(), UserId, default);

        await act.Should().ThrowAsync<CreateProfileApplyException>();
    }

    [Fact]
    public async Task Approve_PendingRequest_SetsStatusApprovedAndAssignsRole()
    {
        var request = MakeRequest(Status.Pending);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default)).ReturnsAsync(request);
        SetupUserWithRoles(UserId, Roles.User);

        await _sut.ApproveUserToOrganizerAsync(UserId, default);

        request.Status.Should().Be(Status.Approved);
        _userManagerMock.Verify(m => m.AddToRoleAsync(
            It.Is<ApplicationUser>(u => u.Id == UserId),
            Roles.Organizer),
            Times.Once);
        _requestRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Approve_RequestNotFound_ThrowsOrganizerRequestNotFoundException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);

        var act = async () => await _sut.ApproveUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestNotFoundException>();
    }

    [Fact]
    public async Task Approve_AlreadyApproved_ThrowsUserAlreadyOrganizerException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Approved));

        var act = async () => await _sut.ApproveUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<UserAlreadyOrganizerException>();
    }

    [Fact]
    public async Task Approve_RejectedRequest_ThrowsApproveRejectedUserException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Rejected));

        var act = async () => await _sut.ApproveUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<ApproveRejectedUserException>();
    }

    [Fact]
    public async Task Reject_PendingRequest_SetsStatusRejectedAndRecordsTimestamp()
    {
        var request = MakeRequest(Status.Pending);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default)).ReturnsAsync(request);
        var before = DateTime.UtcNow;

        await _sut.RejectUserToOrganizerAsync(UserId, default);

        request.Status.Should().Be(Status.Rejected);
        request.LastRejectedAt.Should().NotBeNull();
        request.LastRejectedAt!.Value.Should().BeOnOrAfter(before);
        _requestRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Reject_RequestNotFound_ThrowsOrganizerRequestNotFoundException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);

        var act = async () => await _sut.RejectUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestNotFoundException>();
    }

    [Fact]
    public async Task Reject_AlreadyApproved_ThrowsOrganizerRequestAlreadyApprovedException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Approved));

        var act = async () => await _sut.RejectUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestAlreadyApprovedException>();
    }

    [Fact]
    public async Task Reject_AlreadyRejected_ThrowsOrganizerRequestAlreadyRejectedException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Rejected));

        var act = async () => await _sut.RejectUserToOrganizerAsync(UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestAlreadyRejectedException>();
    }

    [Fact]
    public async Task Demote_ApprovedOrganizer_RemovesRoleAndSetsRejected()
    {
        var request = MakeRequest(Status.Approved);
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default)).ReturnsAsync(request);
        SetupUserWithRoles(UserId, Roles.Organizer);

        await _sut.DemoteOrganizerToUserAsync(UserId, default);

        request.Status.Should().Be(Status.Rejected);
        _userManagerMock.Verify(m => m.RemoveFromRoleAsync(
            It.Is<ApplicationUser>(u => u.Id == UserId),
            Roles.Organizer),
            Times.Once);
    }

    [Fact]
    public async Task Demote_RequestNotFound_ThrowsOrganizerRequestNotFoundException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);

        var act = async () => await _sut.DemoteOrganizerToUserAsync(UserId, default);

        await act.Should().ThrowAsync<OrganizerRequestNotFoundException>();
    }

    [Fact]
    public async Task Demote_PendingRequest_ThrowsDemotePendingRequestException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Pending));

        var act = async () => await _sut.DemoteOrganizerToUserAsync(UserId, default);

        await act.Should().ThrowAsync<DemotePendingRequestException>();
    }

    [Fact]
    public async Task Demote_AlreadyRejected_ThrowsDemoteRejectedException()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(Status.Rejected));

        var act = async () => await _sut.DemoteOrganizerToUserAsync(UserId, default);

        await act.Should().ThrowAsync<DemoteRejectedException>();
    }

    [Fact]
    public async Task GetOrganizerState_NoRequestExists_ReturnsNone()
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync((OrganizerRequest?)null);

        var result = await _sut.GetOrganizerStateAsync(UserId, default);

        result.Should().Be(Status.None);
    }

    [Theory]
    [InlineData(Status.Pending)]
    [InlineData(Status.Approved)]
    [InlineData(Status.Rejected)]
    public async Task GetOrganizerState_ExistingRequest_ReturnsItsStatus(Status status)
    {
        _requestRepoMock.Setup(r => r.GetByUserIdAsync(UserId, default))
            .ReturnsAsync(MakeRequest(status));

        var result = await _sut.GetOrganizerStateAsync(UserId, default);

        result.Should().Be(status);
    }
}