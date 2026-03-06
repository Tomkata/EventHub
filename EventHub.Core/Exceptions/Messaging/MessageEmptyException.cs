
using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Messaging
{
    public class MessageEmptyException :ForbiddenException
    {
        public MessageEmptyException()
            :base("Message cannot be empty.")
        {
            
        }
    }
}
