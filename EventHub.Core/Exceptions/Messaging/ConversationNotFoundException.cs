using EventHub.Core.AppException;
using System.Drawing;

namespace EventHub.Core.Exceptions.Messaging
{
    public class ConversationNotFoundException : NotFoundException
    {
        public ConversationNotFoundException()
                :base("Conversation not found.")
        {
            
        }
    }
}
