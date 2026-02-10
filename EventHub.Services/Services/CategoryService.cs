
namespace EventHub.Services.Services
{
    using EventHub.Core.DTOs.Category;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository  _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> GetCategoriesForDropdownAsync()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();

            var categoriesDto = categories
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();

            return categoriesDto;
        }
    }
}
