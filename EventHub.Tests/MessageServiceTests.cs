

namespace EventHub.Tests.Services;

using Xunit;
using EventHub.Core.Exceptions.Messaging;
using EventHub.Core.Models.Messaging;
using EventHub.Repositories.Interfaces.Messaging;
using EventHub.Services.Services.Messaging;
using FluentAssertions;
using Moq;

public class MessageServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepoMock = new();
    private readonly Mock<IMessageRepository>      _messageRepoMock      = new();
    private readonly MessageService                _sut;

    private static readonly Guid   ConversationId = Guid.NewGuid();
    private const           string SenderId       = "sender-111";



    public MessageServiceTests()
    {
        _sut = new MessageService(_conversationRepoMock.Object, _messageRepoMock.Object);
    }


    private void SetupParticipant(bool isParticipant = true)
        => _conversationRepoMock
            .Setup(r => r.IsUserParticipantAsync(ConversationId, SenderId))
            .ReturnsAsync(isParticipant);

    private Message? CaptureAddedMessage()
    {
      
        Message? captured = null;
        _messageRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => captured = m)
            .Returns(Task.CompletedTask);
        return captured; 
    }

    
    [Fact]
    public async Task SendMessage_ValidContent_ReturnsNonEmptyGuid()
    {
        SetupParticipant();

        var id = await _sut.SendMessageAsync(ConversationId, SenderId, "Hello!", default);

        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SendMessage_ValidContent_PersistsMessageWithCorrectFields()
    {
        SetupParticipant();
        Message? captured = null;
        _messageRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => captured = m)
            .Returns(Task.CompletedTask);

        await _sut.SendMessageAsync(ConversationId, SenderId, "Hello, world!", default);

        captured.Should().NotBeNull();
        captured!.Content.Should().Be("Hello, world!");
        captured.SenderId.Should().Be(SenderId);
        captured.ConversationId.Should().Be(ConversationId);
    }

    [Fact]
    public async Task SendMessage_ValidContent_CallsSaveChanges()
    {
        SetupParticipant();

        await _sut.SendMessageAsync(ConversationId, SenderId, "Hello!", default);

        _messageRepoMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    
    // authorization

    [Fact]
    public async Task SendMessage_SenderNotParticipant_ThrowsUserNotParticipantInConversationException()
    {
        SetupParticipant(isParticipant: false);

        var act = async () => await _sut.SendMessageAsync(ConversationId, SenderId, "Hello!", default);

        await act.Should().ThrowAsync<UserNotParticipantInConversationException>();
        _messageRepoMock.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
    }

    // SendMessageAsync — empty content validation

    [Fact]
    public async Task SendMessage_EmptyString_ThrowsMessageEmptyException()
    {
        SetupParticipant();

        var act = async () => await _sut.SendMessageAsync(ConversationId, SenderId, "", default);

        await act.Should().ThrowAsync<MessageEmptyException>();
    }

    [Fact]
    public async Task SendMessage_NullContent_ThrowsMessageEmptyException()
    {
        SetupParticipant();

        var act = async () => await _sut.SendMessageAsync(ConversationId, SenderId, null!, default);
        
        await act.Should().ThrowAsync<MessageEmptyException>();
    }

    [Fact]
    public async Task SendMessage_EmptyContent_NeverCallsAddAsync()
    {
        SetupParticipant();

        try { await _sut.SendMessageAsync(ConversationId, SenderId, "", default); } catch { }

        _messageRepoMock.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
    }

    // SendMessageAsync — whitespace trimming

    [Fact]
    public async Task SendMessage_ContentWithLeadingAndTrailingSpaces_StoresTrimmedContent()
    {
        // Arrange
        SetupParticipant();
        Message? captured = null;
        _messageRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => captured = m)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.SendMessageAsync(ConversationId, SenderId, "  hello  ", default);

        // Assert
        captured!.Content.Should().Be("hello");
    }

    [Fact]
    public async Task SendMessage_ContentWithInternalSpaces_PreservesInternalSpaces()
    {
        // Arrange
        SetupParticipant();
        Message? captured = null;
        _messageRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Message>()))
            .Callback<Message>(m => captured = m)
            .Returns(Task.CompletedTask);

        await _sut.SendMessageAsync(ConversationId, SenderId, "  hello world  ", default);

        captured!.Content.Should().Be("hello world");
    }

    // MarkAsReadAsync

    [Fact]
    public async Task MarkAsRead_CallsRepositoryWithCorrectParameters()
    {
        await _sut.MarkAsReadAsync(ConversationId, SenderId, default);

        _messageRepoMock.Verify(r => r.MarkAsReadAsync(ConversationId, SenderId, default), Times.Once);
    }
}
