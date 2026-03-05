namespace EventHub.Services.Interfaces.Event
{
    using EventHub.Core.DTOs.Category;

    public interface ICategoryService
    {
        public Task<List<CategoryDto>> GetCategoriesForDropdownAsync(CancellationToken cancellation); 
    }
}
