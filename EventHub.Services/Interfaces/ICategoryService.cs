using EventHub.Core.DTOs.Category;
using System.Diagnostics.Contracts;

namespace EventHub.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<CategoryDto>> GetCategoriesForDropdownAsync(); 
        
    }
}
