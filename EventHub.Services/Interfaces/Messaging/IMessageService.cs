


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


        Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(
             Guid conversationId,
             Guid? beforeMessageId,
             int pageSize);


        Task MarkAsReadAsync(
            Guid conversationId,
            string userId,
            CancellationToken cancellationToken);
    }
}
