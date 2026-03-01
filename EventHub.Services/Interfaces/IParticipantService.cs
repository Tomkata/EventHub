

namespace EventHub.Services.Interfaces
{

    using EventHub.Core.DTOs;
    using EventHub.Services.Common;
    public interface IParticipantService
    {
        Task JoinEventAsync(
            string userId, 
            Guid eventId, 
            CancellationToken cancellationToken);

        Task LeftEventAsync(
            string userId,
            Guid eventId,
            CancellationToken cancellation);

        Task<HashSet<Guid>> GetJoinedEventIdsAsync(
            string userId,
            CancellationToken cancellation);

        Task<PagedResult<EventDto>> GetJoinedEvents(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<int> GetJoinedEventCountAsync(
            string userId,
            CancellationToken cancellation);
    }
}
