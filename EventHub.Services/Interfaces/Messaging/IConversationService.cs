
using EventHub.Core.DTOs.Messaging;

namespace EventHub.Services.Interfaces.Messaging
{
    public interface IConversationService
    {
        public Task<Guid> GetOrCreateConversationAsync(
            Guid userAId,
            Guid userBId,
            CancellationToken cancellationToken);

        public Task<IEnumerable<ConversationPreviewDto>> GetUserConversationsAsync(
            string userId,
            CancellationToken cancellationToken
            );

        //Later
        //public Task ArchiveConversation(Guid conversationId);

        
    }
}
