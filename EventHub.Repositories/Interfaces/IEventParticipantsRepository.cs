using EventHub.Core.Models;


namespace EventHub.Repositories.Interfaces
{
    /// <summary>
    /// The repository return read only model. This ensure not dependancy of Application user.
    /// </summary>
    public interface IEventParticipantsRepository
    {
        Task<List<UserBasicInfo>> GetParticipantsAsync(Event entity);
        Task<UserBasicInfo> GetOrganizerUserNameAsync(string organizerId);
    }
}

