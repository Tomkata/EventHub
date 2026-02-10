using EventHub.Core.Models;
using EventHub.Infrastructure.Data;
using EventHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Category?> GetByIdAsync(Guid id) =>
             await _dbContext.Categories
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
       => await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
                .ToListAsync();
    }
}
