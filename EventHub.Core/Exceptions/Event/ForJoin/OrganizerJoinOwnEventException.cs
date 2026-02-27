
namespace EventHub.Core.Exceptions.Event.ForJoin
{
    using EventHub.Core.AppException;

    public class OrganizerJoinOwnEventException : ForbiddenException
    {
        public OrganizerJoinOwnEventException()
            :base("Organizers cannot join their own events.")
        {
        }
    }
}
