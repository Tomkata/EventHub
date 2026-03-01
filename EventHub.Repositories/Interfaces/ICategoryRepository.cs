
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models;

    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Category> GetCategories();
    }
}
