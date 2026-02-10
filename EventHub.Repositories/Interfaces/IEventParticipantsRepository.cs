using EventHub.Core.Models;


namespace EventHub.Repositories.Interfaces
{
    /// <summary>
    /// The repository return read only model. This ensure not dependancy of Application user.
    /// </summary>
    public interface IEventParticipantsRepository
    {
        Task<IEnumerable<UserBasicInfo>> GetParticipantsAsync(Guid eventId);
        Task<UserBasicInfo?> GetOrganizerAsync(string organizerId);
    }
}

