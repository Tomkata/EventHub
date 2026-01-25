using Microsoft.AspNetCore.Http;

namespace EventHub.Services.Interfaces
{
    public interface IImageService
    {
        public Task<string> StoreImageAsync(IFormFile imageFile);
    }
}
