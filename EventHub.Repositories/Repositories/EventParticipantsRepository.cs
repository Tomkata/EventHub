

namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Net.WebSockets;

    public class EventParticipantsRepository : IEventParticipantsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EventParticipantsRepository(ApplicationDbContext dbContext)
        {
           this._dbContext = dbContext;
        }

        public async Task<UserBasicInfo?> GetOrganizerAsync(string organizerId)
        {
            var organizer = await _dbContext.Users
                .AsNoTracking()
                .Select(x => new UserBasicInfo
                {
                    Id = x.Id,
                    UserName = x.UserName!
                })
                .FirstOrDefaultAsync(x => x.Id == organizerId);

            return organizer;
        }

        public async Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid id)
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
             .ToListAsync();

            return participants;
        }

        public async Task<int> GetParticipantsCountAsync(Guid eventId)
            => await _dbContext.EventParticipants
                .AsNoTracking()
               .Where(ep => ep.EventId == eventId)
               .CountAsync();


        public async Task<bool> ExistsAsync(string userId, Guid eventId)
        => await _dbContext.EventParticipants
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.EventId == eventId);

        public async Task AddParticipantToEventAsync(string userId, Guid eventId)
        {
            var eventParticipant = new EventParticipant
            {
                EventId = eventId,
                UserId = userId
            };

            await _dbContext.EventParticipants.AddAsync(eventParticipant);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
