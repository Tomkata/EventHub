
namespace EventHub.Repositories.Interfaces
{
    using EventHub.Core.Models.Common;

    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Category> GetCategories();
    }
}
