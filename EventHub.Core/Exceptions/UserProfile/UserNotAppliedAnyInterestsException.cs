
namespace EventHub.Core.Exceptions.UserProfile
{
using EventHub.Core.AppException;
    public class UserNotAppliedAnyInterestsException : ValidationException
    {
        public UserNotAppliedAnyInterestsException()
            :base("the user must have at least one added interest.")
        {
        }
    }
}
