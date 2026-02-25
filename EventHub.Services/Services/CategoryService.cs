
namespace EventHub.Services.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using EventHub.Core.DTOs.Category;
    using EventHub.Repositories.Interfaces;
    using EventHub.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository  _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository,
                                IMapper mapper)
        {
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetCategoriesForDropdownAsync()
        {
            return await _categoryRepository.GetCategories()
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
