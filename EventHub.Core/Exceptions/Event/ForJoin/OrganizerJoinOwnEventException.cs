namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class OrganizerJoinOwnEventException : Exception
    {
        public OrganizerJoinOwnEventException()
            :base("The organizer cannot join their own event!")
        {
        }
    }
}
