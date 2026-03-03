


namespace EventHub.Repositories.Interfaces.Social
{
    using EventHub.Core.Models.Social;

    public interface IUserFollowRepository
    {
        public IQueryable<UserFollow> GetAll();
        public Task AddUserFollowAsync(UserFollow userFollow, CancellationToken cancellation);
        public Task RemoveAsync(string followerId, string followingId,
           CancellationToken cancellationToken);
        public Task<UserFollow?> GetAsync(string followerId, string followingId, CancellationToken cancellation);

        public Task<bool> ExistAsync(string followerId, string followingId, CancellationToken cancellation);

        public Task SaveChangesAsync(CancellationToken cancellation);

    }
}
