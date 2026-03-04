using EventHub.Core.Models;
using EventHub.Core.Models.Users;


namespace EventHub.Repositories.Interfaces
{
    /// <summary>
    /// The repository return read only model. This ensure not dependancy of Application user.
    /// </summary>
    public interface IEventParticipantsRepository
    {
        Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid eventId, CancellationToken cancellation);
        Task<UserBasicInfo?> UserExistsAsync(string userId, CancellationToken cancellation);

        Task<int> GetParticipantsCountAsync(Guid eventId, CancellationToken cancellation);

        Task<bool> ExistsAsync(string userId, Guid eventId, CancellationToken cancellation);

        Task AddParticipantToEventAsync(string userId, Guid eventId, CancellationToken cancellation);

        Task RemoveParticipantFromEventAsync(string userId, Guid eventId, CancellationToken cancellation);

        Task<HashSet<Guid>> GetJoinedEventIdsByUserAsync(string userId, CancellationToken cancellation);
        IQueryable<Event> GetJoinedEventsByUserId(string userId);

        Task<int> GetJoinedEventCountAsync(string userId, CancellationToken cancellation);

        Task SaveChangesAsync(CancellationToken cancellation);
    }
}

