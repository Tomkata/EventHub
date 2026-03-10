

namespace EventHub.Services.Interfaces.Messaging
{
using EventHub.Core.DTOs.Messaging;
    using EventHub.Services.Common;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public interface IConversationService
    {
        public Task<Guid> GetOrCreateConversationAsync(
            string userAId,
            string userBId,
            CancellationToken cancellationToken);

        public Task<PagedResult<ConversationPreviewDto>> GetUserConversationsAsync(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken
            );

        public Task<bool> IsUserParticipantAsync(
            Guid conversationId,
            string userId);

        Task<ConversationPreviewDto?> GetConversationInfoAsync(Guid conversationId, string userId, CancellationToken cancellationToken);


        //Later
        //public Task ArchiveConversation(Guid conversationId);


    }
}
