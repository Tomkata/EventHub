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

        public Task AddAsync(Event entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Event entity)
        {
            throw new NotImplementedException();
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

        public Task UpdateAsync(Guid id, EditEventDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
