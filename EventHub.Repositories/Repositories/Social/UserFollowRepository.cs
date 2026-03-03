

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
            
        public async Task AddUserFollowAsync(UserFollow userFollow,CancellationToken cancellation)
        {
            await _applicationDb.UserFollows.AddAsync(userFollow, cancellation);
        }

        public async Task<bool> ExistAsync(string followerId, string followingId,CancellationToken cancellation)
        => await _applicationDb.UserFollows
            .AsNoTracking()
            .AnyAsync(x => x.FollowerId == followerId && x.FollowingId == followingId, cancellation);

        public IQueryable<UserFollow> GetAll()
        => _applicationDb.UserFollows
            .AsNoTracking();

        public async Task<UserFollow?> GetAsync(string followerId, string followingId, CancellationToken cancellation)
        => await _applicationDb.UserFollows
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FollowerId == followerId &&
            x.FollowingId == followingId,cancellation);

        public async Task RemoveAsync(string followerId, string followingId, 
            CancellationToken cancellationToken)
        {
            await _applicationDb.UserFollows
                .Where(x => x.FollowerId == followerId && 
                            x.FollowingId == followingId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellation)
        {
            await _applicationDb.SaveChangesAsync(cancellation);
        }
    }
}
