namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventFilledException : Exception
    {
        public EventFilledException()
            :base("The event is full!")
        {
        }
    }
}
