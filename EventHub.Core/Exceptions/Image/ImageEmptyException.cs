
namespace EventHub.Core.Exceptions.Image
{
    using EventHub.Core.AppException;

    public class ImageEmptyException : ValidationException
    {
        public ImageEmptyException()
            :base("The image file is empty!")
        {
        }
    }
}
