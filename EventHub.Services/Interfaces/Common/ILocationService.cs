namespace EventHub.Services.Interfaces.Common
{
    using EventHub.Core.Common;
    using EventHub.Core.DTOs.Location;
    public interface ILocationService
    {
        public Task<List<DropdownOptionModel>> GetLocationsForDropdownAsync(CancellationToken cancellationToken);
    }
}
