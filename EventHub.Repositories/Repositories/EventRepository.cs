using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Event;
using EventHub.Core.Exceptions.Event;
using EventHub.Core.Models;
using EventHub.Infrastructure.Data;
using EventHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repositories.Repositories
{
    public class EventRepository : IEventRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public EventRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public  async Task AddAsync(Event entity)
        {
            await _dbContext.Events.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
            
        public async Task RemoveAsync(Event entity)
        {
            _dbContext.Events.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
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

        public async Task UpdateAsync(Event entity)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
