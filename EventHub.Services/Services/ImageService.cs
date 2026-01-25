

namespace EventHub.Services.Services
{
    using EventHub.Core.enums.Image;
    using EventHub.Core.Exceptions.Image;
    using EventHub.Services.Images;
    using EventHub.Services.Interfaces;
    using Microsoft.AspNetCore.Http;
    using static System.Net.Mime.MediaTypeNames;

    //Image validation service
    /*
     If someone rename the .exe to .jpg
    we will validate this, couse the bytes are gonna be different 
    */
    public class ImageService : IImageService
    {
        public async Task<string> StoreImageAsync(IFormFile imageFile)
        {
            if (imageFile == null) throw new ImageEmptyException();
            if (imageFile.Length == 0) throw new ImageEmptyException();

             using  var stream = imageFile.OpenReadStream();

            byte[] buffer = new byte[16];

            var isReaded = 0;

            
                isReaded =  await stream.ReadAsync(buffer,0,buffer.Length);

            //if the readed bytes are less than 4, the format is invalid
            if(isReaded < 4) throw new InvalidImageFormatException();

            var format = FindImageFormat(buffer);

            if (format  == ImageFormat.unknown) throw new InvalidImageFormatException();

            var imageUrl = $"/images/events/{Guid.NewGuid()}.{format.ToString()}";

            return imageUrl; //for now
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
