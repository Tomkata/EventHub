
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

    public interface IOrganizerRequestRepository
    {
        Task AddAsync(OrganizerRequest organizer);

        Task<OrganizerRequest> GetByUserIdAsync(string id);
        Task SaveChangesAsync();

        Task<IEnumerable<OrganizerRequest>> GetPengingRequestsAsync();
    }
}
