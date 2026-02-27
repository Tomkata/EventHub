
namespace EventHub.Core.Exceptions.Event.ForJoin
{
    using EventHub.Core.AppException;

    public class EventNotFoundException : NotFoundException
    {
        public EventNotFoundException()
            :base("The event could not be found.")
        {
        }
    }
}
