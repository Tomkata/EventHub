
namespace EventHub.Services.Services
{
    using EventHub.Core.DTOs.Location;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class LocationService : ILocationService
    {
        private readonly ILocationRepository  _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<List<LocationDto>> GetLocationsForDropdownAsync()
        {
            var locations = await _locationRepository.GetLocationsAsync();

            var locationsDtos = locations.
                Select(x => new LocationDto
                {
                    Id = x.Id,
                    City = x.City,
                    ZipCode = x.Zip
                })
                .ToList();

            return locationsDtos;
        }
    }
}
