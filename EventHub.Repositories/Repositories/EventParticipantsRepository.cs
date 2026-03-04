

namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Models;
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class EventParticipantsRepository : IEventParticipantsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventParticipantsRepository(ApplicationDbContext dbContext)
        {
           this._dbContext = dbContext;
        }

        public async Task<UserBasicInfo?> UserExistsAsync(string organizerId,CancellationToken cancellation)
        {
            var organizer = await _dbContext.Users
                .AsNoTracking()
                .Select(x => new UserBasicInfo
                {
                    Id = x.Id,
                    UserName = x.UserName!
                })
                .FirstOrDefaultAsync(x => x.Id == organizerId, cancellation);

            return organizer;
        }

        public async Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid id,CancellationToken cancellation)
        {

            var participants = await _dbContext.EventParticipants
                .AsNoTracking()
               .Where(ep => ep.EventId == id)
             .Join(
                 _dbContext.Users,
                 ep => ep.UserId,
                 u => u.Id,
                 (ep, u) => new UserBasicInfo
                 {
                     Id = u.Id,
                     UserName = u.UserName
                 }
             )
             .ToListAsync(cancellation);

            return participants;
        }

        public async Task<int> GetParticipantsCountAsync(Guid eventId,CancellationToken cancellation)
            => await _dbContext.EventParticipants
                .AsNoTracking()
               .Where(ep => ep.EventId == eventId)
               .CountAsync(cancellation);


        public async Task<bool> ExistsAsync(string userId, Guid eventId,CancellationToken cancellation)
        => await _dbContext.EventParticipants
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.EventId == eventId, cancellation);

        public async Task AddParticipantToEventAsync(string userId, Guid eventId,CancellationToken cancellation)
        {
            var eventParticipant = new EventParticipant
            {
                EventId = eventId,
                UserId = userId
            };

            await _dbContext.EventParticipants.AddAsync(eventParticipant, cancellation);
        }

        public async Task SaveChangesAsync(CancellationToken cancellation)
        {
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task RemoveParticipantFromEventAsync(string userId, Guid eventId, CancellationToken cancellation)
        {
            var entity =await _dbContext.EventParticipants
                .FirstOrDefaultAsync(x=>x.UserId == userId && eventId == x.EventId, cancellation);

            if(entity != null)
             _dbContext.EventParticipants.Remove(entity);
        }

        public async Task<HashSet<Guid>> GetJoinedEventIdsByUserAsync(string userId,CancellationToken cancellation)
       =>
            await _dbContext.EventParticipants
             .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.EventId)
                .ToHashSetAsync(cancellation);

     

        public IQueryable<Event> GetJoinedEventsByUserId(string userId)
        {
            var joinedEventIds = _dbContext.EventParticipants
                   .AsNoTracking()
                   .Where(x => x.UserId == userId)
                   .Select(x => x.EventId);

            return _dbContext.Events
                .AsNoTracking()
                .Where(e => joinedEventIds.Contains(e.Id));

        }

        public async Task<int> GetJoinedEventCountAsync(string userId,CancellationToken cancellation)
        => await _dbContext.EventParticipants.CountAsync(x => x.UserId == userId, cancellation);

        
    }
}
