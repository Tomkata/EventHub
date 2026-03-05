using EventHub.Core.Models.Events;
using EventHub.Infrastructure.Data;
using EventHub.Repositories.Interfaces.Event;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repositories.Repositories.Event
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Category?> GetByIdAsync(Guid id,CancellationToken cancellation) =>
             await _dbContext.Categories
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        public IQueryable<Category> GetCategories()
       => _dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name);
    }
}
