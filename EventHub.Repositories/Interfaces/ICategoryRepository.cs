using EventHub.Core.Models;

namespace EventHub.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(Guid id);
    }
}
