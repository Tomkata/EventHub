


namespace EventHub.Repositories.Interfaces.Social
{
    using EventHub.Core.Models.Social;

    public interface IUserFollowRepository
    {

        public IQueryable<UserFollow> GetAll();
        public Task AddUserFollowAsync(UserFollow userFollow);
        public void RemoveUserFollowAsync(UserFollow userFollow);

        public Task<bool> ExistAsync(string followerId, string followingId);

        public Task SaveChangesAsync();

    }
}
