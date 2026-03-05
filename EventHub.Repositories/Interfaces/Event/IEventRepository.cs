namespace EventHub.Repositories.Interfaces.Event
{
    using EventHub.Core.DTOs;
    using EventHub.Core.DTOs.Event;
    using EventHub.Core.Models.Events;

    public interface IEventRepository
    {
        public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellation);
        public Task<DetailedEventDto?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellation);
        public IQueryable<Event> GetAll();
        public Task AddAsync(Event entity, CancellationToken cancellation);

        public void Remove(Event entity);

        IQueryable<Event> GetByOrganizerId(string id);

        Task<EventJoinInfo?> GetEventJoinInfoAsync(Guid id, CancellationToken cancellation);

        Task<bool> TryJoinAsync(Guid eventId, string userId, CancellationToken cancellation);
        IQueryable<Event?> Query();

        Task SaveChangesAsync(CancellationToken cancellationToken);

    }
}
