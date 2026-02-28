
using EventHub.Core.DTOs;
using EventHub.Services.Common;

namespace EventHub.Services.Interfaces
{
    public interface IParticipantService
    {
        Task JoinEventAsync(string userId, Guid eventId);

        Task LeftEventAsync(string userId, Guid eventId);

        Task<HashSet<Guid>> GetJoinedEventIdsAsync(string userId);

        Task<PagedResult<EventDto>> GetJoinedEvents(string userId, int pageNumber, int pageSize);

        Task<int> GetJoinedEventCountAsync(string userId);
    }
}
