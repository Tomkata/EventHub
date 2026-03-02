

namespace EventHub.Repositories.Repositories.Social
{

    using EventHub.Core.Models.Social;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.Social;
    using Microsoft.EntityFrameworkCore;
    public class UserFollowRepository : IUserFollowRepository
    {
        private readonly ApplicationDbContext _applicationDb;

        public UserFollowRepository(ApplicationDbContext applicationDb)
        {
            this._applicationDb = applicationDb;
        }
            
        public async Task AddUserFollowAsync(UserFollow userFollow)
        {
            await _applicationDb.UserFollows.AddAsync(userFollow);
        }

        public async Task<bool> ExistAsync(string followerId, string followingId)
        => await _applicationDb.UserFollows
            .AsNoTracking()
            .AnyAsync(x => x.FollowerId == followerId && x.FollowingId == followingId);

        public IQueryable<UserFollow> GetAll()
        => _applicationDb.UserFollows
            .AsNoTracking();

        public void RemoveUserFollowAsync(UserFollow userFollow)
        {
            _applicationDb.UserFollows.Remove(userFollow);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDb.SaveChangesAsync();
        }
    }
}
