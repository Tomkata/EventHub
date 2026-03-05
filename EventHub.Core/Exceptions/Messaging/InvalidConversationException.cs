
namespace EventHub.Core.Exceptions.Messaging
{
using EventHub.Core.AppException;
    public class InvalidConversationException : NotFoundException
    {
        public InvalidConversationException()
            :base("Invalid conversation.")
        {
            
        }
    }
}
