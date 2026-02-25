
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface ILocationRepository
    {
        public Task<Location?> GetByIdAsync(Guid id);

        public IQueryable<Location> GetLocations();

    }
}
