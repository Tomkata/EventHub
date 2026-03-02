

using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Social
{
    public class CannotFollowYourselfException : ConflictException
    {
        public CannotFollowYourselfException()
            :base("You cannot follow yourself.")
        {
            
        }
    }
}
