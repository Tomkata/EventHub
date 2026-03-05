namespace EventHub.Repositories.Repositories.Organizer
{
    using EventHub.Core.Enums.Organizer;
    using EventHub.Core.Models.Organizer;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.Organizer;
    using Microsoft.EntityFrameworkCore;

    public class OrganizerRequestRepository : IOrganizerRequestRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrganizerRequestRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task AddAsync(OrganizerRequest organizer,CancellationToken cancellation)
            => await _dbContext.OrganizerRequests.AddAsync(organizer, cancellation);

        public IQueryable<OrganizerRequest> GetAll()
       => _dbContext.OrganizerRequests
            .AsNoTracking();

        public async Task<OrganizerRequest?> GetByUserIdAsync(string userId,CancellationToken cancellation)
            => await _dbContext.OrganizerRequests
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellation);


        public IQueryable<OrganizerRequest> GetPendingRequests()
            => _dbContext.OrganizerRequests
            .AsNoTracking()
            .Where(x => x.Status == Status.Pending)
            .OrderBy(x => x.CreatedAt);

        public async Task SaveChangesAsync(CancellationToken cancellation)
        {
            await _dbContext.SaveChangesAsync(cancellation);
        }
    }
}
