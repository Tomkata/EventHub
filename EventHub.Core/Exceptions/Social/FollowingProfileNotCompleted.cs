


namespace EventHub.Core.Exceptions.Social
{
using EventHub.Core.AppException;
    public class FollowingProfileNotCompleted : NotFoundException
    {
        public FollowingProfileNotCompleted() 
            :base("You need to complete your profile.")
        {
            
        }
    }
}
