using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Social
{
    public class CannotUnfollowYourselfException : ConflictException
    {
        public CannotUnfollowYourselfException()
            :base("You cannot unfollow yourself.")
        {
            
        }
    }
}
