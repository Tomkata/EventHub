
namespace EventHub.Core.Exceptions.Event.ForJoin
{
    using EventHub.Core.AppException;

    public class EventFilledException : ConflictException
    {
        public EventFilledException()
            :base("This event is full.")
        {
        }
    }
}
