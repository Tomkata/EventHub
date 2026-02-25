
using EventHub.Core.DTOs;

namespace EventHub.Services.Interfaces
{
    public interface IParticipantService
    {
        Task JoinEventAsync(string userId, Guid eventId);

        Task LeftEventAsync(string userId, Guid eventId);

        Task<HashSet<Guid>> GetJoinedEventIdsAsync(string userId);

        Task<List<EventDto>> GetJoinedEvents(string userId);

        Task<int> GetJoinedEventCountAsync(string userId);
    }
}
