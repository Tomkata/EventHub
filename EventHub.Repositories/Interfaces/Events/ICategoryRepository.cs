namespace EventHub.Repositories.Interfaces.Events
{
    using EventHub.Core.Models.Events;

    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Category> GetCategories();
    }
}
