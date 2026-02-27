
namespace EventHub.Services.Interfaces
{
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;

    public interface IImageService
    {
        public Task<string> StoreImageAsync(
            Stream stream,
            ImageFormat format,
            ImageFolder folder);


        public Task<ImageFormat> DetectFormat(Stream stream); 

            
        public Task DeleteImageAsync(string imagePath);
    }
}
