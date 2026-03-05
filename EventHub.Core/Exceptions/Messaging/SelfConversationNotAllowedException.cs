


namespace EventHub.Core.Exceptions.Messaging
{
using EventHub.Core.AppException;
    public class SelfConversationNotAllowedException : ConflictException
    {
        public SelfConversationNotAllowedException()
            :base("User cannot start conversation with themselves")
        {
        }
    }
}
