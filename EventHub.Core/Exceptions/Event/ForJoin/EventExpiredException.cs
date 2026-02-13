namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventExpiredException : Exception
    {
        public EventExpiredException()
            : base("This event has already ended.")
        {
        }
    }
}
