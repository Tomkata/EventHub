

namespace EventHub.Core.Exceptions.Event.ForLeft
{
    using EventHub.Core.AppException;
    public class UserNotParticipantException : ConflictException
    {
        public UserNotParticipantException()
            :base("User is not participant in this event!")
        {
        }
    }
}
