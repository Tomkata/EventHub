namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class OrganizerJoinOwnEventException : Exception
    {
        public OrganizerJoinOwnEventException()
            :base("Organizers cannot join their own events.")
        {
        }
    }
}
