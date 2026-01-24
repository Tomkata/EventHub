
namespace EventHub.Services.Services
{
    using EventHub.Core.DTOs.Category;
    using EventHub.Infrastructure.Data;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<CategoryDto>> GetCategoriesForDropdownAsync()
        {
            var categories = await _dbContext.Categories
                .AsNoTracking()
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

            return categories;
        }
    }
}
