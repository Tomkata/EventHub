
namespace EventHub.Repositories.Repositories.Messaging
{


    using EventHub.Core.Models.Messaging;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.Messaging;
    using Microsoft.EntityFrameworkCore;

    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ConversationRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Conversations.AddAsync(conversation, cancellationToken);
        }

        public async Task<bool> ExistsBetweenUsersAsync(string user1Id, string user2Id, CancellationToken cancellationToken)
      => await _applicationDbContext.Conversations
            .AsNoTracking()
            .AnyAsync(x => x.User1Id == user1Id && x.User2Id == user2Id,cancellationToken);

        public IQueryable<Conversation> GetAll()
       => _applicationDbContext.Conversations
            .AsNoTracking()
            .AsQueryable();

        public async Task<Conversation?> GetAsync(Guid id, CancellationToken cancellationToken)
        => await _applicationDbContext.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Conversation?> GetByUsersAsync(string user1Id, string user2Id, CancellationToken cancellationToken)
        => await _applicationDbContext.Conversations
                .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.User1Id == user1Id && x.User2Id == user2Id,cancellationToken);

        public async Task<bool> IsUserParticipantAsync(Guid conversationId, string userId)
        => await _applicationDbContext.Conversations
            .AsNoTracking()
            .AnyAsync(x => x.Id == conversationId &&
            (x.User1Id == userId || x.User2Id == userId));

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
