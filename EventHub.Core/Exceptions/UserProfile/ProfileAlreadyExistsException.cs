namespace EventHub.Core.Exceptions.UserProfile
{
    public class ProfileAlreadyExistsException : Exception
    {
        public ProfileAlreadyExistsException()
            :base("The profile already exists.")
        {
            
        }
    }
}
