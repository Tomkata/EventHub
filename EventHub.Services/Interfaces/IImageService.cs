
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.Enums;

    public interface IImageService
    {
        public Task<string> StoreImageAsync(Stream stream,string fileName,ImageFolder folder);

        public Task DeleteImageAsync(string imagePath);
    }
}
