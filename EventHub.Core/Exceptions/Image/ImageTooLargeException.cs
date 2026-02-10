namespace EventHub.Core.Exceptions.Image
{
    public class ImageTooLargeException : Exception
    {
        public ImageTooLargeException()
            :base("The image size is too large!")
        {
            
        }
    }
}
