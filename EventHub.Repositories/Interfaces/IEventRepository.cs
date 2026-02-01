using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Event;
using EventHub.Core.Models;

namespace EventHub.Repositories.Interfaces
{
    public interface IEventRepository
    {
        public Task<Event?> GetByIdAsync(Guid id);

        public Task AddAsync(Event entity);

        public Task UpdateAsync(Guid id, EditEventDto dto);

        public Task RemoveAsync(Event entity);
    }
}
