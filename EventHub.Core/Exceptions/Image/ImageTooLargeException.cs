namespace EventHub.Core.Exceptions.Image
{
    using EventHub.Core.AppException;

    public class ImageTooLargeException : ValidationException
    {
        public ImageTooLargeException()
            :base("The image size is too large!")
        {
            
        }
    }
}
