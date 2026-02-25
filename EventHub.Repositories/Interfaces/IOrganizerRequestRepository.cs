
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface IOrganizerRequestRepository
    {
        Task AddAsync(OrganizerRequest organizer);

        Task<OrganizerRequest?> GetByUserIdAsync(string userId);
        Task SaveChangesAsync();

        IQueryable<OrganizerRequest> GetPendingRequests();

        IQueryable<OrganizerRequest> GetAll();
    }
}



