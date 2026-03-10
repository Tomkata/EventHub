using EventHub.Core.Models.Messaging;
using EventHub.Infrastructure.Data;
using EventHub.Repositories.Interfaces.Messaging;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repositories.Repositories.Messaging
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Message message)
            => await _context.Messages.AddAsync(message);

        public async Task DeleteAsync(Message message)
        {
            _context.Messages.Remove(message);
            await Task.CompletedTask;
        }

        public IQueryable<Message> GetAllByConversationReadOnly(Guid conversationId)
            => _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .AsNoTracking();

        public async Task<Message?> GetAsync(Guid id)
            => await _context.Messages.FindAsync(new object[] { id });

        public async Task MarkAsReadAsync(
            Guid conversationId, 
            string userId,
            CancellationToken cancellationToken)
        {
            await _context.Messages
               .Where(x => x.IsRead == false &&
               x.ConversationId == conversationId &&
               x.SenderId != userId)
               .ExecuteUpdateAsync(x => x.SetProperty(x => x.IsRead, true), cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
