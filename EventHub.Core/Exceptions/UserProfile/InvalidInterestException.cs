namespace EventHub.Core.Exceptions.UserProfile
{
    public class InvalidInterestException : Exception
    {
        public InvalidInterestException()
            :base("Invalid interest provided.")
        {
        }
    }
}
