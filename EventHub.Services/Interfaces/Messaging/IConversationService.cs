

namespace EventHub.Services.Interfaces.Messaging
{
using EventHub.Core.DTOs.Messaging;
    public interface IConversationService
    {
        public Task<Guid> GetOrCreateConversationAsync(
            string userAId,
            string userBId,
            CancellationToken cancellationToken);

        public Task<IEnumerable<ConversationPreviewDto>> GetUserConversationsAsync(
            string userId,
            CancellationToken cancellationToken
            );

        //Later
        //public Task ArchiveConversation(Guid conversationId);

        
    }
}
