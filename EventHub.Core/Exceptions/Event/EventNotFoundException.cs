
namespace EventHub.Core.Exceptions.Event
{
    using EventHub.Core.AppException;

    public class EventNotFoundException : NotFoundException
    {
        public EventNotFoundException()
            :base("Invalid event!")
        {
            
        }
    }
}
