
using EventHub.Core.Models.Messaging;

namespace EventHub.Repositories.Interfaces.Messaging
{
    public interface IMessageRepository
    {
        public Task AddAsync(Message message,CancellationToken cancellationToken);

        public Task DeleteAsync(Message message, CancellationToken cancellationToken);

        public IQueryable<Message> GetAllByConversationReadOnly(Guid conversationId);

        Task<Message?> GetAsync(Guid id, CancellationToken cancellationToken);

        public Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
