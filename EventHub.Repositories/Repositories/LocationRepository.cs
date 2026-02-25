

namespace EventHub.Repositories.Repositories
{
    using EventHub.Core.Models;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LocationRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Location?> GetByIdAsync(Guid Id) =>
            await _dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == Id);

        public IQueryable<Location> GetLocations()
       =>  _dbContext.Locations
            .AsNoTracking()
            .OrderBy(x => x.City);
    }
}
