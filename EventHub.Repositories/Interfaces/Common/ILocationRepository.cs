namespace EventHub.Repositories.Interfaces.Common
{
    using EventHub.Core.Models.Common;

    public interface ILocationRepository
    {
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Location> GetLocations();

    }
}
