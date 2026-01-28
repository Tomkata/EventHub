namespace EventHub.Core.Exceptions.Event
{
    public class InvalidEventException : Exception
    {
        public InvalidEventException()
            :base("Invalid event!")
        {
            
        }
    }
}
