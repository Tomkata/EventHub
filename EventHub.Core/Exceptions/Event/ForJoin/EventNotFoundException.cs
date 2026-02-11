namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException()
            :base("Event does not exist")
        {
        }
    }
}
