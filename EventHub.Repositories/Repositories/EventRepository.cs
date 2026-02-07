
namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.DTOs;
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class EventRepository : IEventRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var eventEntity = await _dbContext.Events
                   .Include(x => x.Category)
                   .Include(x => x.Location)
                   .Include(x => x.EventParticipants)
                   .AsSplitQuery()
                   .FirstOrDefaultAsync(x => x.Id == id);

            return eventEntity;
        }
        public async Task<Event?> GetByIdReadOnlyAsync(Guid id)
        {
            var eventEntity = await _dbContext.Events
                   .AsNoTracking()
                     .Include(x => x.Category)
                     .Include(x => x.Location)
                     .Include(x => x.EventParticipants)
                     .AsSplitQuery()
                     .FirstOrDefaultAsync(x => x.Id == id);

            return eventEntity;
        }

        public async Task AddAsync(Event entity)
        {
            await _dbContext.Events.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(Event entity)
        {
            _dbContext.Events.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Event entity)
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _dbContext.Events
                  .AsNoTracking()
                  .Include(x => x.Category)
                  .Include(x => x.Location)
                  .Include(x => x.EventParticipants)
                  .AsSplitQuery()
                  .ToListAsync();
        }


        public async Task<IEnumerable<Event>> GetAllEventsByOrganizerIdAsync(string id)
        {
         return  await _dbContext.Events
                .AsNoTracking()
                .Where(x => x.OrganizerId == id)
                .Include(x => x.Category)
                  .Include(x => x.Location)
                  .Include(x => x.EventParticipants)
                  .AsSplitQuery()
                .ToListAsync();
        }
    }
}
