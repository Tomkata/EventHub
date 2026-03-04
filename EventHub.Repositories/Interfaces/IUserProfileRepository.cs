using EventHub.Core.Models;
using EventHub.Core.Models.Users;

namespace EventHub.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task AddAsync(UserProfile profile, CancellationToken cancellation);

        void Delete(UserProfile profile);

        Task<bool> ExistsAsync(string userId, CancellationToken cancellation);
        Task<List<Interest>> GetInterestsByIdsAsync(HashSet<Guid> ids, CancellationToken cancellation);

        Task<int> GetInterestsCountAsync(HashSet<Guid> ids, CancellationToken cancellation);

        Task<UserProfile?> GetByUserIdAsyncReadOnly(string userId, CancellationToken cancellation);
        Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellation);
        IQueryable<UserProfile> GetAll();
        Task SaveChangesAsync(CancellationToken cancellation);
    }
}
