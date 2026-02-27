
namespace EventHub.Core.Exceptions.Event.ForJoin
{
    using EventHub.Core.AppException;

    public class EventExpiredException : ConflictException
    {
        public EventExpiredException()
            : base("This event has already ended.")
        {
        }
    }
}
