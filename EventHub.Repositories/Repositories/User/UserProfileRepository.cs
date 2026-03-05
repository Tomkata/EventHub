namespace EventHub.Repositories.Repositories.User
{
    using EventHub.Core.Models.Common;
    using EventHub.Core.Models.Users;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.User;
    using Microsoft.EntityFrameworkCore;

    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserProfileRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        public async Task AddAsync(UserProfile profile,CancellationToken cancellation)
        {
            await _dbContext.UserProfiles.AddAsync(profile, cancellation);
        }

        public void Delete(UserProfile profile)
        {
            _dbContext.UserProfiles.Remove(profile);
        }

        public async Task<bool> ExistsAsync(string userId,CancellationToken cancellation)
       => await _dbContext.UserProfiles
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId, cancellation);

        public IQueryable<UserProfile> GetAll()
        => _dbContext.UserProfiles.AsQueryable();

        public async Task<List<Interest>> GetInterestsByIdsAsync(HashSet<Guid> ids,CancellationToken cancellation)
        => await _dbContext.Interests
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellation);

        public Task<UserProfile?> GetByUserIdAsync(string userId,CancellationToken cancellation)
       => _dbContext.UserProfiles
            .Include(x=>x.UserProfileInterests)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellation);

        public Task<UserProfile?> GetByUserIdAsyncReadOnly(string userId, CancellationToken cancellation)
          => _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellation);

     

        public async Task SaveChangesAsync(CancellationToken cancellation)
        {
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task<int> GetInterestsCountAsync(HashSet<Guid> ids,CancellationToken cancellation)
        => await _dbContext
            .Interests.CountAsync(x=>ids.Contains(x.Id), cancellation);
    }
}
