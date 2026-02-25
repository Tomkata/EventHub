

namespace EventHub.Core.Exceptions.UserProfile
{
    public class UserAlreadyHaveProfileException : Exception
    {
        public UserAlreadyHaveProfileException()
            :base("User already have a profile.")
        {
            
        }
    }
}
