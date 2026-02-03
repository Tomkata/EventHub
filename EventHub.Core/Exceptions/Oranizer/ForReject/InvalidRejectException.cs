namespace EventHub.Core.Exceptions.Oranizer.ForReject
{
    public class InvalidRejectException : Exception
    {
        public InvalidRejectException()
            :base("The user is already in rejected status!")
        {
        }
    }
}
