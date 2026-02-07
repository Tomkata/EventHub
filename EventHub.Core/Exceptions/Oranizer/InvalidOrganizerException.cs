namespace EventHub.Core.Exceptions.Oranizer
{
    public class InvalidOrganizerException : Exception
    {
        public InvalidOrganizerException()
            :base("Organizer doesnt exist!")
        {
            
        }
    }
}
