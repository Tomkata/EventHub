namespace EventHub.Core.Exceptions.User
{
    public class InvalidOrganizerException : Exception
    {
        public InvalidOrganizerException()
            :base("Invalid organizer!")
        {
            
        }
    }
}
