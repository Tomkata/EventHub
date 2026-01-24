
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Location;
    public interface ILocationService
    {
        public Task<List<LocationDto>> GetLocationsForDropdownAsync();
    }
}
