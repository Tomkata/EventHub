
namespace EventHub.Services.Services.Messaging
{
    using EventHub.Core.DTOs.Messaging;
    using EventHub.Core.Exceptions.Messaging;
    using EventHub.Core.Models.Messaging;
    using EventHub.Repositories.Interfaces.Messaging;
    using EventHub.Services.Interfaces.Messaging;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using System.Diagnostics.Eventing.Reader;

    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            this._conversationRepository = conversationRepository;
        }

        public async Task<Guid> GetOrCreateConversationAsync(
            string user1Id,
            string user2Id, 
            CancellationToken cancellationToken)
        {

            if (user1Id == user2Id)
                throw new SelfConversationNotAllowedException();



            // Normalize user ids so the conversation pair is always stored in the same order.
            // This prevents duplicate conversations like (A,B) and (B,A) because the database
            // has a unique index on (User1Id, User2Id).
            var userAId = string.Empty;
            var userBId = string.Empty;

            if (String.CompareOrdinal(user1Id,user2Id) < 0)
            {
                userAId = user1Id;
                userBId = user2Id;
            }
            else
            {
                userAId = user2Id;
                userBId = user1Id;
            }

            try
            {
                var getOrCreateConversation = await _conversationRepository
                                 .GetByUsersAsync(userAId, userBId, cancellationToken);

                if (getOrCreateConversation == null)
                {
                    var conversation = new Conversation
                    {
                        User1Id = userAId,
                        User2Id = userBId
                    };

                    await _conversationRepository.AddAsync(conversation,cancellationToken);
                    await _conversationRepository.SaveChangesAsync(cancellationToken);
                    return conversation.Id;
                }

                return getOrCreateConversation.Id;

            }
            catch (DbUpdateException ex)
            when (IsUniqueViolation(ex))
            {
                var existingConversation = await _conversationRepository
                                  .GetByUsersAsync(userAId, userBId, cancellationToken)
                                  ?? throw new InvalidConversationException();

                return existingConversation.Id;
            }
        }

        public Task<IEnumerable<ConversationPreviewDto>> GetUserConversationsAsync(
            string userId, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static bool IsUniqueViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
                return sqlEx.Number is 2627 or 2601;

            return false;
        }
    }
}
