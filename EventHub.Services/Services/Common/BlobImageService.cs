

namespace EventHub.Services.Services.Common
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using EventHub.Core.enums.Image;
    using EventHub.Core.Enums;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Services.Images;
    using EventHub.Services.Interfaces.Common;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static System.Net.Mime.MediaTypeNames;
    public class BlobImageService : IImageService
    {
        private readonly BlobContainerClient _container;

        public BlobImageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage__ConnectionString"];
            var containerName = configuration["AzureStorage__ContainerName"] ?? "images";
            this._container = new BlobContainerClient(connectionString,containerName);
        }

        public Task DeleteImageAsync(string imageUrl)
        {
            if (imageUrl == null) throw new ImageEmptyException();

            var uri = new Uri(imageUrl);

            var blobName = string.Join("/", uri.Segments[2..]);

            var blob = _container.GetBlobClient(blobName);
            return blob.DeleteAsync();
            


        }

  
        public async Task<string> StoreImageAsync(Stream stream, ImageFormat format, ImageFolder folder, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            var blobName = $"{folder.ToString().ToLower()}/{guid}.{format}";

            var blob = _container.GetBlobClient(blobName);

            var contentType = format == ImageFormat.png ? "image/png" : "image/jpeg";

            await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

            return blob.Uri.ToString();

        }

        public async Task<ImageFormat> DetectFormat(Stream stream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[16];
            await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            stream.Position = 0;
            return FindImageFormat(buffer);
        }


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
