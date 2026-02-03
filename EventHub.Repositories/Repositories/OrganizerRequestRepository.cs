

namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class OrganizerRequestRepository : IOrganizerRequestRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrganizerRequestRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddAsync(OrganizerRequest organizer)
            => await _dbContext.OrganizerRequests.AddAsync(organizer);

        public async Task<OrganizerRequest?> GetByUserIdAsync(string id)=>
            await _dbContext.OrganizerRequests
                .FirstOrDefaultAsync(x => x.UserId == id);


        public async Task<IEnumerable<OrganizerRequest>> GetPendingRequestsAsync()
            => await _dbContext.OrganizerRequests
            .AsNoTracking()
            .Where(x => x.Status == Status.Pending)
            .ToListAsync();

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
