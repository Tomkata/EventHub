


namespace EventHub.Services.Interfaces.Messaging
{
using EventHub.Core.DTOs.Messaging;
    public interface IMessageService
    {
        public Task<Guid> SendMessageAsync(
            Guid conversationId, 
            string senderId,
            string messageContent,
            CancellationToken cancellationToken);

        public Task<MessageDto> GetConversationMessagesAsync(
            Guid conversationId,
            CancellationToken cancellationToken
            );

    }
}
