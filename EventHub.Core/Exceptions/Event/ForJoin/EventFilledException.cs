namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventFilledException : Exception
    {
        public EventFilledException()
            :base("This event is full.!")
        {
        }
    }
}
