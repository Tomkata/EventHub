namespace EventHub.Core.Exceptions.Image
{
    public class InvalidImageFormatException : Exception
    {
        public InvalidImageFormatException()
            :base("The image format is invalid!")
        {
        }
    }
}
