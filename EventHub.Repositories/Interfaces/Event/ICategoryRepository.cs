namespace EventHub.Repositories.Interfaces.Event
{
    using EventHub.Core.Models.Events;

    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellation);

        public IQueryable<Category> GetCategories();
    }
}
