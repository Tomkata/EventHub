


namespace EventHub.Core.Exceptions.UserProfile
{
using EventHub.Core.AppException;
    public class ProfileAlreadyExistsException : ConflictException
    {
        public ProfileAlreadyExistsException()
            :base("User already have a profile.")
        {
        }
    }
}
