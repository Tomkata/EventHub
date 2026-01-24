
namespace EventHub.Services.Services
{
    using EventHub.Core.DTOs.Location;
    using EventHub.Infrastructure.Data;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _dbContext;

        public LocationService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<LocationDto>> GetLocationsForDropdownAsync()
        {
            var locations = await _dbContext.Locations
                .AsNoTracking()
                .Select(x => new LocationDto
                {
                     Id = x.Id,
                     City = x.City,
                     ZipCode = x.Zip
                })
                .OrderBy(x=>x.City)
                .ThenByDescending(x=>x.ZipCode)
                .ToListAsync();


            return locations;
        }
    }
}
