
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface ILocationRepository
    {
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Location> GetLocations();

    }
}
