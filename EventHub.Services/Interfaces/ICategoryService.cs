
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.DTOs.Category;

    public interface ICategoryService
    {
        public Task<List<CategoryDto>> GetCategoriesForDropdownAsync(); 
        
    }
}
