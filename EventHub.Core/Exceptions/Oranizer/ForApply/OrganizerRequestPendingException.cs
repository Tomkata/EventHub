namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class OrganizerRequestPendingException : Exception
    {
        public OrganizerRequestPendingException()
            :base("Organizer request is in pending!")
        {
        }
    }
}
