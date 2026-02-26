
namespace EventHub.Core.Exceptions.UserProfile
{
using EventHub.Core.AppException;
    public class ProfileRequiredException : ConflictException
    {
        public ProfileRequiredException()
            :base("You need to complete your profile.")
        {
        }
    }
}
