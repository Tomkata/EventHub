namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventExpiredException : Exception
    {
        public EventExpiredException()
            : base("Cannot join the event because it has already ended.")
        {
        }
    }
}
