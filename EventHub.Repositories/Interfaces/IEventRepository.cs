
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models;
    
    public interface IEventRepository
    {
        public Task<Event?> GetByIdAsync(Guid id);
        public Task<DetailedEventDto?> GetByIdReadOnlyAsync(Guid id);
        public IQueryable<Event> GetAll();
        public Task AddAsync(Event entity);

        public Task RemoveAsync(Event entity);

        IQueryable<Event> GetByOrganizerId(string id);

        Task<EventJoinInfo?> GetEventJoinInfoAsync(Guid id);

        Task<bool> TryJoinAsync(Guid eventId, string userId);
        IQueryable<Event?> Query();

        Task SaveChangesAsync();

    }
}
