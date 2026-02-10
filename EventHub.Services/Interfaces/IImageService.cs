
namespace EventHub.Services.Interfaces
{
    using Microsoft.AspNetCore.Http;

    public interface IImageService
    {
        public Task<string> StoreImageAsync(IFormFile imageFile);
    }
}
