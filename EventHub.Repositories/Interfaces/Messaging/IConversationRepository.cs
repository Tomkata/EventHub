
using EventHub.Core.Models.Messaging;

namespace EventHub.Repositories.Interfaces.Messaging
{
    public interface IConversationRepository
    {
        public IQueryable<Conversation> GetAll();
        public Task<Conversation?> GetAsync(Guid id, CancellationToken cancellationToken);

        public Task<Conversation?> GetByUsersAsync(
      string user1Id,
      string user2Id,
      CancellationToken cancellationToken);

        public IQueryable<Conversation> GetAllByUser(string userId);

        public Task AddAsync(Conversation conversation, CancellationToken cancellationToken);

        public Task<bool> ExistsBetweenUsersAsync(
            string user1Id,
            string user2Id,
            CancellationToken cancellationToken);

        public Task<int> GetUnreadConversationsCountAsync(string userId,CancellationToken cancellationToken);

        public Task<bool> IsUserParticipantAsync(Guid conversationId, string userId);

        public Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
