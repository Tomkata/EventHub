
namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserProfileRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }


        public async Task AddAsync(UserProfile profile)
        {
            await _dbContext.UserProfiles.AddAsync(profile);
        }

        public void Delete(UserProfile profile)
        {
            _dbContext.UserProfiles.Remove(profile);
        }

        public async Task<bool> ExistsAsync(string userId)
       => await _dbContext.UserProfiles
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId);

        public IQueryable<UserProfile> GetAll()
        => _dbContext.UserProfiles.AsQueryable();

        public async Task<List<Interest>> GetInterestsByIdsAsync(HashSet<Guid> ids)
        => await _dbContext.Interests
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

        public Task<UserProfile?> GetByUserIdAsync(string userId)
       => _dbContext.UserProfiles
            .Include(x=>x.UserProfileInterests)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        public Task<UserProfile?> GetByUserIdAsyncReadOnly(string userId)
          => _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);

     

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
