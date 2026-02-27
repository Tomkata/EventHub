
namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
using EventHub.Core.AppException;
    public class OrganizerRequestPendingException : ConflictException
    {
        public OrganizerRequestPendingException()
            :base("Organizer request is in pending!")
        {
        }
    }
}
