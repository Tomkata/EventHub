

using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Social
{
    public class FollowingProfileNotCompleted : NotFoundException
    {
        public FollowingProfileNotCompleted() 
            :base("You need to complete your profile.")
        {
            
        }
    }
}
