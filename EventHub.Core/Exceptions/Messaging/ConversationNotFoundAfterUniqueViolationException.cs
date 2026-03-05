
namespace EventHub.Core.Exceptions.Messaging
{
using EventHub.Core.AppException;
    public class ConversationNotFoundAfterUniqueViolationException : NotFoundException
    {
        public ConversationNotFoundAfterUniqueViolationException()
            :base("Invalid not fount after unique violation.")
        {
            
        }
    }
}
