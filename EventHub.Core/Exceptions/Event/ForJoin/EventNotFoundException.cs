namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException()
            :base("The event could not be found.")
        {
        }
    }
}
