/*
 * ConversationServiceTests
 * ────────────────────────
 * Tests the GetOrCreateConversationAsync method which contains the most
 * interesting business logic in the messaging system:
 *
 *   1. Self-conversation guard
 *   2. Returns existing conversation ID without creating a new one
 *   3. Creates a new conversation when none exists
 *   4. Normalizes user ID pair alphabetically so (A,B) and (B,A) always
 *      resolve to the same stored record — this is the key invariant that
 *      prevents duplicate conversations.
 */

namespace EventHub.Tests.Services;

using Xunit;
using EventHub.Core.Exceptions.Messaging;
using EventHub.Core.Models.Messaging;
using EventHub.Repositories.Interfaces.Messaging;
using EventHub.Services.Services.Messaging;
using FluentAssertions;
using Moq;

public class ConversationServiceTests
{
    // Two user IDs chosen so that string.CompareOrdinal(A, B) < 0
    private const string UserAId = "aaa-alpha";   // comes first 
    private const string UserBId = "zzz-omega";   // comes second 

    private readonly Mock<IConversationRepository> _repoMock = new();
    private readonly ConversationService _sut;

    public ConversationServiceTests()
    {
        _sut = new ConversationService(_repoMock.Object, mapper: null!);
    }


    private static Conversation MakeConversation(string user1 = UserAId, string user2 = UserBId)
        => new Conversation { Id = Guid.NewGuid(), User1Id = user1, User2Id = user2 };

    // GetOrCreateConversationAsync — guard clauses

    [Fact]
    public async Task GetOrCreate_SameUserId_ThrowsSelfConversationNotAllowedException()
    {
        var act = async () => await _sut.GetOrCreateConversationAsync(UserAId, UserAId, default);

        await act.Should().ThrowAsync<SelfConversationNotAllowedException>();
        _repoMock.Verify(r => r.GetByUsersAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
    }

    // GetOrCreateConversationAsync — existing conversation

    [Fact]
    public async Task GetOrCreate_ConversationAlreadyExists_ReturnsExistingIdWithoutCreating()
    {
        var existing = MakeConversation();
        _repoMock.Setup(r => r.GetByUsersAsync(UserAId, UserBId, default)).ReturnsAsync(existing);

        var result = await _sut.GetOrCreateConversationAsync(UserAId, UserBId, default);

        result.Should().Be(existing.Id);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Conversation>(), default), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    // GetOrCreateConversationAsync — creation

    [Fact]
    public async Task GetOrCreate_NoExistingConversation_CreatesAndReturnsNewId()
    {
        _repoMock.Setup(r => r.GetByUsersAsync(UserAId, UserBId, default))
            .ReturnsAsync((Conversation?)null);

        var result = await _sut.GetOrCreateConversationAsync(UserAId, UserBId, default);

        result.Should().NotBeEmpty();
        _repoMock.Verify(r => r.AddAsync(It.Is<Conversation>(c =>
            c.User1Id == UserAId && c.User2Id == UserBId), default), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    // GetOrCreateConversationAsync — normalization invariant

    [Fact]
    public async Task GetOrCreate_ReversedInputOrder_NormalizesAndQueriesSamePair()
    {
        var existing = MakeConversation(UserAId, UserBId);
        _repoMock.Setup(r => r.GetByUsersAsync(UserAId, UserBId, default)).ReturnsAsync(existing);

        var result = await _sut.GetOrCreateConversationAsync(UserBId, UserAId, default);

        result.Should().Be(existing.Id);
        _repoMock.Verify(r => r.GetByUsersAsync(UserAId, UserBId, default), Times.Once);
        _repoMock.Verify(r => r.GetByUsersAsync(UserBId, UserAId, default), Times.Never);
    }

    [Fact]
    public async Task GetOrCreate_StoredConversationAlwaysHasUser1BeforeUser2Alphabetically()
    {
        _repoMock.Setup(r => r.GetByUsersAsync(UserAId, UserBId, default))
            .ReturnsAsync((Conversation?)null);

        Conversation? captured = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Conversation>(), default))
            .Callback<Conversation, CancellationToken>((c, _) => captured = c);

        await _sut.GetOrCreateConversationAsync(UserBId, UserAId, default);

        captured.Should().NotBeNull();
        captured!.User1Id.Should().Be(UserAId);
        captured.User2Id.Should().Be(UserBId);
    }

    [Fact]
    public async Task GetOrCreate_ForwardAndReverseOrder_BothReturnSameConversationId()
    {
        var existing = MakeConversation(UserAId, UserBId);
        _repoMock.Setup(r => r.GetByUsersAsync(UserAId, UserBId, default)).ReturnsAsync(existing);

        var resultForward = await _sut.GetOrCreateConversationAsync(UserAId, UserBId, default);
        var resultReversed = await _sut.GetOrCreateConversationAsync(UserBId, UserAId, default);

        resultForward.Should().Be(resultReversed);
    }

    // IsUserParticipantAsync
    [Fact]
    public async Task IsUserParticipant_UserIsInConversation_ReturnsTrue()
    {
        var conversationId = Guid.NewGuid();
        _repoMock.Setup(r => r.IsUserParticipantAsync(conversationId, UserAId)).ReturnsAsync(true);

        var result = await _sut.IsUserParticipantAsync(conversationId, UserAId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsUserParticipant_UserIsNotInConversation_ReturnsFalse()
    {
        var conversationId = Guid.NewGuid();
        _repoMock.Setup(r => r.IsUserParticipantAsync(conversationId, "outsider")).ReturnsAsync(false);

        var result = await _sut.IsUserParticipantAsync(conversationId, "outsider");

        result.Should().BeFalse();
    }
}
