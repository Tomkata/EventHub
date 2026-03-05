
using EventHub.Core.Models.Messaging;

namespace EventHub.Repositories.Interfaces.Messaging
{
    public interface IConversationRepository
    {
        public IQueryable<Conversation> GetAll();
        public Task<Conversation?> GetAsync(Guid id,CancellationToken cancellationToken);

        Task<Conversation?> GetByUsersAsync(
      string user1Id,
      string user2Id,
      CancellationToken cancellationToken);

        Task AddAsync(Conversation conversation, CancellationToken cancellationToken);

        Task<bool> ExistsBetweenUsersAsync(
            string user1Id,
            string user2Id,
            CancellationToken cancellationToken);

        Task<bool> IsUserParticipantAsync(Guid conversationId,string userId);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
