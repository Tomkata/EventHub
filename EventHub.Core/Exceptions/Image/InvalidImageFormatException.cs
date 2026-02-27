namespace EventHub.Core.Exceptions.Image
{
    using EventHub.Core.AppException;

    public class InvalidImageFormatException : ValidationException
    {
        public InvalidImageFormatException()
            :base("The image format is invalid!")
        {
        }
    }
}
