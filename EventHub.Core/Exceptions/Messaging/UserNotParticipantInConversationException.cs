
using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Messaging
{
    public class UserNotParticipantInConversationException : ForbiddenException
    {
        public UserNotParticipantInConversationException()
                : base("User is not a participant in this conversation.")
        {
            
        }
    }
}
