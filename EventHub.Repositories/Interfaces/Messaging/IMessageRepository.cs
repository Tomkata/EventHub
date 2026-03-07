
using EventHub.Core.Models.Messaging;

namespace EventHub.Repositories.Interfaces.Messaging
{
    public interface IMessageRepository
    {
        public Task AddAsync(Message message);

        public Task DeleteAsync(Message message);

        public IQueryable<Message> GetAllByConversationReadOnly(Guid conversationId);   

        Task<Message?> GetAsync(Guid id);

        public Task SaveChangesAsync();
    }
}
