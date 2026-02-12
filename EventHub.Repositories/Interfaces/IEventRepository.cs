
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models;
    
    public interface IEventRepository
    {
        public Task<Event?> GetByIdAsync(Guid id);
        public Task<Event?> GetByIdReadOnlyAsync(Guid id);
        public Task<IEnumerable<Event>> GetAllAsync();
        public Task AddAsync(Event entity);

        public Task UpdateAsync(Event entity);

        public Task RemoveAsync(Event entity);

        Task<IEnumerable<Event>> GetAllEventsByOrganizerIdAsync(string id);
        Task<EventJoinInfo?> GetEventJoinInfoAsync(Guid id);

    }
}
