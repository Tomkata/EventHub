

namespace EventHub.Services.Services
{
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Services.Images;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;

    //Image validation service
    /*
     If someone rename .jpg  to .exe file,
    we will validate this, couse the bytes are gonna be different 
    */


    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment env;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            this.env = webHostEnvironment;
        }

        public Task DeleteImageAsync(string imagePath)
        {
            if (imagePath == null) throw new ImageEmptyException();


            var physicalPath = Path.Combine(env.WebRootPath, imagePath);

            if (File.Exists(physicalPath))
                File.Delete(physicalPath);

            return Task.CompletedTask;
        }

        public async Task<ImageFormat> DetectFormat(Stream stream,CancellationToken cancellation)
        {
            byte[] buffer = new byte[16];

            var isReaded = 0;

            isReaded = await stream.ReadAsync(buffer, 0, buffer.Length, cancellation);
            stream.Position = 0;

            return FindImageFormat(buffer);
        }



        public async Task<string> StoreImageAsync(
            Stream stream,
            ImageFormat format,
            ImageFolder folder,
            CancellationToken cancellation)
        {


            var guid = Guid.NewGuid();
            var fileName = $"{guid}.{format}";

            var physicalFolder = Path.Combine(env.WebRootPath, "images", folder.ToString().ToLower());

            Directory.CreateDirectory(physicalFolder);

            var physicalPath = Path.Combine(physicalFolder, fileName);

            using var fileStream = new FileStream(physicalPath, FileMode.Create);

            await stream.CopyToAsync(fileStream, cancellation);

            var imageUrl = $"images/{folder.ToString().ToLower()}/{fileName}";
            return imageUrl;
        }

        //With bytes we check if the format is correct
        //We compare the first bytes and decide if the image is in correct format
        private ImageFormat FindImageFormat(byte[] bytes)
        {

            if (ImageSignatures.png.SequenceEqual(bytes.Take(ImageSignatures.png.Length)))
                return ImageFormat.png;

            if (ImageSignatures.jpeg.SequenceEqual(bytes.Take(ImageSignatures.jpeg.Length)))
                return ImageFormat.jpeg;

            if (ImageSignatures.jpeg2.SequenceEqual(bytes.Take(ImageSignatures.jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }
    }
}
