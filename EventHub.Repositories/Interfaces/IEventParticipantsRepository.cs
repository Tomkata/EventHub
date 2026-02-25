using EventHub.Core.Models;


namespace EventHub.Repositories.Interfaces
{
    /// <summary>
    /// The repository return read only model. This ensure not dependancy of Application user.
    /// </summary>
    public interface IEventParticipantsRepository
    {
        Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid eventId);
        Task<UserBasicInfo ?> UserExistsAsync(string userId);

        Task<int> GetParticipantsCountAsync(Guid eventId);

        Task<bool> ExistsAsync(string userId, Guid eventId);

        Task AddParticipantToEventAsync(string userId, Guid eventId);

        Task RemoveParticipantFromEventAsync(string userId, Guid eventId);

        Task<HashSet<Guid>> GetJoinedEventIdsByUserAsync(string userId);
        IQueryable<Event> GetJoinedEventsByUserId(string userId);

        Task<int> GetJoinedEventCountAsync(string userId);

        Task SaveChangesAsync();
    }
}

