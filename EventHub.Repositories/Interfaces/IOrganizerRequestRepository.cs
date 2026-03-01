
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface IOrganizerRequestRepository
    {
        Task AddAsync(OrganizerRequest organizer, CancellationToken cancellation);

        Task<OrganizerRequest?> GetByUserIdAsync(string userId, CancellationToken cancellation);
        Task SaveChangesAsync(CancellationToken cancellation);

        IQueryable<OrganizerRequest> GetPendingRequests();

        IQueryable<OrganizerRequest> GetAll();
    }
}



