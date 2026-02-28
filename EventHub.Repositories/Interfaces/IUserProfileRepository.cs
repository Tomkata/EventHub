using EventHub.Core.Models;

namespace EventHub.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task AddAsync(UserProfile profile);

        void Delete(UserProfile profile);

        Task<bool> ExistsAsync(string userId);
        Task<List<Interest>> GetInterestsByIdsAsync(HashSet<Guid> ids);

        Task<int> GetInterestsCountAsync(HashSet<Guid> ids);

        Task<UserProfile?> GetByUserIdAsyncReadOnly(string userId);
        Task<UserProfile?> GetByUserIdAsync(string userId);
        IQueryable<UserProfile> GetAll();
        Task SaveChangesAsync();
    }
}
