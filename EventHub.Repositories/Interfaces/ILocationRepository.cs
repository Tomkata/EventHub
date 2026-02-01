using EventHub.Core.Models;

namespace EventHub.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        public Task<Location?> GetByIdAsync(Guid Id);
    }
}
