namespace EventHub.Services.Interfaces.Common
{
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;

    public interface IImageService
    {
        public Task<string> StoreImageAsync(
            Stream stream,
            ImageFormat format,
            ImageFolder folder,
            CancellationToken cancellationToken);


        public Task<ImageFormat> DetectFormat(
            Stream stream, 
            CancellationToken cancellationToken); 

            
        public Task DeleteImageAsync(
            string imagePath);
    }
}
