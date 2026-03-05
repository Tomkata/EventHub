namespace EventHub.Services.Services.Common
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.Common;
    using EventHub.Core.DTOs.Location;
    using EventHub.Infrastructure.Data;
    using EventHub.Repositories.Interfaces.Common;
    using EventHub.Services.Interfaces.Common;
    using Microsoft.EntityFrameworkCore;

    public class LocationService : ILocationService
    {
        private readonly ILocationRepository  _locationRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository,
                                IMapper mapper)
        {
            this._locationRepository = locationRepository;
            this._mapper = mapper;
        }

        public async Task<List<DropdownOptionModel>> GetLocationsForDropdownAsync(CancellationToken cancellation)
    => await _locationRepository.GetLocations()
        .ProjectTo<DropdownOptionModel>(_mapper.ConfigurationProvider)
        .ToListAsync(cancellation);
    }
}
